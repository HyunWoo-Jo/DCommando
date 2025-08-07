using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using TMPro;
using Zenject;
using DG.Tweening;
using Game.Core;
using Game.Core.Event;
using Game.ViewModels;
using R3;

namespace Game.UI {
    /// <summary>
    /// 데미지 UI 매니저 - 플로팅 데미지 텍스트를 Object Pool로 관리
    /// </summary>
    public class DamageUIManager : MonoBehaviour {
        [Header("Dependencies")]
        [Inject] private UIViewModel _viewModel;
        [Inject] private SO_DamageUIStyle _damageUIStyle;

        [Header("Pool Settings")]
        [SerializeField] private int _poolDefaultCapacity = 20;
        [SerializeField] private int _poolMaxSize = 100;

        // 내부 변수
        private Dictionary<int, Transform> _healthPositionDict = new();
        private CompositeDisposable _disposables = new();
        private ObjectPool<TextMeshProUGUI> _damageUIPool;
        private GameObject _damageUIPrefab;
        private Transform _damageUIParent;

        private void Start() {
            Bind();
            InitializeDamageUIAsync();
        }

        private void OnDestroy() {
            _disposables?.Dispose();
            _damageUIPool?.Dispose();
            _viewModel?.ReleaseDamageUI();
        }

        // 바인딩
        private void Bind() {
            EventBus.Subscribe<UIOpenedNotificationEvent>(OnOpenedHealthUI)
                .AddTo(_disposables);
            EventBus.Subscribe<UIClosedNotificationEvent>(OnClosedHealthUI)
                .AddTo(_disposables);
            EventBus.Subscribe<DamageTakenEvent>(OnDamageUI)
                .AddTo(_disposables);
            EventBus.Subscribe<HealedEvent>(OnHealUI)
                .AddTo(_disposables);
        }

        // 데미지 UI 초기화
        private async void InitializeDamageUIAsync() {
            try {
                // 비동기로 프리팹 로드
                _damageUIPrefab = await _viewModel.LoadDamageUIPrefabAsync();
                _damageUIParent = _viewModel.GetParent(UIType.HUD);

                // Object Pool 생성
                _damageUIPool = new ObjectPool<TextMeshProUGUI>(
                    createFunc: CreateDamageUI,
                    actionOnGet: OnGetDamageUI,
                    actionOnRelease: OnReleaseDamageUI,
                    actionOnDestroy: OnDestroyDamageUI,
                    collectionCheck: true,
                    defaultCapacity: _poolDefaultCapacity,
                    maxSize: _poolMaxSize
                );

                GameDebug.Log($"DamageUIManager 초기화 완료 Pool Size: {_poolDefaultCapacity}");
            } catch (System.Exception e) {
                GameDebug.LogError($"DamageUIManager 초기화 실패: {e.Message}");
            }
        }

        #region Object Pool 관리

        // 오브젝트 생성
        private TextMeshProUGUI CreateDamageUI() {
            if (_damageUIPrefab == null || _damageUIParent == null) {
                GameDebug.LogError("DamageUI Prefab 또는 Parent가 null");
                return null;
            }

            GameObject obj = Instantiate(_damageUIPrefab, _damageUIParent);
            return obj.GetComponent<TextMeshProUGUI>();
        }

        // 풀에서 가져올 때
        private void OnGetDamageUI(TextMeshProUGUI textComp) {
            textComp.gameObject.SetActive(true);
            if (textComp != null) {
                textComp.alpha = 1.0f;
            }

            // 트윈 정지
            textComp.gameObject.transform.DOKill();
        }

        // 풀로 반환할 때
        private void OnReleaseDamageUI(TextMeshProUGUI textComp) {
            if (textComp != null) {
                var obj = textComp.gameObject;
                obj.SetActive(false);
                obj.transform.DOKill();
            }
        }

        // 오브젝트 파괴할 때
        private void OnDestroyDamageUI(TextMeshProUGUI textComp) {
            if (textComp != null) {
                textComp.transform.DOKill();
                Destroy(textComp.gameObject);
            }
        }

        #endregion

        #region UI 이벤트 처리

