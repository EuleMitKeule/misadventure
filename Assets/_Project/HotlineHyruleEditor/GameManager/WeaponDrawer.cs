using HotlineHyrule.Level;
using HotlineHyrule.Weapons;
using HotlineHyrule.Weapons.Projectiles;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace HotlineHyruleEditor.GameManager
{
    public class WeaponDrawer : ScriptableObjectDrawer<WeaponData>
    {
        [HideLabel]
        [EnumToggleButtons]
        [ShowInInspector]
        [ShowIf("IsRanged")]
        [TitleGroup("Tools/Main/Vertical/Settings")]
        TabState CurrentTabState { get; set; }

        [HideLabel]
        [EnumToggleButtons]
        [ShowInInspector]
        [ShowIf("IsOnWeaponTab")]
        [TitleGroup("Tools/Main/Vertical/Settings")]
        WeaponTabState CurrentWeaponTabState { get; set; }

        [HideLabel]
        [EnumToggleButtons]
        [ShowInInspector]
        [ShowIf("IsOnProjectileTab")]
        [TitleGroup("Tools/Main/Vertical/Settings")]
        ProjectileTabState CurrentProjectileTabState { get; set; }

        bool IsOnWeaponTab => CurrentTabState == TabState.Weapon;
        bool IsOnProjectileTab => CurrentTabState == TabState.Projectile;

        protected override bool ShowSelected => IsOnWeaponTab && CurrentWeaponTabState == WeaponTabState.Settings;
        bool IsRanged => Selected is RangedWeaponData;

        bool ShowWeaponPrefab => IsOnWeaponTab && WeaponPrefab && CurrentWeaponTabState == WeaponTabState.Prefab;
        bool ShowProjectile => IsRanged && IsOnProjectileTab && ProjectileData && CurrentProjectileTabState == ProjectileTabState.Settings;
        bool ShowProjectilePrefab => IsRanged && IsOnProjectileTab && ProjectilePrefab && CurrentProjectileTabState == ProjectileTabState.Prefab;

        [ShowInInspector]
        [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
        [ShowIf("ShowWeaponPrefab")]
        [TitleGroup("Tools/Main/Vertical/Settings")]
        GameObject WeaponPrefab { get; set; }

        [ShowInInspector]
        [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
        [ShowIf("ShowProjectile")]
        [TitleGroup("Tools/Main/Vertical/Settings")]
        ProjectileData ProjectileData { get; set; }

        [ShowInInspector]
        [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
        [ShowIf("ShowProjectilePrefab")]
        [TitleGroup("Tools/Main/Vertical/Settings")]
        GameObject ProjectilePrefab { get; set; }

        [PropertySpace(5)]
        [HideLabel]
        [BoxGroup("Tools/Main/Vertical/Create")]
        [EnumToggleButtons]
        [ShowInInspector]
        WeaponBuilder.WeaponOwnerType CurrentWeaponOwnerType { get; set; }

        [PropertySpace(5)]
        [HideLabel]
        [BoxGroup("Tools/Main/Vertical/Create")]
        [EnumToggleButtons]
        [ShowInInspector]
        WeaponBuilder.WeaponType CurrentWeaponType { get; set; }

        [PropertySpace(5)]
        [HideIf("IsNotRanged")]
        [HideLabel]
        [BoxGroup("Tools/Main/Vertical/Create")]
        [EnumToggleButtons]
        [ShowInInspector]
        WeaponBuilder.ProjectileType CurrentProjectileType { get; set; }

        bool IsNotRanged => CurrentWeaponType != WeaponBuilder.WeaponType.Ranged;

        public override string Path => WeaponBuilder.ParentPath;

        [BoxGroup("Tools/Main/Weapon")]
        [PropertySpace(5, 5)]
        [PropertyOrder(2)]
        [Button]
        void DoSomething()
        {

        }

        public override void CreateNew()
        {
            var weaponData = WeaponBuilder.Create(NameForNew, CurrentWeaponType, CurrentWeaponOwnerType, CurrentProjectileType);

            SetSelected(weaponData);
        }

        public void CreateNew(
            string overrideName,
            string overridePath,
            WeaponBuilder.WeaponType weaponType,
            WeaponBuilder.WeaponOwnerType weaponOwnerType,
            WeaponBuilder.ProjectileType projectileType)
        {
            if (overrideName == "") return;
            if (overridePath == "") return;

            var weaponData = WeaponBuilder.Create(overrideName, weaponType, weaponOwnerType, projectileType);

            SetSelected(weaponData);
        }

        public override void CreateNew(
            string overrideName,
            string overridePath)
        {
            if (overrideName == "") return;
            if (overridePath == "") return;

            var weaponData = WeaponBuilder.Create(overrideName, CurrentWeaponType, CurrentWeaponOwnerType, CurrentProjectileType);

            SetSelected(weaponData);
        }

        public override void DeleteSelected()
        {
            if (!Selected) return;

            var message = $"Are you sure you want to delete the Weapon \"{Selected.name}\"?\nThis will also delete all animations and prefabs.";
            var isSure = EditorUtility.DisplayDialog("Delete Weapon", message, "Yes", "Cancel");

            if (!isSure) return;

            WeaponBuilder.Delete(Selected);
        }

        public override void RenameSelected()
        {
            if (!Selected) return;
            if (RenameName == "") return;

            WeaponBuilder.Rename(Selected, RenameName);
        }

        public override void SetSelected(object item)
        {
            if (item is WeaponData weaponData)
            {
                Selected = weaponData;
                WeaponPrefab = GetWeaponPrefab();
            }

            if (item is RangedWeaponData rangedWeaponData)
            {
                ProjectileData = GetProjectileData();
                ProjectilePrefab = GetProjectilePrefab();
            }
        }

        ProjectileData GetProjectileData()
        {
            var projectileData = ScriptableObject.CreateInstance<ProjectileData>();

            if (!Selected) return projectileData;

            var weaponAssetPath = AssetDatabase.GetAssetPath(Selected);
            var weaponFile = System.IO.Path.GetFileName(weaponAssetPath);
            var weaponDirectory = System.IO.Path.GetDirectoryName(weaponAssetPath);
            var projectileFile = weaponFile.Replace("weapon_", "projectile_");

            var projectileDataPath = $"{weaponDirectory}/{projectileFile}";
            projectileData = AssetDatabase.LoadAssetAtPath<ProjectileData>(projectileDataPath);

            return projectileData;
        }

        GameObject GetWeaponPrefab()
        {
            if (!Selected) return null;

            var weaponAssetPath = AssetDatabase.GetAssetPath(Selected);
            var weaponFile = System.IO.Path.GetFileNameWithoutExtension(weaponAssetPath);
            var weaponDirectory = System.IO.Path.GetDirectoryName(weaponAssetPath);

            var weaponPrefabPath = $"{weaponDirectory}/{weaponFile}.prefab";

            var weaponPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(weaponPrefabPath);

            return weaponPrefab;
        }

        GameObject GetProjectilePrefab()
        {
            if (!Selected) return null;

            var weaponAssetPath = AssetDatabase.GetAssetPath(Selected);
            var weaponFile = System.IO.Path.GetFileNameWithoutExtension(weaponAssetPath);
            var weaponDirectory = System.IO.Path.GetDirectoryName(weaponAssetPath);
            var projectileFile = weaponFile.Replace("weapon_", "projectile_");

            var projectilePrefabPath = $"{weaponDirectory}/{weaponFile}.prefab";

            var weaponPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(projectilePrefabPath);

            return weaponPrefab;
        }

        public override void SetPath(string newPath) { }

        enum TabState
        {
            Weapon,
            Projectile,
        }

        enum WeaponTabState
        {
            Settings,
            Prefab,
        }

        enum ProjectileTabState
        {
            Settings,
            Prefab,
        }
    }

}