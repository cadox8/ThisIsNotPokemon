using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace pokemon {
    
    [System.Serializable]
    public class Pokemon {

        [SerializeField] private PokemonBase _base;
        [SerializeField] private int level;

        public PokemonBase Base => _base;

        public int Level => level;

        public int HP { get; set; }
        
        public List<Move> Moves { get; set; }
        public Move CurrentMove { get; set; }
        
        public Dictionary<Stat, int> Stats { get; private set; }
        public Dictionary<Stat, int> StatsBoosts { get; private set; }
        public Condition Status { get; set; }
        public int StatusTime { get; set; }
        public Condition VolatileStatus { get; set; }
        public int VolatileStatusTime { get; set; }
        
        public Queue<string> StatusChanges { get; private set; }
        public bool HpChanged { get; set; }

        public event Action OnStatusChanged;

        
        public Pokemon(PokemonBase pBase, int level)
        {
            _base = pBase;
            this.level = level;
            
            Init();
        }
        
        public void Init()
        {
            // --- Generate Moves ---
            Moves = new List<Move>();
            
            foreach (var move in Base.LearnableMoves)
            {
                if (move.level <= Level) Moves.Add(new Move(move.moveBase));
                
                if (Moves.Count >= 4) break;
            }
            
            CalculateStats();
            HP = Base.maxHp;

            StatusChanges = new Queue<string>();
            
            ResetStatBoost();
            Status = null;
            VolatileStatus = null;
        }

        private void CalculateStats()
        {
            Stats = new Dictionary<Stat, int>();
            Stats.Add(Stat.Ataque, Mathf.FloorToInt((Base.attack * Level) / 100f) + 5);
            Stats.Add(Stat.Defensa, Mathf.FloorToInt((Base.defense * Level) / 100f) + 5);
            Stats.Add(Stat.Ataque_Especial, Mathf.FloorToInt((Base.SpAttack * Level) / 100f) + 5);
            Stats.Add(Stat.Defensa_Especial, Mathf.FloorToInt((Base.SpDefense * Level) / 100f) + 5);
            Stats.Add(Stat.Velocidad, Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5);

            MaxHp = Mathf.FloorToInt((Base.MaxHp * Level) / 100f) + 10 + Level;
        }

        private void ResetStatBoost()
        {
            StatsBoosts = new Dictionary<Stat, int>()
            {
                {Stat.Ataque, 0},
                {Stat.Defensa, 0},
                {Stat.Ataque_Especial, 0},
                {Stat.Defensa_Especial, 0},
                {Stat.Velocidad, 0},
                {Stat.Precision, 0},
                {Stat.Evasion, 0},
            };
        }

        private int GetStat(Stat stat)
        {
            int value = Stats[stat];
            int boost = StatsBoosts[stat];
            float[] boostValues = { 1, 1.5f, 2, 2.5f, 3, 3.5f, 4 };

            if (boost >= 0) 
                value = Mathf.FloorToInt(value * boostValues[boost]);
            else
                value = Mathf.FloorToInt(value / boostValues[-boost]);
            
            return value;
        }

        public void ApplyBoost(List<StatBoost> statBoosts)
        {
            foreach (StatBoost statBoost in statBoosts)
            {
                StatsBoosts[statBoost.stat] = Mathf.Clamp(StatsBoosts[statBoost.stat] + statBoost.boost, -6, 6);

                StatusChanges.Enqueue(statBoost.boost > 0
                    ? $"Se ha aumentado {statBoost.stat.ToString().Replace('_', ' ')} a {Base.Name}"
                    : $"Se ha bajado {statBoost.stat.ToString().Replace('_', ' ')} a {Base.Name}");
            }
        }
        
        public int Attack => GetStat(Stat.Ataque);

        public int Defense => GetStat(Stat.Defensa);
        
        public int SpAttack => GetStat(Stat.Ataque_Especial);
        
        public int SpDefense => GetStat(Stat.Defensa_Especial);
        
        public int Speed => GetStat(Stat.Velocidad);
        
        public int MaxHp { get; private set; }

        // Formula real de los juegos de Pokemon del daño
        public DamageDetails TakeDamage(Move move, Pokemon attacker)
        {
            float critical = 1;
            if (Random.value * 100f < 6.25f) critical = 2;
            
            float type = TypeChart.GetEffectiveness(move.Base.Type, this.Base.TypePrimary) *
                         TypeChart.GetEffectiveness(move.Base.Type, this.Base.TypeSecondary);

            DamageDetails damageDetails = new DamageDetails()
            {
                TypeEffectiveness = type,
                Critical = critical,
                Fainted = false
            };

            float attack = move.Base.Category == MoveCat.Especial ? attacker.SpAttack : attacker.Attack;
            float defense = move.Base.Category == MoveCat.Especial ? SpDefense : Defense;
            
            float modifiers = Random.Range(0.85f, 1f) * type * critical;
            float a = (2 * attacker.Level + 10) / 250f;
            float d = a * move.Base.Power * (attack / defense) + 2;
            int damage = Mathf.FloorToInt(d * modifiers);
            
            UpdateHP(damage);
            
            return damageDetails;
        }

        public void UpdateHP(int damage)
        {
            HP = Mathf.Clamp(HP - damage, 0, MaxHp);
            HpChanged = true;
        }
        
        public void SetStatus(ConditionID conditionID)
        {
            if (Status != null) return;
            
            Status = ConditionsDB.Conditions[conditionID];
            Status?.OnStart?.Invoke(this);
            StatusChanges.Enqueue($"{Base.Name} {Status.StartMessage}");
            OnStatusChanged?.Invoke();
        }

        public void CureStatus()
        {
            Status = null;
            OnStatusChanged?.Invoke();
        }
        
        public void SetVolatileStatus(ConditionID conditionID)
        {
            if (VolatileStatus != null) return;
            
            VolatileStatus = ConditionsDB.Conditions[conditionID];
            VolatileStatus?.OnStart?.Invoke(this);
            StatusChanges.Enqueue($"{Base.Name} {VolatileStatus.StartMessage}");
        }
        
        public void CureVolatileStatus()
        {
            VolatileStatus = null;
        }

        public Move GetRandomMove()
        {
            var movesWithPp = Moves.Where(m => m.PP > 0).ToList();
            return movesWithPp[Random.Range(0, movesWithPp.Count)];
        }

        public bool OnBeforeTurn()
        {
            bool canPerformMove = true;

            if (Status?.OnBeforeMove != null)
            {
                if (!Status.OnBeforeMove(this)) canPerformMove = false;
            }

            if (VolatileStatus?.OnBeforeMove != null)
            {
                if (!VolatileStatus.OnBeforeMove(this)) canPerformMove = false;
            }

            return canPerformMove;
        }
        
        public void OnAfterTurn()
        {
            Status?.OnAfterTurn?.Invoke(this);
            VolatileStatus?.OnAfterTurn?.Invoke(this);
        }
        
        public void OnBattleOver()
        {
            VolatileStatus = null;
            ResetStatBoost();
        }
    }
}

public class DamageDetails {

    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }
}