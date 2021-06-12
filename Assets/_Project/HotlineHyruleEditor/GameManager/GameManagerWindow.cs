using System;
using System.Collections.Generic;
using System.Linq;
using HotlineHyrule;
using HotlineHyrule.Items;
using HotlineHyrule.Level;
using HotlineHyrule.Quests;
using HotlineHyrule.Weapons;
using HotlineHyrule.Weapons.Projectiles;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace HotlineHyruleEditor.GameManager
{
    public class GameManagerWindow : OdinMenuEditorWindow
    {
        [OnValueChanged("OnTabStateChanged")]
        [HideLabel]
        [EnumToggleButtons]
        [ShowInInspector]
        [OdinSerialize]
        TabState CurrentTabState { get; set; }
        int CurrentEnumIndex { get; set; }

        public ItemDrawer ItemDrawer { get; private set; }
        public WeaponDrawer WeaponDrawer { get; private set; }
        public LevelDrawer LevelDrawer { get; private set; }
        public ScriptableObjectDrawer<QuestData> QuestDrawer { get; private set; }
        public LevelOrQuestDrawer LevelOrQuestDrawer { get; private set; }
        public EnemyDrawer EnemyDrawer { get; private set; }

        object Selected => MenuTree?.Selection?.SelectedValue;

        bool treeRebuild;

        [MenuItem("Tools/Game Manager")]
        public static void ShowWindow()
        {
            GetWindow<GameManagerWindow>("Game Manager").Show();
        }

        protected override void Initialize()
        {
            ItemDrawer = new ItemDrawer();
            WeaponDrawer = new WeaponDrawer();
            LevelDrawer = new LevelDrawer();
            QuestDrawer = new ScriptableObjectDrawer<QuestData>();
            LevelOrQuestDrawer = new LevelOrQuestDrawer(this);
            EnemyDrawer = new EnemyDrawer();

            QuestDrawer.SetPath(LevelBuilder.ParentPath);

            OnSelectionChanged(SelectionChangedType.SelectionCleared);
        }

        void OnTabStateChanged()
        {
            treeRebuild = true;
        }

        protected override void OnGUI()
        {
            if (treeRebuild && Event.current.type == EventType.Layout)
            {
                ForceMenuTreeRebuild();
                treeRebuild = false;
            }

            SirenixEditorGUI.BeginHorizontalToolbar();

            SirenixEditorGUI.Title("Game Manager", "", TextAlignment.Center, true);

            if (SirenixEditorGUI.ToolbarButton("Refresh"))
            {
                Initialize();
            }

            SirenixEditorGUI.EndHorizontalToolbar();

            EditorGUILayout.Space();

            switch (CurrentTabState)
            {
                case TabState.Levels:
                case TabState.Enemies:
                case TabState.Weapons:
                case TabState.Items:
                    DrawEditor(CurrentEnumIndex);
                    break;
            }

            EditorGUILayout.Space();

            base.OnGUI();
        }

        void OnSelectionChanged(SelectionChangedType selectionChangedType)
        {
            switch (CurrentTabState)
            {
                case TabState.Levels:
                    LevelDrawer.SetSelected(Selected);
                    QuestDrawer.SetSelected(Selected);
                    break;
                case TabState.Weapons:
                    WeaponDrawer.SetSelected(Selected);
                    break;
                case TabState.Items:
                    ItemDrawer.SetSelected(Selected);
                    break;
                case TabState.Enemies:
                    EnemyDrawer.SetSelected(Selected);
                    break;
            }
        }

        protected override void DrawEditors()
        {
            switch (CurrentTabState)
            {
                case TabState.Sound:
                    DrawEditor(CurrentEnumIndex);
                    break;
            }

            DrawEditor((int)CurrentTabState);
        }

        protected override IEnumerable<object> GetTargets()
        {
            var targets = new List<object>();

            if (Selected is LevelData) targets.Add(LevelDrawer);
            else if (Selected is QuestData) targets.Add(QuestDrawer);
            else targets.Add(LevelOrQuestDrawer);

            targets.Add(EnemyDrawer);
            targets.Add(WeaponDrawer);
            targets.Add(ItemDrawer);
            targets.Add(null);
            targets.Add(base.GetTarget());

            CurrentEnumIndex = targets.Count - 1;

            return targets;
        }

        protected override void DrawMenu()
        {
            switch (CurrentTabState)
            {
                case TabState.Levels:
                case TabState.Enemies:
                case TabState.Weapons:
                case TabState.Items:
                    base.DrawMenu();
                    break;
            }
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();

            switch (CurrentTabState)
            {
                case TabState.Levels:
                    var subfolders = AssetDatabase.GetSubFolders(LevelBuilder.ParentPath).ToList();

                    tree.Add("Level", null);
                    tree.Add("Quest", null);
                    tree.AddAllAssetsAtPath("Level", LevelBuilder.ParentPath, typeof(LevelData));
                    tree.AddAllAssetsAtPath("Quest", LevelBuilder.ParentPath, typeof(QuestData));

                    foreach (var subfolder in subfolders)
                    {
                        var subfolderName = System.IO.Path.GetFileName(subfolder);

                        tree.Add($"Level/{subfolderName}", null);
                        tree.Add($"Quest/{subfolderName}", null);
                        tree.AddAllAssetsAtPath($"Level/{subfolderName}", subfolder, typeof(LevelData));
                        tree.AddAllAssetsAtPath($"Quest/{subfolderName}", subfolder, typeof(QuestData));
                    }

                    break;
                case TabState.Enemies:
                    tree.AddAllAssetsAtPath("Enemies", EnemyBuilder.Path, typeof(GameObject), true);
                    break;
                case TabState.Weapons:
                    tree.AddAllAssetsAtPath("Weapons/Enemy", $"{WeaponBuilder.ParentPath}/Enemy", typeof(WeaponData), true, true);
                    tree.AddAllAssetsAtPath("Weapons/Player", $"{WeaponBuilder.ParentPath}/Player", typeof(WeaponData), true, true);
                    break;
                case TabState.Items:
                    tree.AddAllAssetsAtPath("Items", ItemBuilder.Path, typeof(ItemData), true);
                    break;
            }

            tree.Selection.SelectionChanged += OnSelectionChanged;
            return tree;
        }

        enum TabState
        {
            Levels,
            Enemies,
            Weapons,
            Items,
            Sound,
        }
    }
}