using R3;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Zenject;

namespace Game.Core.Event {
    // 편의를 위한 Static
    public static class EventBus {
        private static IEventBus _instance = new EventBusCore(); // 기본 EventBus로 생성


        /// <summary>
        /// Event Bus 클레스 변경
        /// </summary>
        /// <param name="eventBus"></param>
        public static void ChangeEventBus(IEventBus eventBus) => _instance = eventBus;

        /// <summary>
        /// 이벤트 발행
        /// </summary>
        public static void Publish<T>(T eventData) where T : struct {
            _instance.Publish(eventData);
        }
        /// <summary>
        /// 이벤트 구독
        /// </summary>
        public static IDisposable Subscribe<T>(Action<T> onEvent) where T : struct {
            return _instance.Subscribe(onEvent);
        }
        /// <summary>
        /// 특정 타입의 이벤트 스트림 가져오기
        /// </summary>
        public static Observable<T> GetEventStream<T>() where T : struct {
            return _instance.GetEventStream<T>();
        }

        public static void Clear() {
            _instance.Clear();
        }
    }
}