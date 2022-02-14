using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay {
    public class DialogManager : MonoBehaviour {

        [SerializeField] public GameObject dialogBox;
        [SerializeField] public Text dialogText;
        [SerializeField] private int lettersPerSeconds;

        private Dialog dialog;
        private int currentLine = 0;
        private bool isTyping;
        
        public event Action OnShowDialog;
        public event Action OnCloseDialog;
        

        public static DialogManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public IEnumerator ShowDialog(Dialog dialog)
        {
            Debug.Log("show");
            yield return new WaitForEndOfFrame();
            
            OnShowDialog?.Invoke();

            this.dialog = dialog;
            dialogBox.SetActive(true);
            StartCoroutine(TypeDialog(dialog.Lines[0]));
        }

        public void HandleUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Z) && !isTyping)
            {
                ++currentLine;
                
                if (currentLine < dialog.Lines.Count)
                    StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
                else
                {
                    currentLine = 0;
                    dialogBox.SetActive(false);
                    OnCloseDialog?.Invoke();
                }
            }
        }
        
        public IEnumerator TypeDialog(string line)
        {
            isTyping = true;
            dialogText.text = "";
            foreach (var c in line.ToCharArray())
            {
                dialogText.text += c;
                yield return new WaitForSeconds(1f / this.lettersPerSeconds);
            }
            isTyping = false;
        }
    }
}