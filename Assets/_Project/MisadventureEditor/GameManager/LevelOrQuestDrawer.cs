using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;

namespace HotlineHyruleEditor.GameManager
{
    public class LevelOrQuestDrawer
    {
        [TitleGroup("Tools")]
        [EnumToggleButtons]
        [ShowInInspector]
        [HideLabel]
        [PropertySpace(10, 10)]
        LevelTabState CurrentLevelTabState { get; set; } = LevelTabState.Level;

        [PropertySpace(5)]
        [LabelWidth(100)]
        [LabelText("Subfolder")]
        [BoxGroup("Tools/Create")]
        [ValueDropdown("GetSubfolders")]
        [ShowInInspector]
        string SubfolderName { get; set; } = "";

        [PropertySpace(5)]
        [LabelWidth(100)]
        [LabelText("Name")]
        [BoxGroup("Tools/Create")]
        [ShowInInspector]
        string NameForNew { get; set; }

        GameManagerWindow GameManager { get; }

        public LevelOrQuestDrawer(GameManagerWindow gameManager) => GameManager = gameManager;

        [PropertySpace(SpaceAfter = 5)]
        [BoxGroup("Tools/Create")]
        [GUIColor(0, 0.9f, 0)]
        [Button]
        public void CreateNew()
        {
            if (NameForNew == "") return;

            var path = $"{LevelBuilder.ParentPath}/{SubfolderName}".TrimEnd('/');

            if (CurrentLevelTabState == LevelTabState.Level) GameManager.LevelDrawer.CreateNew(NameForNew, path);
            else GameManager.QuestDrawer.CreateNew(NameForNew, path);
        }

        List<string> GetSubfolders()
        {
            var subfolders = AssetDatabase.GetSubFolders(LevelBuilder.ParentPath).ToList();
            var existingSubfolderNames = subfolders.Select(System.IO.Path.GetFileName).ToList();

            var subfolderNames = new List<string>
            {
                "",
            };

            subfolderNames.AddRange(existingSubfolderNames);

            return subfolderNames;
        }

        enum LevelTabState
        {
            Level,
            Quest,
        }
    }
}