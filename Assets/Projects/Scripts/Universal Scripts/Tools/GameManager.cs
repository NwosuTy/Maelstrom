using UnityEngine;

namespace Creotly_Studios
{
    public class GameManager : MonoBehaviour
    {
        public static PlayerManager playerManager;

        private void Awake()
        {
            playerManager = GetComponentInChildren<PlayerManager>();
        }
    }
}
