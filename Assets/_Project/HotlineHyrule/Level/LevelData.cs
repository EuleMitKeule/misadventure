using UnityEngine;

namespace HotlineHyrule.Level
{
    [CreateAssetMenu(menuName = "Level/New Level")]
    public class LevelData : ScriptableObject
    {
        [SerializeField] public string areaName;
        [TextArea] [SerializeField] public string areaText;
        [SerializeField] public QuestData questData;
        [SerializeField] public Vector3Int playerSpawnPosition;
    }
}