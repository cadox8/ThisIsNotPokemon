using UnityEngine;

namespace Battle {
    public class HPBar : MonoBehaviour {

        [SerializeField] public GameObject health;

        private void Start()
        {
            health.transform.localScale = new Vector3(1f, 1f);
        }

        public void setHP(float hpNormalized)
        {
            health.transform.localScale = new Vector3(hpNormalized, 1f);
        }
    }
}