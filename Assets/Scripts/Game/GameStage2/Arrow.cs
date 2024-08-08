/*
 * 플레이어가 발사하는 객체처리
 */

using UnityEngine;

namespace Game.GameStage2
{
    public class Arrow : MonoBehaviour
    {
        public int demage = 50; // 화살 데미지
        
        private Vector2 direction;  // 화살 방향
        private float speed;    // 화살 속도
        private float lifetime; // 화살 최대 생존 시간
        private float distanceTraveled = 0f;    // 화살이 이동한 거리
        private bool hasHit = false; // 화살이 이미 타격했는지 여부를 추적

        /// <summary>
        /// Arrow 발사시 호출
        /// </summary>
        /// <param name="d">direction</param>
        /// <param name="s">speed</param>
        /// <param name="lt">lifetime</param>
        public void Setup(Vector2 d, float s, float lt)
        {
            this.direction = d.normalized;;
            this.speed = s;
            this.lifetime = lt;

            // 화살 회전
            float angle = Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

            // 최대 생존 시간 후 파괴
            Destroy(gameObject, lt);
        }

        private void Update()
        {
            if (hasHit || Stage2Scene.IsStop)
                return;
            
            Vector3 movement = (Vector3)direction * speed * Time.deltaTime;
            transform.position += movement;
            distanceTraveled += movement.magnitude;

            if (distanceTraveled >= speed * lifetime)
            {
                Destroy(gameObject);
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (hasHit)
                return;
            
            if (other.CompareTag("Monster"))
            {
                Monster monster = other.GetComponent<Monster>();
                if (monster != null)
                {
                    monster.TakeDamage(demage);
                    hasHit = true;
                    Destroy(gameObject);
                }
            }
        }
        
        // 디버그용.
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, transform.up * 2f);
        }
    }
}