using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;
using Game.Core.Event;
using System.Linq;
using Game.Core;
using System;
using R3;
using DG.Tweening;
using UnityEngine.Assertions;

using Random = UnityEngine.Random;
namespace Game.Systems
{
    /// <summary>
    /// Object Pool로 Exp와 Gold드롭을 관리하는 시스템
    /// </summary>

    // 빠르게 등록되도록 실행
    [DefaultExecutionOrder(0)]
    public class DropSystem : MonoBehaviour {
        private struct AmountGameObject {
            public GameObject obj;
            public int amount;

            public AmountGameObject(GameObject obj, int amount) {
                this.obj = obj;
                this.amount = amount;
            }
        }
        private class DropObjectPool {
          

            private GameObject _prefab;
            private ObjectPool<GameObject> _pool;
            private List<AmountGameObject> _liveAmountList;
            private int _defaultCapacity;
            private int _maxSize;

            public int ActiveCount => _pool?.CountActive ?? 0;
            public int InactiveCount => _pool?.CountInactive ?? 0;
            public List<AmountGameObject> LiveObjects => _liveAmountList;

            // 생성자
            public DropObjectPool(GameObject prefab, int defaultCapacity = 20, int maxSize = 100) {
                _prefab = prefab;
                _defaultCapacity = defaultCapacity;
                _maxSize = maxSize;
                _liveAmountList = new List<AmountGameObject>();
                Initialize();
            }

            private void Initialize() {
                _pool = new ObjectPool<GameObject>(
                    createFunc: () => {
                        GameObject obj = Instantiate(_prefab);
                        obj.SetActive(false);
                        return obj;
                    },
                    actionOnGet: (obj) => {
                        obj.SetActive(true);
                    },
                    actionOnRelease: (obj) => {
                        obj.SetActive(false);
                        RemoveFromLiveList(obj);
                    },
                    actionOnDestroy: (obj) => {
                        if (obj != null)
                            Destroy(obj);
                    },
                    collectionCheck: true,
                    defaultCapacity: _defaultCapacity,
                    maxSize: _maxSize
                );

            }

            // 오브젝트 스폰
            public GameObject Spawn(Vector3 position, int amount = 0) {
                if (_pool.CountActive >= _maxSize) {
                    GameDebug.LogWarning($"풀 최대 용량 초과 {_prefab.name} - Active: {_pool.CountActive}");
                    return null;
                }

                GameObject obj = _pool.Get();
                obj.transform.position = position;

                // 리스트에 추가
                _liveAmountList.Add(new AmountGameObject(obj, amount));

                return obj;
            }

            // Try 패턴 스폰
            public bool TrySpawn(Vector3 position, int amount, out GameObject obj) {
                obj = Spawn(position, amount);
                return obj != null;
            }

            // 오브젝트 반환
            public void Return(GameObject obj) {
                if (obj != null && IsLiveObject(obj)) {
                    _pool.Release(obj);
                }
            }

            // 모든 활성 오브젝트 반환
            public void ReturnAll() {
                for (int i = _liveAmountList.Count - 1; i >= 0; i--) {
                    _liveAmountList[i].obj.transform.DOKill();
                    _pool.Release(_liveAmountList[i].obj);
                }
            }

            // Amount 업데이트
            public void UpdateAmount(GameObject obj, int newAmount) {
                for (int i = 0; i < _liveAmountList.Count; i++) {
                    if (_liveAmountList[i].obj == obj) {
                        var item = _liveAmountList[i];
                        item.amount = newAmount;
                        _liveAmountList[i] = item;
                        break;
                    }
                }
            }

            // Amount 가져오기
            public int GetAmount(GameObject obj) {
                var item = _liveAmountList.FirstOrDefault(x => x.obj == obj);
                return item.obj != null ? item.amount : 0;
            }

            // 오래된 오브젝트 자동 회수
            public void RecycleOldest(int keepCount) {
                if (_liveAmountList.Count > keepCount) {
                    int recycleCount = _liveAmountList.Count - keepCount;
                    for (int i = 0; i < recycleCount; i++) {
                        if (_liveAmountList.Count > 0) {
                            _pool.Release(_liveAmountList[0].obj);
                        }
                    }
                    GameDebug.Log($"{_prefab.name} {recycleCount}개 자동 회수");
                }
            }

