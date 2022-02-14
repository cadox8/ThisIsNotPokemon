using System;
using System.Collections.Generic;
using Data;
using UnityEngine;

namespace pokemon {
    
    [CreateAssetMenu(fileName = "Move", menuName = "Pokemon/Create new move", order = 1)]
    public class MoveBase : ScriptableObject {
        
        [SerializeField] private string name;

        [TextArea] [SerializeField] private string description;
        
        [SerializeField] private PokemonType type;
        [SerializeField] private MoveCat category;
        [SerializeField] private MoveEffects effects;
        [SerializeField] private List<SecondaryEffects> secondaryEffects;
        [SerializeField] private MoveTarget target;
        [SerializeField] private int power;
        [SerializeField] private int accuracy;
        [SerializeField] private int pp;
        [SerializeField] private int priority;

        public string Name => name;

        public string Description => description;

        public PokemonType Type => type;

        public MoveCat Category => category;

        public int Power => power;

        public int Accuracy => accuracy;

        public int PP => pp;

        public int Priority => priority;

        public MoveEffects Effects => effects;

        public MoveTarget Target => target;

        public List<SecondaryEffects> SecondaryEffects => secondaryEffects;
    }

    [Serializable]
    public class MoveEffects {

        [SerializeField] private List<StatBoost> boosts;
        [SerializeField] private ConditionID status;
        [SerializeField] private ConditionID volatileStatus;

        public List<StatBoost> Boosts => boosts;
        public ConditionID Status => status;
        public ConditionID VolatileStatus => volatileStatus;
    }
    
    [Serializable]
    public class SecondaryEffects : MoveEffects {

        [SerializeField] private int chance;
        [SerializeField] private MoveTarget target;

        public int Chance => chance;
        public MoveTarget Target => target;
    }

    [Serializable]
    public class StatBoost {
        public Stat stat;
        public int boost;
    }

    public enum MoveCat {
        Físico, Especial, Estado
    }

    public enum MoveTarget {
        Foe, Self
    }
}