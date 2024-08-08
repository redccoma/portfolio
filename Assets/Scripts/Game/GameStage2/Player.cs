using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GameStage2
{
    public class Player : MonoBehaviour
    {
        public GameObject arrowPrefab;  // 화살 프리팹
        public float arrowSpeed = 10f;  // 화살 속도
        public float shootInterval = 1f; // 자동 발사 간격
        public float arrowLifetime = 2f; // fire후 화살이 사라질 시간
        public float moveSpeed = 20f;   // 캐릭터 이동속도
        public int health = 100;    // 캐릭터 체력
        
        public Action<int, int> OnHealthChanged;    // 체력이 변경될때 호출할 이벤트 (최대체력, 현재체력)
        
        private float invincibilityTime = 0.7f; // 몬스터에게 히트된 후 다음 히트까지의 시간
        private Rigidbody2D rb;    // 캐릭터 물리적인 움직임을 처리하기 위한 변수
        private Vector2 movement;   // 캐릭터 이동처리 변수
        private bool isMoving;      // 캐릭터의 이동중 여부
        private bool isInvincible;  // 몬스터에게 히트된 경우 true
        private HashSet<Monster> collidingMonsters = new HashSet<Monster>();
        private ContactFilter2D hitCheckfilter; // 몬스터와 충돌체크를 위한 필터
        private float damageInterval = 0.5f; // 데미지를 주는 간격
        private float lastDamageTime;   // 마지막 데미지를 준 시간
        private Collider2D mCollider;
        private int maxHealth;  // 지정된 최대체력 저장용
        private bool init;  // 초기화 여부
        private Action playerDead;  // 플레이어가 죽었을때 호출할 콜백

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            mCollider = GetComponent<CircleCollider2D>();
            maxHealth = health;

            hitCheckfilter = new ContactFilter2D();
            hitCheckfilter.SetLayerMask(LayerMask.GetMask("Monster"));
            
            init = false;
        }
        
        public void Init(Action deadCallback)
        {
            playerDead = deadCallback;
            
            StartCoroutine(AutoShoot());
            OnHealthChanged?.Invoke(maxHealth, health);
            init = true;
        }

        private void Update()
        {
            if (Stage2Scene.IsStop || !init)
                return;
            
            // 입력 처리
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            // 이동 상태 확인
            isMoving = movement != Vector2.zero;

            // 방향 설정 (좌우 방향만 고려할 경우)
            if (movement.x != 0)
            {
                // x 스케일을 1, 혹은 -1처리해서 이미지 좌우 반전 처리
                transform.localScale = new Vector3(Mathf.Sign(movement.x), 1, 1);
            }
            
            // 일정 간격으로 누적 데미지 적용
            if (Time.time - lastDamageTime >= damageInterval)
            {
                ApplyCumulativeDamage();
                lastDamageTime = Time.time;
            }
        }

        private void FixedUpdate()
        {
            if (Stage2Scene.IsStop || !init)
                return;
            
            // 캐릭터 이동
            rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);

            HandleCollisions();
        }
        
        // 피격 데미지 처리
        private void ApplyCumulativeDamage()
        {
            if (collidingMonsters.Count > 0 && !isInvincible)
            {
                int totalDamage = 0;
                foreach (Monster monster in collidingMonsters)
                {
                    totalDamage += monster.Damage;
                }

                TakeDamage(totalDamage);
                StartCoroutine(InvincibilityCoroutine());
            }
        }
        
        private IEnumerator AutoShoot()
        {
            while (true)
            {
                if (!Stage2Scene.IsStop)
                {
                    ShootArrow();   
                }
                
                yield return new WaitForSeconds(shootInterval);
            }
        }
        
        private void HandleCollisions()
        {
            Collider2D[] colliders = new Collider2D[10];    // 최대 얻어올 몬스터의 갯수는 10개.
            int colliderCount = Physics2D.OverlapCollider(mCollider, hitCheckfilter, colliders);

            collidingMonsters.Clear();
            for (int i = 0; i < colliderCount; i++)
            {
                Monster monster = colliders[i].GetComponent<Monster>();
                if (monster != null)
                {
                    collidingMonsters.Add(monster);
                }
            }

            ApplyCumulativeDamage();
        }
        
        private void ShootArrow()
        {
            Monster nearestMonster = FindNearestMonster();
            if (nearestMonster != null)
            {
                Vector2 direction = (nearestMonster.transform.position - transform.position).normalized;
                GameObject arrowObj = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
                Arrow arrow = arrowObj.GetComponent<Arrow>();
                if (arrow != null)
                {
                    arrow.Setup(direction, arrowSpeed, arrowLifetime);
                    
#if UNITY_EDITOR
                    // 디버그 라인 추가
                    Debug.DrawLine(transform.position, nearestMonster.transform.position, Color.red, arrowLifetime);
                    Debug.DrawRay(transform.position, direction * 5f, Color.blue, arrowLifetime);
#endif
                }
            }
        }

        // 플레이어 기준 가장 가까운 몬스터를 찾기위한 함수
        private Monster FindNearestMonster()
        {
            Monster[] monsters = Stage2Scene.GetActiveMonsters();
            Monster nearest = null;
            float minDistance = Mathf.Infinity;

            foreach (Monster monster in monsters)
            {
                float distance = Vector2.Distance(transform.position, monster.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = monster;
                }
            }

            return nearest;
        }
        
        private void TakeDamage(int damage)
        {
            health -= damage;
            
            OnHealthChanged?.Invoke(maxHealth, health);
        
            if (health <= 0)
            {
                Die();
            }
        }
        
        private void Die()
        {
            playerDead?.Invoke();
        }

        // 연속으로 피격되는 경우를 방지하기 위한 코루틴
        private IEnumerator InvincibilityCoroutine()
        {
            isInvincible = true;
            yield return new WaitForSeconds(invincibilityTime);
            isInvincible = false;
        }
    }    
}