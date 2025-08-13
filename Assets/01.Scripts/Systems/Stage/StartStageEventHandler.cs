using UnityEngine;
using Game.Core.Event;
using Game.Core;
namespace Game.Systems {
    public class StartStageEventHandler : MonoBehaviour {
        [SerializeField] private StageName _stageName;
        void Start() {
            EventBus.Publish(new StartStageEvent(_stageName));
        }


    }
}