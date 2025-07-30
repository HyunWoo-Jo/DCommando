using R3;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Zenject;
namespace Game.Core.Event
{
    /// <summary>
    /// Event Bus ���� ����
    /// </summary>
    internal class EventBusCore : IEventBus {
        private static readonly Dictionary<Type, object> _subjects = new();
        private static readonly CompositeDisposable _disposables = new();

        /// <summary>
        /// �̺�Ʈ ����
        /// </summary>
        public void Publish<T>(T eventData) where T : class {
            if (eventData == null) {
                Debug.LogWarning($"�̺�Ʈ �����Ͱ� null�Դϴ�: {typeof(T).Name}");
                return;
            }

            var eventType = typeof(T);

            if (_subjects.TryGetValue(eventType, out var subject)) {
                if (subject is Subject<T> typedSubject) {
                    typedSubject.OnNext(eventData);
                    Debug.Log($"�̺�Ʈ ����: {eventType.Name}");
                }
            } else {
                Debug.LogWarning($"�����ڰ� ���� �̺�Ʈ�Դϴ�: {eventType.Name}");
            }
        }

        /// <summary>
        /// �̺�Ʈ ����
        /// </summary>
        public IDisposable Subscribe<T>(Action<T> onEvent) where T : class {
            if (onEvent == null) {
                Debug.LogError("�̺�Ʈ �ڵ鷯�� null�Դϴ�.");
                return Disposable.Empty;
            }

            var subject = GetOrCreateSubject<T>();
            var subscription = subject.Subscribe(onEvent);

            Debug.Log($"�̺�Ʈ ����: {typeof(T).Name}");
            return subscription;
        }

        /// <summary>
        /// Ư�� Ÿ���� �̺�Ʈ ��Ʈ�� ��������
        /// </summary>
        public Observable<T> GetEventStream<T>() where T : class {
            var subject = GetOrCreateSubject<T>();
            return subject.AsObservable();
        }

        /// <summary>
        /// Subject �������� �Ǵ� ����
        /// </summary>
        private Subject<T> GetOrCreateSubject<T>() where T : class {
            var eventType = typeof(T);

            if (!_subjects.TryGetValue(eventType, out var subject)) {
                var newSubject = new Subject<T>();
                _subjects[eventType] = newSubject;

                // Subject�� disposables�� �߰��Ͽ� �޸� ����
                newSubject.AddTo(_disposables);

                Debug.Log($"���ο� �̺�Ʈ ��Ʈ�� ����: {eventType.Name}");
                return newSubject;
            }

            return (Subject<T>)subject;
        }

        /// <summary>
        /// ��� �̺�Ʈ ����
        /// </summary>
        public void Clear() {
            Debug.Log("Event Bus ���� ��...");

            foreach (var kvp in _subjects) {
                if (kvp.Value is IDisposable disposable) {
                    disposable.Dispose();
                }
            }

            _subjects.Clear();
            _disposables.Clear();

            Debug.Log("Event Bus ���� �Ϸ�");
        }

        /// <summary>
        /// Event Bus ����
        /// </summary>
        public void Dispose() {
            Clear();
        }

        /// <summary>
        /// ���� ��ϵ� �̺�Ʈ Ÿ�� �� ��ȯ (������)
        /// </summary>
        public int GetRegisteredEventCount() {
            return _subjects.Count;
        }

        /// <summary>
        /// ��ϵ� �̺�Ʈ Ÿ�Ե� ��ȯ (������)
        /// </summary>
        public IEnumerable<Type> GetRegisteredEventTypes() {
            return _subjects.Keys;
        }

    }
}
