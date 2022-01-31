using pokemon;
using UnityEngine;
using UnityEngine.UI;

namespace Battle {
    public class BattleUnit : MonoBehaviour {

        [SerializeField] private PokemonBase _base;
        [SerializeField] public int level;

        [SerializeField] public bool isPlayerUnit;
        public Pokemon Pokemon { get; set; }
        
        public void Setup()
        {
            this.Pokemon = new Pokemon(_base, level);
            if (this.isPlayerUnit)
            {
                GetComponent<Image>().sprite = Pokemon.Base.BackSprite;
            }
            else
            {
                GetComponent<Image>().sprite = Pokemon.Base.FrontSprite;
            }
        }
    }
}