        // Health UI 생성 시 위치 등록
        private void OnOpenedHealthUI(UIOpenedNotificationEvent openedEvent) {
            if (openedEvent.uiName == UIName.Health_UI) {
                _healthPositionDict[openedEvent.id] = openedEvent.uiObject.transform;
                GameDebug.Log($"Health UI 위치 등록 Character {openedEvent.id}");
            }
        }

        // Health UI 제거 시 위치 해제
        private void OnClosedHealthUI(UIClosedNotificationEvent closedEvent) {
            if (closedEvent.uiName == UIName.Health_UI) {
                // ID를 직접 가져올 수 없으므로 null인 Transform 제거
                var keysToRemove = new List<int>();
                foreach (var kvp in _healthPositionDict) {
                    if (kvp.Value == null) {
                        keysToRemove.Add(kvp.Key);
                    }
                }

                foreach (var key in keysToRemove) {
                    _healthPositionDict.Remove(key);
                    GameDebug.Log($"Health UI 위치 해제 Character {key}");
                }
            }
        }

        #endregion

        #region 데미지/치료 UI 생성

        // 데미지 UI 생성 (이벤트에서 호출)
        private void OnDamageUI(DamageTakenEvent damageEvent) {
            ShowDamageUI(damageEvent.characterID, damageEvent.damage, damageEvent.type);
        }

        // 치료 UI 생성 (이벤트에서 호출)
        private void OnHealUI(HealedEvent healedEvent) {
            ShowDamageUI(healedEvent.characterID, healedEvent.healAmount, DamageType.Heal);
        }

        // 데미지 UI 생성 (공개 메서드)
        public void ShowDamageUI(int characterId, int amount, DamageType damageType, bool isCritical = false) {
            if (!CanShowDamageUI(characterId)) return;

            var textComp = _damageUIPool.Get();
            if (textComp == null) {
                GameDebug.LogError("TextMeshProUGUI 컴포넌트를 찾을 수 없음");
                _damageUIPool.Release(textComp);
                return;
            }

            // UI 설정
            SetupDamageUI(textComp, amount, damageType, isCritical);

            // 위치 설정
            Vector3 worldPos = _healthPositionDict[characterId].position;
            Vector3 startPos = worldPos + _damageUIStyle.StartOffset;
            textComp.gameObject.transform.position = startPos;

            // 애니메이션 시작
            StartDamageAnimation(textComp, isCritical);

            GameDebug.Log($"데미지 UI 생성 Character {characterId}: {amount} {damageType} damage");
        }

        // Miss UI 생성
        public void ShowMissUI(int characterId) {
            if (!CanShowDamageUI(characterId)) return;


            var textComp = _damageUIPool.Get();
            if (textComp == null) {
                GameDebug.LogError("TextMeshProUGUI 컴포넌트를 찾을 수 없음");
                _damageUIPool.Release(textComp);
                return;
            }

            // Miss UI 설정
            textComp.text = "MISS";
            textComp.color = _damageUIStyle.MissColor;

            // 위치 설정
            Vector3 worldPos = _healthPositionDict[characterId].position;
            Vector3 startPos = worldPos + _damageUIStyle.StartOffset;
            textComp.gameObject.transform.position = startPos;

            // Miss 애니메이션
            StartMissAnimation(textComp);

            GameDebug.Log($"Miss UI 생성 Character {characterId}");
        }

        #endregion

        #region UI 설정 및 애니메이션

        // 데미지 UI 설정
        private void SetupDamageUI(TextMeshProUGUI textComp, int amount, DamageType damageType, bool isCritical) {
            // 텍스트 설정
            if (damageType == DamageType.Heal) {
                textComp.text = $"+{amount}";
            } else {
                string criticalMark = isCritical ? "!" : "";
                textComp.text = $"-{amount}{criticalMark}";
            }

            // 색상 설정
            textComp.color = _damageUIStyle.GetDamageTypeColor(damageType);

            // 크리티컬인 경우 폰트 크기 증가
            if (isCritical && damageType != DamageType.Heal) {
                var criticalStyle = _damageUIStyle.GetFloatingUIStyle(DamageUIType.Critical);
                textComp.fontSize *= criticalStyle.fontSizeMultiplier;
            }
        }

