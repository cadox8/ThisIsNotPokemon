using pokemon;
using UnityEngine;
using UnityEngine.UI;

namespace Battle {
    public class BattleHud : MonoBehaviour {

        [SerializeField] public Text nameText;
        [SerializeField] public Text levelText;
        [SerializeField] public HPBar HpBar;

        public void SetData(Pokemon pokemon)
        {
            this.nameText.text = pokemon.Base.Name;
            this.levelText.text = "Lvl " + pokemon.Level;
            this.HpBar.setHP((float) pokemon.HP / pokemon.Base.maxHp);
            
            Debug.Log(pokemon.Base.name + " " + (float) pokemon.HP / pokemon.Base.maxHp);
        }
    }
}