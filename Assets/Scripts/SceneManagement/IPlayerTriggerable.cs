using Player;
using UnityEngine;

namespace SceneManagement {
    public interface IPlayerTriggerable {
        void OnPlayerTriggered(PlayerController player);
    }
}