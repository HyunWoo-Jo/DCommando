using R3;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Zenject;
namespace Game.Core.Event
{
    /// <summary>
    /// Event Bus 내부 로직
    /// </summary>
    internal class EventBusCore : IEventBus {
        private static readonly Dictionary<Type, object> _subjects = new();
        private static readonly CompositeDisposable _disposables = new();

        /// <summary>
        /// 이벤트 발행
        /// </summary>
        public void Publish<T>(T eventData) where T : struct {
            var eventType = typeof(T);

            if (_subjects.TryGetValue(eventType, out var subject)) {
                if (subject is Subject<T> typedSubject) {
                    typedSubject.OnNext(eventData);
                    GameDebug.Log($"이벤트 발행: {eventType.Name}");
                }
            } else {
                GameDebug.LogWarning($"구독자가 없는 이벤트입니다: {eventType.Name}");
            }
        }

        /// <summary>
        /// 이벤트 구독
        /// </summary>
        public IDisposable Subscribe<T>(Action<T> onEvent) where T : struct {
            if (onEvent == null) {
                GameDebug.LogError("이벤트 핸들러가 null입니다.");
                return Disposable.Empty;
            }

            var subject = GetOrCreateSubject<T>();
            var subscription = subject.Subscribe(onEvent);

            GameDebug.Log($"이벤트 구독: {typeof(T).Name}");
            return subscription;
        }

        /// <summary>
        /// 특정 타입의 이벤트 스트림 가져오기
        /// </summary>
        public Observable<T> GetEventStream<T>() where T : struct {
            var subject = GetOrCreateSubject<T>();
            return subject.AsObservable();
        }

        /// <summary>
        /// Subject 가져오기 또는 생성
        /// </summary>
        private Subject<T> GetOrCreateSubject<T>() where T : struct {
            var eventType = typeof(T);

            if (!_subjects.TryGetValue(eventType, out var subject)) {
                var newSubject = new Subject<T>();
                _subjects[eventType] = newSubject;

                // Subject를 disposables에 추가하여 메모리 관리
                newSubject.AddTo(_disposables);

                GameDebug.Log($"새로운 이벤트 스트림 생성: {eventType.Name}");
                return newSubject;
            }

            return (Subject<T>)subject;
        }

        /// <summary>
        /// 모든 이벤트 정리
        /// </summary>
        public void Clear() {
            GameDebug.Log("Event Bus 정리 중...");

            foreach (var kvp in _subjects) {
                if (kvp.Value is IDisposable disposable) {
                    disposable.Dispose();
                }
            }

            _subjects.Clear();
            _disposables.Clear();

            GameDebug.Log("Event Bus 정리 완료");
        }

        /// <summary>
        /// Event Bus 해제
        /// </summary>
        public void Dispose() {
            Clear();
        }

        /// <summary>
        /// 현재 등록된 이벤트 타입 수 반환 (디버깅용)
        /// </summary>
        public int GetRegisteredEventCount() {
            return _subjects.Count;
        }

        /// <summary>
        /// 등록된 이벤트 타입들 반환 (디버깅용)
        /// </summary>
        public IEnumerable<Type> GetRegisteredEventTypes() {
            return _subjects.Keys;
        }

    }
}
