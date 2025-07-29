namespace Game.Core.Event { 
    /// <summary>
    /// 이벤트 베이스 클래스
    /// </summary>
    public abstract class EventBase
    {
        public float Timestamp { get; }
        
        protected EventBase()
        {
            Timestamp = UnityEngine.Time.time;
        }
    }
    
}