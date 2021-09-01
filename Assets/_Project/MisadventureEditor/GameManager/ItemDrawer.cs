using System;
using System.Collections.Generic;
using System.Linq;
using Misadventure.Items;
using Misadventure.Weapons;
using MisadventureEditor.Extensions;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace MisadventureEditor.GameManager
{
    public class ItemDrawer : ScriptableObjectDrawer<ItemData>
    {
        protected override string RenameName { get; set; }
        protected override bool ShowSelected => true;
        public override string Path { get; protected set; }
        public string CurrentPath => $"{Path}/{GetCurrentSubfolder()}";

        [ValueDropdown("@GetItemTypes()")]
        [ShowInInspector]
        [BoxGroup("Tools/Main/Vertical/Create")]
        Type SelectedItemType { get; set; }

        [ShowInInspector]
        [BoxGroup("Tools/Main/Vertical/Create")]
        bool HasPrefab { get; set; }

        GameObject Prefab => Selected ? Selected.GetPrefab() : null;

        [Button]
        [BoxGroup("Tools/Main/Item")]
        [ShowIf("Prefab")]
        void OpenPrefab()
        {
            if (!Selected) return;

            var prefab = Selected.GetPrefab();
            if (!prefab) return;

            Selection.activeGameObject = prefab;
            EditorApplication.ExecuteMenuItem("Window/General/Inspector");
        }

        public override void CreateNew()
        {
            var itemData = ItemBuilder.CreateItem(NameForNew, SelectedItemType, HasPrefab);
            SetSelected(itemData);
        }

        public override void CreateNew(string overrideName, string overridePath)
        {
            var itemData = ItemBuilder.CreateItem(overrideName, SelectedItemType, HasPrefab);
            SetSelected(itemData);
        }

        public override void RenameSelected()
        {
            if (!Selected) return;

            ItemBuilder.RenameItem(Selected, RenameName);
        }

        public override void DeleteSelected()
        {
            if (!Selected) return;

            var message =
                $"Are you sure you want to delete the item \"{Selected.name}\"?\nThis will also delete the prefab file.";
            var isSure = EditorUtility.DisplayDialog("Delete Item", message, "Yes", "Cancel");

            if (!isSure) return;

            ItemBuilder.DeleteItem(Selected);
        }

        public override void SetSelected(object item)
        {
            if (item is ItemData itemData)
            {
                Selected = itemData;
                NameForNew = itemData.ItemName;
                RenameName = itemData.ItemName;
                SelectedItemType = itemData.GetType();
                HasPrefab = itemData.GetPrefab();
            }
        }

        public override void SetPath(string newPath) { }

        List<Type> GetItemTypes() =>
            (from assembly in AppDomain.CurrentDomain.GetAssemblies()
            from type in assembly.GetTypes()
            where type.IsSubclassOf(typeof(ItemData))
            where !type.IsAbstract
            where !type.IsSubclassOf(typeof(WeaponData))
            select type).ToList();
    }
}