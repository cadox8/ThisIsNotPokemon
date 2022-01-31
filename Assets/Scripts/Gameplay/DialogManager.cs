using UnityEngine;
using UnityEngine.UI;

namespace Gameplay {
    public class DialogManager : MonoBehaviour {

        [SerializeField] public GameObject dialogBox;
        [SerializeField] public Text dialogText;

        public void ShowDialog(Dialog dialog)
        {
            this.dialogBox.SetActive(true);
            this.dialogText.text = dialog.lines[0];
        }
    }
}