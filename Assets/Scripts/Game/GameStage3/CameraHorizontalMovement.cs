using UnityEngine;

namespace Game.GameStage3
{
    public class CameraHorizontalMovement : MonoBehaviour
    {
        [SerializeField]
        private float moveSpeed = 5f;
        [SerializeField]
        private float minX = -10f;
        [SerializeField]
        private float maxX = 10f;

        private void Update()
        {
            float horizontalInput = Input.GetAxis("Horizontal");
        
            if (horizontalInput != 0)
            {
                Vector3 newPosition = transform.position + Vector3.right * (horizontalInput * moveSpeed * Time.deltaTime);
            
                newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            
                transform.position = newPosition;
            }
        }
    }
}