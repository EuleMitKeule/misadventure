using Misadventure.Weapons;
using Misadventure.Weapons.Projectiles;
using MisadventureEditor.Extensions;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace MisadventureEditor.GameManager
{
    public class WeaponDrawer : ScriptableObjectDrawer<WeaponData>
    {
        public override string Path => WeaponBuilder.ParentPath;
        protected override bool ShowSelected =>
            CurrentTabState == TabState.Weapon && CurrentWeaponTabState == SubTabState.Settings;

        [HideLabel]
        [EnumToggleButtons]
        [ShowInInspector]
        [ShowIf("@Selected is RangedWeaponData")]
        [TitleGroup("Tools/Main/Vertical/Settings")]
        TabState CurrentTabState { get; set; }

        [HideLabel]
        [EnumToggleButtons]
        [ShowInInspector]
        [ShowIf("@CurrentTabState == TabState.Weapon && Selected != null")]
        [TitleGroup("Tools/Main/Vertical/Settings")]
        SubTabState CurrentWeaponTabState { get; set; }

        [HideLabel]
        [EnumToggleButtons]
        [ShowInInspector]
        [ShowIf("@CurrentTabState == TabState.Projectile")]
        [TitleGroup("Tools/Main/Vertical/Settings")]
        SubTabState CurrentProjectileTabState { get; set; }

        [ShowInInspector]
        [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
        [ShowIf("@CurrentTabState == TabState.Weapon && CurrentWeaponTabState == SubTabState.Prefab")]
        [TitleGroup("Tools/Main/Vertical/Settings")]
        GameObject WeaponPrefab { get; set; }

        [ShowInInspector]
        [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
        [ShowIf("@Selected is RangedWeaponData && CurrentTabState == TabState.Projectile && CurrentProjectileTabState == SubTabState.Settings")]
        [TitleGroup("Tools/Main/Vertical/Settings")]
        ProjectileData ProjectileData { get; set; }

        [ShowInInspector]
        [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
        [ShowIf("@Selected is RangedWeaponData && CurrentTabState == TabState.Projectile && CurrentProjectileTabState == SubTabState.Prefab")]
        [TitleGroup("Tools/Main/Vertical/Settings")]
        GameObject ProjectilePrefab { get; set; }

        [ShowInInspector]
        [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
        [ShowIf("@CurrentTabState == TabState.Dropped")]
        [TitleGroup("Tools/Main/Vertical/Settings")]
        GameObject DroppedPrefab { get; set; }

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
        [ShowIf("@CurrentWeaponType == WeaponBuilder.WeaponType.Ranged")]
        [HideLabel]
        [BoxGroup("Tools/Main/Vertical/Create")]
        [EnumToggleButtons]
        [ShowInInspector]
        WeaponBuilder.ProjectileType CurrentProjectileType { get; set; }

        [LabelText("Idle")]
        [ShowIf("@CurrentTabState == TabState.Weapon && IdleAnimation != null")]
        [BoxGroup("Tools/Main/Animation")]
        [PropertySpace(5, 5)]
        [PropertyOrder(2)]
        [Button]
        void SelectWeaponIdleAnimation() => SelectAnimation(IdleAnimation);
        AnimationClip IdleAnimation => Selected ? Selected.GetIdleAnimation() : null;

        [LabelText("Attack")]
        [ShowIf("@CurrentTabState == TabState.Weapon && AttackAnimation != null")]
        [BoxGroup("Tools/Main/Animation")]
        [PropertySpace(5, 5)]
        [PropertyOrder(2)]
        [Button]
        void SelectWeaponAttackAnimation() => SelectAnimation(AttackAnimation);
        AnimationClip AttackAnimation => Selected ? Selected.GetAttackAnimation() : null;

        [LabelText("Impact")]
        [ShowIf("@CurrentTabState == TabState.Weapon && ImpactAnimation != null")]
        [BoxGroup("Tools/Main/Animation")]
        [PropertySpace(5, 5)]
        [PropertyOrder(2)]
        [Button]
        void SelectWeaponImpactAnimation() => SelectAnimation(ImpactAnimation);
        AnimationClip ImpactAnimation => Selected ? Selected.GetImpactAnimation() : null;

        [LabelText("Idle")]
        [ShowIf("@CurrentTabState == TabState.Projectile && ProjectileIdleAnimation != null")]
        [BoxGroup("Tools/Main/Animation")]
        [PropertySpace(5, 5)]
        [PropertyOrder(2)]
        [Button]
        void SelectProjectileIdleAnimation() => SelectAnimation(ProjectileIdleAnimation);
        AnimationClip ProjectileIdleAnimation =>
            Selected is RangedWeaponData rangedWeaponData ? rangedWeaponData.GetProjectileIdleAnimation() : null;

        [LabelText("Attack")]
        [ShowIf("@CurrentTabState == TabState.Projectile && ProjectileAttackAnimation != null")]
        [BoxGroup("Tools/Main/Animation")]
        [PropertySpace(5, 5)]
        [PropertyOrder(2)]
        [Button]
        void SelectAttackAttackAnimation() => SelectAnimation(ProjectileAttackAnimation);
        AnimationClip ProjectileAttackAnimation =>
            Selected is RangedWeaponData rangedWeaponData ? rangedWeaponData.GetProjectileAttackAnimation() : null;

        [LabelText("Impact")]
        [ShowIf("@CurrentTabState == TabState.Projectile && ProjectileImpactAnimation != null")]
        [BoxGroup("Tools/Main/Animation")]
        [PropertySpace(5, 5)]
        [PropertyOrder(2)]
        [Button]
        void SelectProjectileImpactAnimation() => SelectAnimation(ProjectileImpactAnimation);
        AnimationClip ProjectileImpactAnimation =>
            Selected is RangedWeaponData rangedWeaponData ? rangedWeaponData.GetProjectileImpactAnimation() : null;

        [LabelText("Stop")]
        [ShowIf("@CurrentTabState == TabState.Projectile && ProjectileStopAnimation != null")]
        [BoxGroup("Tools/Main/Animation")]
        [PropertySpace(5, 5)]
        [PropertyOrder(2)]
        [Button]
        void SelectProjectileStopAnimation() => SelectAnimation(ProjectileStopAnimation);
        AnimationClip ProjectileStopAnimation =>
            Selected is RangedWeaponData rangedWeaponData ? rangedWeaponData.GetProjectileStopAnimation() : null;

        public override void CreateNew()
        {
            var weaponData =
                WeaponBuilder.Create(NameForNew, CurrentWeaponType, CurrentWeaponOwnerType, CurrentProjectileType);
            SetSelected(weaponData);
        }

        public override void CreateNew(string overrideName, string overridePath)
        {
            var weaponData =
                WeaponBuilder.Create(overrideName, CurrentWeaponType, CurrentWeaponOwnerType, CurrentProjectileType);
            SetSelected(weaponData);
        }

        public override void DeleteSelected()
        {
            if (!Selected) return;

            var message =
                $"Are you sure you want to delete the Weapon \"{Selected.name}\"?\n" +
                $"This will also delete all animations and prefabs.";
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
                WeaponPrefab = weaponData.GetPrefab();
                DroppedPrefab = weaponData.GetDroppedPrefab();
                NameForNew = weaponData.ItemName;
                RenameName = weaponData.ItemName;

                CurrentWeaponType = weaponData switch
                {
                    MeleeWeaponData _ => WeaponBuilder.WeaponType.Melee,
                    RangedWeaponData _ => WeaponBuilder.WeaponType.Ranged,
                    ConjuringWeaponData _ => WeaponBuilder.WeaponType.Conjuring,
                    _ => WeaponBuilder.WeaponType.Melee,
                };

                CurrentWeaponOwnerType = weaponData.GetWeaponOwnerTypeName() switch
                {
                    "Player" => WeaponBuilder.WeaponOwnerType.Player,
                    "Enemy" => WeaponBuilder.WeaponOwnerType.Enemy,
                    _ => WeaponBuilder.WeaponOwnerType.Player,
                };
            }

            if (item is RangedWeaponData rangedWeaponData)
            {
                ProjectileData = rangedWeaponData.GetProjectile();
                ProjectilePrefab = rangedWeaponData.GetProjectilePrefab();

                CurrentProjectileType = ProjectileData switch
                {
                    LinearProjectileData _ => WeaponBuilder.ProjectileType.Linear,
                    CurvedProjectileData _ => WeaponBuilder.ProjectileType.Curved,
                    _ => WeaponBuilder.ProjectileType.Linear,
                };
            }
            else
            {
                CurrentTabState = TabState.Weapon;
            }
        }

        public override void SetPath(string newPath) { }

        static void SelectAnimation(AnimationClip animation)
        {
            Selection.activeObject = animation;
            EditorApplication.ExecuteMenuItem("Window/Animation/Animation");
        }

        enum TabState
        {
            Weapon,
            Projectile,
            Dropped,
        }

        enum SubTabState
        {
            Settings,
            Prefab,
        }
    }
}