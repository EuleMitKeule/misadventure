using System;
using Sirenix.OdinInspector;

namespace HotlineHyrule.Items
{
    public class QuestDestroyComponent : SerializedMonoBehaviour
    {
        public event EventHandler Destroyed;

        void OnDestroy() => Destroyed?.Invoke(this, EventArgs.Empty);
    }
}