using R3;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Event {
    /// <summary>
    /// Event Bus 인터페이스
    /// </summary>
    public interface IEventBus {
        /// <summary>
        /// 이벤트 발행
        /// </summary>
        void Publish<T>(T eventData) where T : struct;

        /// <summary>
        /// 이벤트 구독
        /// </summary>
        IDisposable Subscribe<T>(Action<T> onEvent) where T : struct;

        /// <summary>
        /// 특정 타입의 이벤트 스트림 가져오기
        /// </summary>
        Observable<T> GetEventStream<T>() where T : struct;

        /// <summary>
        /// 모든 이벤트 정리
        /// </summary>
        void Clear();
    }
}