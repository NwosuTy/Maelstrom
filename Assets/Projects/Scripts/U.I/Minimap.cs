using UnityEngine;

namespace Creotly_Studios
{
    public class Minimap : MonoBehaviour
    {
        Vector3 newPosition;
        private Transform playerTransform;

        private void Awake()
        {
            playerTransform = GameManager.playerManager.transform;
        }

        private void LateUpdate()
        {
            newPosition = playerTransform.transform.position;

            newPosition.y = transform.position.y;
            transform.position = newPosition;
        }
    }
}
