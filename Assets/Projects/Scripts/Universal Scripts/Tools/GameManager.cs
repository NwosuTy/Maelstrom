using UnityEngine;

namespace Creotly_Studios
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public PlayerManager playerManager;

        private void Awake()
        {
            if(Instance != null)
            {
                Destroy(gameObject);
            }
            Instance = this;
        }

        public void SetPlayerManager(PlayerManager player)
        {
            playerManager = player;
        }
    }
}
