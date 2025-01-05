using UnityEngine;

namespace Creotly_Studios
{
    public class Minimap : MonoBehaviour
    {
        Vector3 newPosition;
        private Transform playerTransform;

        private void LateUpdate()
        {
            if(playerTransform == null)
            {
                playerTransform = GameManager.Instance.playerManager?.transform;
                return;
            }

            newPosition = playerTransform.transform.position;

            newPosition.y = transform.position.y;
            transform.position = newPosition;
        }
    }
}
