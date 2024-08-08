/*
 * 몬스터 이동, 파괴 처리 담당.
 */

using System;
using System.Collections;
using UnityEngine;

namespace Game.GameStage2
{
    public class Monster : MonoBehaviour
    {
        public float moveSpeed = 10f;
        public int health = 100;
        
        private Transform target;   // 플레이어
        private Rigidbody2D rb;
        private bool isMoving = false;  // 이동중 여부
        private bool canAttack = true;  // 공격 가능 여부
        private int attackDamage = 10;  // 공격 데미지
        private Coroutine coroutine;
        private Action<int> OnHitCallback;  // 피격시 호출할 콜백
        
        public int Damage => attackDamage;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }
        
        private void Die()
        {
            ClearObject();
        }
        
        private IEnumerator Move()
        {
            WaitForFixedUpdate wffu = new WaitForFixedUpdate();
            
            while (true)
            {
                if (target != null)
                {
                    Vector2 direction = (target.position - transform.position).normalized;
                    rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
                }

                yield return wffu;
            }
        }
        
        public void Setup(Transform player, Action<int> callback)
        {
            target = player;
            OnHitCallback = callback;
            coroutine = StartCoroutine(Move());
        }
        
        public void TakeDamage(int damage)
        {
            health -= damage;
            
            OnHitCallback?.Invoke(health);
            
            if (health <= 0)
            {
                Die();
            }
        }
        
        public void ClearObject()
        {
            StopCoroutine(coroutine);
            Destroy(gameObject);
        }
    }
}