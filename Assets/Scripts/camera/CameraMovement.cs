using UnityEngine;

namespace camera {
    public class CameraMovement : MonoBehaviour {

        [Header("Juagor")] public GameObject player;


        void Update()
        {
            Vector3 playerPosition = player.transform.position;
            transform.position = new Vector3(playerPosition.x, playerPosition.y, -10);
        }
    }
}