using UnityEngine;

namespace Game.Core
{
    public interface IModelInjecter<T>
    {
        void InjectModel(T model);
    }
    
    // 또는 object 타입 사용
    public interface IHealthModelInjecter
    {
        void InjectHealthModel(object healthModel);
    }
}