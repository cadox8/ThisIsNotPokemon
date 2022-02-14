using System;
using System.Collections;
using System.Collections.Generic;
using pokemon;
using UnityEngine;
using UnityEngine.UI;

namespace Battle {
    public class BattleDialogBox : MonoBehaviour {

        [SerializeField] public int lettersPerSeconds;
        [SerializeField] public Color highlightedColor;
        
        [SerializeField] public Text dialogText;
        [SerializeField] public GameObject actionSelector;
        [SerializeField] public GameObject moveSelector;
        [SerializeField] public GameObject moveDetails;

        [SerializeField] public List<Text> actionText;
        [SerializeField] public List<Text> moveText;
        
        [SerializeField] public Text ppText;
        [SerializeField] public Text typeText;

        public void SetDialog(String dialog)
        {
            this.dialogText.text = dialog;
        }

        public IEnumerator TypeDialog(string dialog)
        {
            this.dialogText.text = "";
            foreach (var c in dialog.ToCharArray())
            {
                this.dialogText.text += c;
                yield return new WaitForSeconds(1f / this.lettersPerSeconds);
            }
            yield return new WaitForSeconds(1f);
        }

        public void EnableDialogText(bool enabled)
        {
            this.dialogText.enabled = enabled;
        }
        public void EnableActionsSelector(bool enabled)
        {
            this.actionSelector.SetActive(enabled);
        }
        public void EnableMoveSelector(bool enabled)
        {
            this.moveSelector.SetActive(enabled);
            this.moveDetails.SetActive(enabled);
        }

        public void UpdateActionSelector(int selectedAction)
        {
            for (int i = 0; i < this.actionText.Count; i++)
            {
                if (i == selectedAction)
                {
                    this.actionText[i].color = highlightedColor;
                }
                else
                {
                    this.actionText[i].color = Color.black;
                }
            }
        }

        public void UpdateMoveSelector(int selectedMove, Move move)
        {
            for (int i = 0; i < this.moveText.Count; i++)
            {
                if (i == selectedMove)
                {
                    this.moveText[i].color = highlightedColor;
                }
                else
                {
                    this.moveText[i].color = Color.black;
                }
            }

            ppText.text = $"PP {move.PP}/{move.Base.PP}";
            typeText.text = move.Base.Type.ToString();
            
            if (move.PP == 0) 
                ppText.color = Color.red;
            else
                ppText.color = Color.black;
        }
        
        public void SetMoveNames(List<Move> moves)
        {
            for (int i = 0; i < this.moveText.Count; i++)
            {
                if (i < moves.Count)
                {
                    this.moveText[i].text = moves[i].Base.Name;
                }
                else
                {
                    this.moveText[i].text = "---";
                }
            }
        }
    }
}