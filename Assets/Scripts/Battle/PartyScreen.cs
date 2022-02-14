using System.Collections.Generic;
using pokemon;
using UnityEngine;
using UnityEngine.UI;

namespace Battle {
    public class PartyScreen : MonoBehaviour {

        [SerializeField] public Text messageText;
        
        private PartyMemberUI[] memberSlots;
        private List<Pokemon> _pokemons;

        public void Init()
        {
            memberSlots = GetComponentsInChildren<PartyMemberUI>();
        }

        public void SetPartyData(List<Pokemon> pokemons)
        {
            _pokemons = pokemons;
            for (int i = 0; i < memberSlots.Length; i++)
            {
                if (i < pokemons.Count) 
                    memberSlots[i].SetData(pokemons[i]);
                else
                    memberSlots[i].gameObject.SetActive(false);
            }

            messageText.text = "Elige un pokemon";
        }

        public void UpdateMemberSelection(int selectedMember)
        {
            for (int i = 0; i < _pokemons.Count; i++)
            {
                if (i == selectedMember)
                    memberSlots[i].SetSelected(true);
                else
                    memberSlots[i].SetSelected(false);
            }
        }

        public void SetMessageText(string message)
        {
            messageText.text = message;
        }
    }
}