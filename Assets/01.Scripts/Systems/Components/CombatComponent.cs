using Game.Models;
using UnityEngine;
using Zenject;
using Game.Core;
using static UnityEngine.GraphicsBuffer;
using System.Collections.Generic;
using R3;
using System;
namespace Game.Systems
{
    /// <summary>
    /// 전투를 하기 위한 기본 스텟(공격력, 방어력)을 등록 하는 컴포넌트 + 데미지 처리
    /// </summary>
    public class CombatComponent : MonoBehaviour,  IAnimAttackReceiver{
        [SerializeField] private CombatData _combatData;
        [Inject] private readonly CombatSystem _combatSystem;

        // 실제 스트림
        private readonly Subject<Unit> _attackStart = new();
        private readonly Subject<Unit> _attackEnd = new();
        
        // 외부 노출
        public Observable<Unit> OnAttackStart => _attackStart;
        public Observable<Unit> OnAttackEnd => _attackEnd;

       
        public float AttackSpeed { get; private set; }

        private CompositeDisposable _disposables = new();
       
        


        #region 초기화
        private void OnDestroy() {
            _disposables?.Dispose();
            // 해제
            _combatSystem.UnregisterCombatCharacter(gameObject.GetInstanceID());
            
        }

        private void Awake() {
            // 등록
            _combatSystem.RegisterCombatCharacter(gameObject.GetInstanceID(), _combatData.FinalAttack, _combatData.FinalDefense, _combatData.attackSpeedMultiplier);
            var RORP = _combatSystem.GetRORP_CombatData(gameObject.GetInstanceID());
            RORP.Subscribe((combatData) => { AttackSpeed = combatData.FinalAttackSpeed;
            })
                .AddTo(_disposables);
        }
        #endregion
  

        #region AnimSender에서 전달 받는 영역
        public void OnAttackStartEvent() {
            _attackStart.OnNext(Unit.Default);
        }

        public void OnAttackEndEvent() {
            _attackEnd.OnNext(Unit.Default);
        }

        #endregion
    }
}
