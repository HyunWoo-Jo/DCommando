using UnityEngine;
using Zenject;
using Game.Core;
using Game.Core.Event;
using System.Collections;
namespace Game.Systems
{
    /// <summary>
    /// Enemy 컴포넌트 - Enemy 개체 관리
    /// </summary>
    public class EnemyComponent : MonoBehaviour
    {
        [SerializeField] private int enemyId;
        [SerializeField] private int expReward = 10;
        [SerializeField] private int goldReward = 5;
        
        private HealthComponent _healthComponent;
        private bool _isDead = false;
        
        private void Awake()
        {
            _healthComponent = GetComponent<HealthComponent>();
            if (_healthComponent == null)
            {
                GameDebug.LogError($"Enemy {gameObject.name}에 HealthComponent가 없음");
                return;
            }
            
            // 체력이 0이 되면 처치 처리
            _healthComponent.OnDeath += OnDeath;
        }
        
        private void OnDestroy()
        {
            if (_healthComponent != null)
            {
                _healthComponent.OnDeath -= OnDeath;
            }
        }

        /// <summary>
        /// Enemy 초기화
        /// </summary>
        public void Initialize(int id, int exp, int gold)
        {
            enemyId = id;
            expReward = exp;
            goldReward = gold;
        }
        
        /// <summary>
        /// Enemy 사망 처리
        /// </summary>
        private void OnDeath()
        {
            if (_isDead) return;
            _isDead = true;
            
            GameDebug.Log($"Enemy {enemyId} 처치됨. 보상: {expReward} EXP, {goldReward} Gold");
            
            
            // 사망 애니메이션이나 이펙트 재생 후 제거
            StartCoroutine(DeathSequence());
        }
        
        /// <summary>
        /// 사망 연출 후 제거
        /// </summary>
        private IEnumerator DeathSequence()
        {
            // 사망 애니메이션 재생
            var animator = GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("Death");
                yield return new WaitForSeconds(1f);
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
            // Enemy 처치 이벤트 발행
            EventBus.Publish(new EnemyDefeatedEvent(enemyId, expReward, goldReward));

            // 경험치 획득 이벤트 발행
            EventBus.Publish(new ExpRewardEvent(expReward));

            // 골드 획득 이벤트 발행  
            EventBus.Publish(new GoldGainedEvent(goldReward));

            // 오브젝트 제거
            Destroy(gameObject);
        }
        
        /// <summary>
        /// Enemy ID 반환
        /// </summary>
        public int GetEnemyId()
        {
            return enemyId;
        }
        
        /// <summary>
        /// 보상 정보 반환
        /// </summary>
        public (int exp, int gold) GetRewards()
        {
            return (expReward, goldReward);
        }
    }
}