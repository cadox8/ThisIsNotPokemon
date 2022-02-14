using System.Collections;
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

        public IEnumerator SetHPSmooth(float newHp)
        {
            float curHp = health.transform.localScale.x;
            float changeAmt = curHp - newHp;

            while (curHp - newHp > Mathf.Epsilon)
            {
                curHp -= changeAmt * Time.deltaTime;
                health.transform.localScale = new Vector3(curHp, 1f);
                yield return null;
            }

            health.transform.localScale = new Vector3(newHp, 1f);
        }
    }
}