using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NPC {
    public class NPC : MonoBehaviour {

        [Header("Inteligencia artificial")]
        public bool hasAI = true;

        [Header("Velocidad")] public float velocity = 3f;

        [Header("Dialogo")] public List<String> dialog;

        void Start()
        {

        }

        
        void Update()
        {
            // --- Dialog ---
            
            if (!this.hasAI) return;
        }
    }
}
