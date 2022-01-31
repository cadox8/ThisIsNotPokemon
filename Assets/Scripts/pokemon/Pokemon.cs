using System;
using System.Collections.Generic;
using UnityEngine;

namespace pokemon {
    public class Pokemon {

        public PokemonBase Base { get; set; }
        public int Level { get; set; }

        public int HP { get; set; }
        
        public List<Move> Moves { get; set; }

        public Pokemon(PokemonBase @base, int level)
        {
            this.Base = @base;
            this.Level = level;

            this.HP = this.Base.maxHp;

            // --- Generate Moves ---
            this.Moves = new List<Move>();
            
            foreach (var move in this.Base.LearnableMoves)
            {
                if (move.level <= this.Level) Moves.Add(new Move(move.moveBase));
                
                if (Moves.Count >= 4) break;
            }
        }
        
        public int Attack()
        {
            return Mathf.FloorToInt((this.Base.attack * Level) / 100f) + 5;
        }
        
        public int Defense()
        {
            return Mathf.FloorToInt((this.Base.defense * Level) / 100f) + 5;
        }
        
        public int SpAttack()
        {
            return Mathf.FloorToInt((this.Base.spAttack * Level) / 100f) + 5;
        }
        
        public int SpDefense()
        {
            return Mathf.FloorToInt((this.Base.spDefense * Level) / 100f) + 5;
        }
        
        public int Speed()
        {
            return Mathf.FloorToInt((this.Base.speed * Level) / 100f) + 5;
        }
        
        public int MaxHp()
        {
            return Mathf.FloorToInt((this.Base.maxHp * Level) / 100f) + 10;
        }
    }
}