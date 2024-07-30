
using System.Collections;
using Game.Manager;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Rendering.Universal;

using UnityEngine.Tilemaps;

namespace Game.GameStage1
{
    public class Cat : MonoBehaviour
    {
        public float moveSpeed = 5f;
        public float updateFireValue = 0.2f;
        public float duration = 0.3f;

        public AnimatorController[] animatorControllers; 
        
        private Rigidbody2D rb;
        private Animator animator;
        private Vector2 movement;
        private bool isMoving;
        private Light2D light;
        private float maxInnerPoint = 1;
        private bool stop = false;
        
        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            light = GetComponentInChildren<Light2D>();
        }

        private void Update()
        {
            if (stop)
                return;
            
            // 입력 처리
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            // 이동 상태 확인
            isMoving = movement != Vector2.zero;

            // 애니메이터 파라미터 설정
            if (!isMoving)
                animator.runtimeAnimatorController = animatorControllers[0];
            else
                animator.runtimeAnimatorController = animatorControllers[1];

            // 방향 설정 (좌우 방향만 고려할 경우)
            if (movement.x != 0)
            {
                transform.localScale = new Vector3(Mathf.Sign(movement.x), 1, 1);
            }
        }

        private void FixedUpdate()
        {
            if (stop)
                return;
            
            // 이동 적용
            rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
        }
        
        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.transform.CompareTag("item"))
            {
                Tilemap itemTilemap = other.GetComponent<Tilemap>();
                // 충돌 지점을 가져옵니다.
                Vector3 hitPosition = other.ClosestPoint(transform.position);
                // 충돌 지점을 타일맵의 셀 좌표로 변환합니다.
                Vector3Int cellPosition = itemTilemap.WorldToCell(hitPosition);
                // 해당 셀 위치의 TileBase를 가져옵니다.
                TileBase tile = itemTilemap.GetTile(cellPosition);
            
                if (tile != null)
                {
                    if (tile.name == "fire")
                    {
                        // 타일 제거
                        StartCoroutine(UpdateFire(duration));
                        itemTilemap.SetTile(cellPosition, null);    
                    }
                    else if (tile.name == "escape")
                    {
                        itemTilemap.SetTile(cellPosition, null);
                        Debug.Log("탈출");
                        SceneManager.LoadScene(SceneManager.SceneType.Stage1, SceneManager.SceneType.Lobby);
                    }
                }    
            }
        }

        private IEnumerator UpdateFire(float duration)
        {
            float startInnerPoint = light.pointLightInnerRadius;
            float startOuterPoint = light.pointLightOuterRadius;

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                float newInnerPoint = Mathf.Lerp(startInnerPoint, startInnerPoint + updateFireValue, elapsedTime / duration);
                newInnerPoint = newInnerPoint > maxInnerPoint ? maxInnerPoint : newInnerPoint;
                
                light.pointLightInnerRadius = newInnerPoint;
                light.pointLightOuterRadius = Mathf.Lerp(startOuterPoint, startOuterPoint + updateFireValue, elapsedTime / duration);

                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }    
}