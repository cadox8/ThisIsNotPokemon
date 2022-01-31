using System;
using UnityEngine;

namespace pokemon {
    [CreateAssetMenu(fileName = "Move", menuName = "Pokemon/Create new move", order = 1)]
    public class MoveBase : ScriptableObject {
        
        [SerializeField] private string name;

        [TextArea] [SerializeField] private string description;
        
        [SerializeField] private PokemonType type;
        [SerializeField] private MoveCat category;
        [SerializeField] private int power;
        [SerializeField] private int accuracy;
        [SerializeField] private int pp;

        public string Name => name;

        public string Description => description;

        public PokemonType Type => type;

        public MoveCat Category => category;

        public int Power => power;

        public int Accuracy => accuracy;

        public int Pp => pp;
    }

    public enum MoveCat {
        Physic, Other, Special
    }
}