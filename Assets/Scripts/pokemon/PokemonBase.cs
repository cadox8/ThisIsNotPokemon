using System;
using System.Collections.Generic;
using pokemon;
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
        Normal, Fuego, Agua, Eléctrico, Planta, Hielo, Lucha, Veneno, Tierra, Volador, Psíquico, Bicho, Roca, Fantasma, Dragón, Nada
    }

    public enum Stat {
        Ataque, Defensa, Ataque_Especial, Defensa_Especial, Velocidad, Precision, Evasion
    }
}

public class TypeChart {

    // Orden = PokemonType
    public static float[][] chart =
    {
        new float[] {1f,    1f,     1f,     1,      1,      1,      1,      1},
        new float[] {1f,    .5f,    .5f,    1,      2,      2,      1,      1},
        new float[] {1f,    2f,     .5f,    2,      .5f,    1,      1,      1},
        new float[] {1f,    1f,     2,      .5f,    .5f,    2,      1,      1},
        new float[] {1f,    .5f,    2,      2,      .5f,    1,      1,      .5f},
        new float[] {1f,    1,      1,      1,      1,      1,      1,      1}
    };

    public static float GetEffectiveness(PokemonType attackType, PokemonType defenseType)
    {
        if (attackType == PokemonType.Nada || defenseType == PokemonType.Nada) return 1;
        return chart[(int) attackType][(int) defenseType];
    }
}