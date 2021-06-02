using HotlineHyrule.Quests;
using UnityEngine;

namespace HotlineHyrule.Level
{
    [CreateAssetMenu(menuName = "Level/New Level")]
    public class LevelData : ScriptableObject
    {
        [SerializeField] public string areaName;
        [TextArea] [SerializeField] public string areaText;
        [SerializeField] public string areaFinishedText;
        [SerializeField] public QuestData questData;
        [SerializeField] public Vector3Int playerSpawnPosition;
        /// <summary>
        /// Whether to enable the rain effect.
        /// </summary>
        [Header("Effects")] [SerializeField] public bool isRaining;
        /// /// <summary>
        /// Whether to enable the snow effect.
        /// </summary>
        [SerializeField] public bool isSnowing;
    }
}