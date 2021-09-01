using System.Runtime.CompilerServices;
using HotlineHyrule.Extensions;
using HotlineHyrule.Weapons;
using HotlineHyrule.Weapons.Projectiles;
using HotlineHyruleEditor.GameManager;
using UnityEditor;
using UnityEngine;

namespace HotlineHyruleEditor.Extensions
{
    public static class WeaponDataExtensions
    {
        public static GameObject GetPrefab(this WeaponData weaponData) =>
            AssetDatabase.LoadAssetAtPath<GameObject>(weaponData.GetPrefabPath());

        public static GameObject GetDroppedPrefab(this WeaponData weaponData) =>
            AssetDatabase.LoadAssetAtPath<GameObject>(weaponData.GetDroppedPrefabPath());

        public static AnimationClip GetIdleAnimation(this WeaponData weaponData) =>
            AssetDatabase.LoadAssetAtPath<AnimationClip>(weaponData.GetIdleAnimationPath());

        public static AnimationClip GetAttackAnimation(this WeaponData weaponData) =>
            AssetDatabase.LoadAssetAtPath<AnimationClip>(weaponData.GetAttackAnimationPath());

        public static AnimationClip GetImpactAnimation(this WeaponData weaponData) =>
            AssetDatabase.LoadAssetAtPath<AnimationClip>(weaponData.GetImpactAnimationPath());

        public static string GetPrefabPath(this WeaponData weaponData) =>
            weaponData.GetPath().Replace(".asset", ".prefab");

        public static string GetDroppedPrefabPath(this WeaponData weaponData) =>
            weaponData.GetPath().Replace(".asset", "_dropped.prefab");

        public static string GetAnimationPath(this WeaponData weaponData) =>
            $"{WeaponBuilder.AnimationParentPath}/{weaponData.GetWeaponOwnerTypeName()}/{weaponData.ItemName}";

        public static string GetIdleAnimationPath(this WeaponData weaponData) =>
            $"{weaponData.GetAnimationPath()}/animation_weapon_{weaponData.ItemName}_idle.anim";

        public static string GetAttackAnimationPath(this WeaponData weaponData) =>
            $"{weaponData.GetAnimationPath()}/animation_weapon_{weaponData.ItemName}_attack.anim";

        public static string GetImpactAnimationPath(this WeaponData weaponData) =>
            $"{weaponData.GetAnimationPath()}/animation_weapon_{weaponData.ItemName}_impact.anim";

        public static string GetWeaponOwnerTypeName(this WeaponData weaponData)
        {
            var assetPath = weaponData.GetPath();
            var weaponDirectory = System.IO.Path.GetDirectoryName(assetPath);
            var weaponOwnerDirectory = System.IO.Path.GetDirectoryName(weaponDirectory);
            var weaponOwnerTypeName = System.IO.Path.GetFileName(weaponOwnerDirectory);

            return weaponOwnerTypeName;
        }
    }

    public static class RangedWeaponDataExtensions
    {
        public static ProjectileData GetProjectile(this RangedWeaponData weaponData) =>
            AssetDatabase.LoadAssetAtPath<ProjectileData>(weaponData.GetProjectilePath());

        public static GameObject GetProjectilePrefab(this RangedWeaponData weaponData) =>
            AssetDatabase.LoadAssetAtPath<GameObject>(weaponData.GetProjectilePrefabPath());

        public static AnimationClip GetProjectileIdleAnimation(this RangedWeaponData weaponData) =>
            AssetDatabase.LoadAssetAtPath<AnimationClip>(weaponData.GetProjectileIdleAnimationPath());

        public static AnimationClip GetProjectileAttackAnimation(this RangedWeaponData weaponData) =>
            AssetDatabase.LoadAssetAtPath<AnimationClip>(weaponData.GetProjectileAttackAnimationPath());

        public static AnimationClip GetProjectileImpactAnimation(this RangedWeaponData weaponData) =>
            AssetDatabase.LoadAssetAtPath<AnimationClip>(weaponData.GetProjectileImpactAnimationPath());

        public static AnimationClip GetProjectileStopAnimation(this RangedWeaponData weaponData) =>
            AssetDatabase.LoadAssetAtPath<AnimationClip>(weaponData.GetProjectileStopAnimationPath());

        public static string GetProjectilePath(this RangedWeaponData weaponData) =>
            weaponData.GetPath().Replace("weapon_", "projectile_");

        public static string GetProjectilePrefabPath(this RangedWeaponData weaponData) =>
            weaponData.GetPath().Replace("weapon_", "projectile_").Replace(".asset", ".prefab");

        public static string GetProjectileIdleAnimationPath(this RangedWeaponData weaponData) =>
            $"{weaponData.GetAnimationPath()}/animation_projectile_{weaponData.ItemName}_idle.anim";

        public static string GetProjectileAttackAnimationPath(this RangedWeaponData weaponData) =>
            $"{weaponData.GetAnimationPath()}/animation_projectile_{weaponData.ItemName}_attack.anim";

        public static string GetProjectileImpactAnimationPath(this RangedWeaponData weaponData) =>
            $"{weaponData.GetAnimationPath()}/animation_projectile_{weaponData.ItemName}_impact.anim";

        public static string GetProjectileStopAnimationPath(this RangedWeaponData weaponData) =>
            $"{weaponData.GetAnimationPath()}/animation_projectile_{weaponData.ItemName}_stop.anim";
    }
}