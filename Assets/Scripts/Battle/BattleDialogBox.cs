using System;
using UnityEngine;
using UnityEngine.UI;

namespace Battle {
    public class BattleDialogBox : MonoBehaviour {

        [SerializeField] public Text dialogText;

        public void SetDialog(String dialog)
        {
            this.dialogText.text = dialog;
        }
    }
}