        // 데미지 애니메이션
        private void StartDamageAnimation(TextMeshProUGUI textComp, bool isCritical) {
            var obj = textComp.gameObject;
            Vector3 startPos = obj.transform.position;
            Vector3 endPos = startPos + _damageUIStyle.EndOffset;

            // 스타일 가져오기
            var styleData = isCritical ?
                _damageUIStyle.GetFloatingUIStyle(DamageUIType.Critical) :
                _damageUIStyle.GetFloatingUIStyle(DamageUIType.Normal);

            // 시퀀스 생성
            var sequence = DOTween.Sequence();

            // 위로 이동
            sequence.Append(
                obj.transform.DOMove(endPos, styleData.animationDuration)
                    .SetEase(_damageUIStyle.MovementCurve)
            );

            // 크리티컬 애니메이션
            if (isCritical) {
                // 스케일 효과
                sequence.Join(
                    obj.transform.DOScale(Vector3.one * styleData.scaleMultiplier, 0.2f)
                        .SetEase(Ease.OutBack)
                        .OnComplete(() => obj.transform.DOScale(Vector3.one, 0.2f))
                );

                // 반짝임 효과
                sequence.Insert(0.1f,
                    textComp.DOColor(Color.white, 0.1f)
                        .SetLoops(2, LoopType.Yoyo)
                );
            }

            // 페이드 아웃
            sequence.Insert(_damageUIStyle.FadeDelay,
                textComp.DOFade(0f, styleData.animationDuration - _damageUIStyle.FadeDelay)
                    .SetEase(Ease.InQuad)
            );

            // 완료 시 풀로 반환
            sequence.OnComplete(() => _damageUIPool.Release(textComp));
        }

        // Miss 애니메이션
        private void StartMissAnimation(TextMeshProUGUI textComp) {
            var obj = textComp.gameObject;
            var styleData = _damageUIStyle.GetFloatingUIStyle(DamageUIType.Miss);

            Vector3 startPos = obj.transform.position;
            Vector3 endPos = startPos + _damageUIStyle.EndOffset;

            // 시퀀스 생성
            var sequence = DOTween.Sequence();

            // 위로 이동
            sequence.Append(
                obj.transform.DOMove(endPos, styleData.animationDuration)
                    .SetEase(_damageUIStyle.MovementCurve)
            );

            // 좌우 흔들림
            sequence.Join(
                obj.transform.DOShakePosition(0.5f, new Vector3(20f, 0, 0), 8, 90f)
            );

            // 페이드 아웃
            sequence.Insert(_damageUIStyle.FadeDelay,
                textComp.DOFade(0f, styleData.animationDuration - _damageUIStyle.FadeDelay)
                    .SetEase(Ease.InQuad)
            );

            // 완료 시 풀로 반환
            sequence.OnComplete(() => _damageUIPool.Release(textComp));
        }

        #endregion

        #region 유틸리티

        // 데미지 UI 표시 가능 여부 확인
        private bool CanShowDamageUI(int characterId) {
            if (_damageUIPool == null) {
                GameDebug.LogWarning("DamageUI Pool이 초기화되지 않음");
                return false;
            }

            if (!_healthPositionDict.TryGetValue(characterId, out Transform healthTransform)) {
                GameDebug.LogWarning($"Health UI 위치를 찾을 수 없음 Character {characterId}");
                return false;
            }

            if (healthTransform == null) {
                GameDebug.LogWarning($"Health UI Transform이 null Character {characterId}");
                _healthPositionDict.Remove(characterId);
                return false;
            }

            return true;
        }

        // 풀 상태 정보 (디버그용)
        public string GetPoolStatus() {
            if (_damageUIPool == null) return "Pool not initialized";

            return $"Pool Status - Active: {_damageUIPool.CountActive}, " +
                   $"Inactive: {_damageUIPool.CountInactive}, " +
                   $"Total: {_damageUIPool.CountAll}";
        }

        #endregion
    }
}