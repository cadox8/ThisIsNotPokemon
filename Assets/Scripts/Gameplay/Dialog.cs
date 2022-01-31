using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay {
    
    [System.Serializable]
    public class Dialog : MonoBehaviour {

        [SerializeField] public List<string> lines;
        
    }
}