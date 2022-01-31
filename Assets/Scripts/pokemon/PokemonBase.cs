using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace pokemon {
    [CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create new pokemon", order = 0)]
    public class PokemonBase : ScriptableObject {

        [SerializeField] private string name;

        [TextArea] [SerializeField] public string description;

        [SerializeField] public Sprite frontSprite;
        [SerializeField] public Sprite backSprite;

        [SerializeField] public PokemonType typePrimary;
        [SerializeField] public PokemonType typeSecondary;

        // --- Basic Stats ---
        [SerializeField] public int maxHp;
        [SerializeField] public int attack;
        [SerializeField] public int defense;
        [SerializeField] public int spAttack;
        [SerializeField] public int spDefense;
        [SerializeField] public int speed;

        [SerializeField] public List<LearnableMove> learnableMoves;

        // --- Getters ---
        public string Name => name;

        public string Description => description;

        public Sprite FrontSprite => frontSprite;

        public Sprite BackSprite => backSprite;

        public PokemonType TypePrimary => typePrimary;

        public PokemonType TypeSecondary => typeSecondary;

        public int MaxHp => maxHp;

        public int Attack => attack;

        public int Defense => defense;

        public int SpAttack => spAttack;

        public int SpDefense => spDefense;

        public int Speed => speed;

        public List<LearnableMove> LearnableMoves => learnableMoves;
    }

    [System.Serializable]
    public class LearnableMove {

        [SerializeField] public MoveBase moveBase;
        [SerializeField] public int level;

        public MoveBase MoveBase => moveBase;

        public int Level => level;
    }
    
    
    public enum PokemonType {
        Normal, Fire, Water, Electric, Grass, Ice, Fighting, Poison, Ground, Flying, Psychic, Bug, Rock, Ghost, Dragon, None
    }
}