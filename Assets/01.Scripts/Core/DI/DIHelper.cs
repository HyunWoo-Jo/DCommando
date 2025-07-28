using UnityEngine;
using Zenject;

namespace Game.Core
{
    /// <summary>
    /// DI 컨테이너 유틸리티 헬퍼 클래스
    /// </summary>
    public static class DIHelper
    {
        private static DiContainer _container;
        
        /// <summary>
        /// 컨테이너 초기화 (Project Context에서 호출)
        /// </summary>
        /// <param name="container">DI 컨테이너</param>
        public static void Initialize(DiContainer container)
        {
            _container = container;
        }

        /// <summary>
        /// 현재 씬의 컨테이너에서 객체를 가져옴
        /// </summary>
        /// <typeparam name="T">가져올 객체 타입</typeparam>
        /// <returns>바인딩된 객체</returns>
        public static T Resolve<T>()
        {
            if (_container == null)
            {
                Debug.LogError("DI Container가 초기화되지 않았습니다. ProjectContext 설정을 확인하세요.");
                return default(T);
            }
            
            return _container.Resolve<T>();
        }

        /// <summary>
        /// ID를 가진 객체를 가져옴
        /// </summary>
        /// <typeparam name="T">가져올 객체 타입</typeparam>
        /// <param name="id">객체 식별자</param>
        /// <returns>바인딩된 객체</returns>
        public static T ResolveId<T>(string id)
        {
            if (_container == null)
            {
                Debug.LogError("DI Container가 초기화되지 않았습니다. ProjectContext 설정을 확인하세요.");
                return default(T);
            }
            
            return _container.ResolveId<T>(id);
        }

        /// <summary>
        /// 바인딩이 존재하는지 확인
        /// </summary>
        /// <typeparam name="T">확인할 타입</typeparam>
        /// <returns>바인딩 존재 여부</returns>
        public static bool HasBinding<T>()
        {
            if (_container == null)
            {
                return false;
            }
            
            return _container.HasBinding<T>();
        }

        /// <summary>
        /// 런타임에 객체를 인스턴스화하고 의존성 주입
        /// </summary>
        /// <param name="prefab">인스턴스화할 프리팹</param>
        /// <param name="parent">부모 Transform</param>
        /// <returns>인스턴스화된 GameObject</returns>
        public static GameObject InstantiateWithInjection(GameObject prefab, Transform parent = null)
        {
            if (_container == null)
            {
                Debug.LogError("DI Container가 초기화되지 않았습니다. ProjectContext 설정을 확인하세요.");
                return null;
            }
            
            return _container.InstantiatePrefab(prefab, parent);
        }

        /// <summary>
        /// 컴포넌트가 있는 프리팹을 인스턴스화하고 의존성 주입
        /// </summary>
        /// <typeparam name="T">컴포넌트 타입</typeparam>
        /// <param name="prefab">인스턴스화할 프리팹</param>
        /// <param name="parent">부모 Transform</param>
        /// <returns>컴포넌트</returns>
        public static T InstantiateComponentWithInjection<T>(GameObject prefab, Transform parent = null)
            where T : Component
        {
            if (_container == null)
            {
                Debug.LogError("DI Container가 초기화되지 않았습니다. ProjectContext 설정을 확인하세요.");
                return null;
            }
            
            return _container.InstantiatePrefabForComponent<T>(prefab, parent);
        }
    }
}