using UnityEngine;

namespace HotlineHyrule.Level
{
    public class QuestData : ScriptableObject
    {
        [TextArea] [SerializeField] public string questText;
    }
}