using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay {
    
    [Serializable]
    public class Dialog : MonoBehaviour {
        [SerializeField] private List<string> lines;
        public List<string> Lines => lines;
    }
}