            // 활성 오브젝트인지 확인
            private bool IsLiveObject(GameObject obj) {
                return _liveAmountList.Any(x => x.obj == obj);
            }

            // 리스트에서 제거
            private void RemoveFromLiveList(GameObject obj) {
                _liveAmountList.RemoveAll(x => x.obj == obj);
            }

            // 풀 상태 로그
            public void LogStatus() {
                GameDebug.Log($"{_prefab.name} 풀 - 활성: {_pool.CountActive}, 비활성: {_pool.CountInactive}, 전체: {_pool.CountAll}");
            }

            // 정리
            public void Dispose() {
                ReturnAll();
                _pool?.Dispose();
                _liveAmountList?.Clear();

            }

        }


        [SerializeField] private GameObject _goldPrefab;
        [SerializeField] private GameObject _expPrefab;

        private DropObjectPool _goldPool;
        private DropObjectPool _expPool;

        private Transform _goldUiTr;
        private Transform _expUiTr;

        private IDisposable _initDisposable;
        private CompositeDisposable _disposables = new();

        private const float _MOVE_DURATION = 0.4f;
        private const float _RAND_RANGE = 0.1f;

        private void Awake() {
#if UNITY_EDITOR
            Assert.IsNotNull( _goldPrefab );
            Assert.IsNotNull( _expPrefab );
#endif

            // Gold 풀 생성
            _goldPool = new DropObjectPool(_goldPrefab);
            // Exp 풀 생성
            _expPool = new DropObjectPool(_expPrefab);

            _initDisposable = EventBus.Subscribe<UIOpenedNotificationEvent>(OnSetUITransform);
            EventBus.Subscribe<GoldDropRequestEvent>(OnGoldDrop).AddTo(_disposables);
            EventBus.Subscribe<ExpDropRequestEvent>(OnExpDrop).AddTo(_disposables);
            EventBus.Subscribe<StageEndedEvent>(OnStageEnd).AddTo(_disposables);
        }
        // 정리
        private void OnDestroy() {
            _initDisposable?.Dispose();
            _goldPool?.Dispose();
            _expPool?.Dispose();
            _disposables?.Dispose();
        }
        #region Event
        public void OnGoldDrop(GoldDropRequestEvent e) {
            if (e.amount <= 0) return;
            _goldPool.Spawn(e.position + new Vector3(Random.Range(-_RAND_RANGE, _RAND_RANGE), Random.Range(-_RAND_RANGE, _RAND_RANGE), 0), e.amount);
            Debug.Log("골드 생성");
        }

        public void OnExpDrop(ExpDropRequestEvent e) {
            if (e.amount <= 0) return;
            _expPool.Spawn(e.position + new Vector3(Random.Range(-_RAND_RANGE, _RAND_RANGE), Random.Range(-_RAND_RANGE, _RAND_RANGE), 0), e.amount);
        }

        public void OnSetUITransform(UIOpenedNotificationEvent e) {
            if (e.uiName == UIName.Gold_UI) {
                _goldUiTr = e.uiObject.transform;
            } else if(e.uiName == UIName.Exp_UI) {
                _expUiTr = e.uiObject.transform;
            }
            if (_goldUiTr != null && _expUiTr != null) {
                _initDisposable?.Dispose();
            }

        }

        public void OnStageEnd(StageEndedEvent e) {
            Vector3 goldPos = Camera.main.ScreenToWorldPoint(_goldUiTr.position);
            Vector3 expPos = Camera.main.ScreenToWorldPoint(_expUiTr.position);

            foreach(var amountObject in _goldPool.LiveObjects) {
                amountObject.obj.transform.DOMove(goldPos, _MOVE_DURATION)
                    .OnComplete(() => {
                        EventBus.Publish(new GoldRewardEvent(amountObject.amount));
                    });
            }
            foreach (var amountObject in _expPool.LiveObjects) {
                amountObject.obj.transform.DOMove(expPos, _MOVE_DURATION)
                    .OnComplete(() => {
                        EventBus.Publish(new ExpRewardEvent(amountObject.amount));
                    });
            }
        }
        #endregion
    }
}
