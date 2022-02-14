using System;
using System.Collections;
using pokemon;
using UnityEngine;
using UnityEngine.UI;

namespace Battle {
    public class BattleHud : MonoBehaviour {

        [SerializeField] public Text nameText;
        [SerializeField] public Text levelText;
        [SerializeField] public Text statusText;
        [SerializeField] public HPBar HpBar;

        private Pokemon _pokemon;

        public void SetData(Pokemon pokemon)
        {
            _pokemon = pokemon;
            nameText.text = pokemon.Base.Name;
            levelText.text = "Lvl " + pokemon.Level;
            HpBar.setHP((float) pokemon.HP / pokemon.Base.maxHp);
            
            SetStatusText();
            _pokemon.OnStatusChanged += SetStatusText;
        }

        public void SetStatusText()
        {
            if (_pokemon.Status == null) 
                statusText.text = "";
            else
            {
                statusText.text = _pokemon.Status.Id.ToString().ToUpper();
                statusText.color = _pokemon.Status.Color;
            }
        }

        public IEnumerator UpdateHP()
        {
            if (_pokemon.HpChanged)
                yield return HpBar.SetHPSmooth((float) _pokemon.HP / _pokemon.Base.maxHp);

            _pokemon.HpChanged = false;
        }
    }
}