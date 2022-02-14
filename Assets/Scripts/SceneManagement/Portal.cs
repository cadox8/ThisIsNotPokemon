using System;
using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneManagement {
    public class Portal : MonoBehaviour, IPlayerTriggerable {

        [SerializeField] private int scene;
        
        public void OnPlayerTriggered(PlayerController player)
        {
            StartCoroutine(SwitchScene());
        }

        private IEnumerator SwitchScene()
        {
            yield return SceneManager.LoadSceneAsync(scene);
        }
    }
}