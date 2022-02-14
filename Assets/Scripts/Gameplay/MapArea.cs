using System.Collections.Generic;
using pokemon;
using UnityEngine;

namespace Gameplay {
    public class MapArea : MonoBehaviour {

        [SerializeField] public List<Pokemon> wildPokemons;

        public Pokemon GetRandomWildPokemon()
        {
            Pokemon pokemon = wildPokemons[Random.Range(0, wildPokemons.Count)];
            pokemon.Init();
            return pokemon;
        }
    }
}