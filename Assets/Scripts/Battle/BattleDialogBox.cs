using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Battle {
    public class BattleDialogBox : MonoBehaviour {

        [SerializeField] public Text dialogText;
        [SerializeField] public int lettersPerSeconds;

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
        }
    }
}