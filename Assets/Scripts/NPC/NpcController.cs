using Gameplay;
using UnityEngine;

namespace NPC {
    public class NpcController : MonoBehaviour, Interactable {

        [SerializeField] private Dialog dialog;

        public void Interact()
        {
            StartCoroutine(DialogManager.Instance.ShowDialog(dialog));
        }
    }
}
