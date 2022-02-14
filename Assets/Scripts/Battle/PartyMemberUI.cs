using pokemon;
using UnityEngine;
using UnityEngine.UI;

namespace Battle {
    public class PartyMemberUI : MonoBehaviour {
        
        [SerializeField] public Text nameText;
        [SerializeField] public Text levelText;
        [SerializeField] public HPBar HpBar;

        [SerializeField] public Color highlightedColor;
        
        private Pokemon _pokemon;

        public void SetData(Pokemon pokemon)
        {
            _pokemon = pokemon;
            this.nameText.text = pokemon.Base.Name;
            this.levelText.text = "Lvl " + pokemon.Level;
            this.HpBar.setHP((float) pokemon.HP / pokemon.Base.maxHp);
        }

        public void SetSelected(bool selected)
        {
            if (selected)
                nameText.color = highlightedColor;
            else
                nameText.color = Color.black;
        }
    }
}