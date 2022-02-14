using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace pokemon {
    public class PokemonParty : MonoBehaviour {

        [SerializeField] public List<Pokemon> pokemons;

        private void Start()
        {
            foreach (var pokemon in pokemons)
            {
                pokemon.Init();
            }
        }

        public Pokemon GetHealthyPokemon()
        {
            return pokemons.FirstOrDefault(pokemon => pokemon.HP > 0);
        }

        public void AddPokemon(Pokemon pokemon)
        {
            if (pokemons.Count < 6)
                pokemons.Add(pokemon);
        }
    }
}