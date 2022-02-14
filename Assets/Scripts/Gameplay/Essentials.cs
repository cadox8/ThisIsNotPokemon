using System;
using UnityEngine;

namespace Gameplay {
    public class Essentials : MonoBehaviour {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}