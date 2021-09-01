using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HotlineHyrule;
using HotlineHyrule.Entities;
using HotlineHyrule.Items;
using HotlineHyrule.Weapons;
using HotlineHyrule.Weapons.Projectiles;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HotlineHyruleEditor.GameManager
{
    public static class WeaponBuilder
    {
        public static string ParentPath => "Assets/_Project/Prefabs/Weapons";
        public static string AnimationParentPath => "Assets/_Project/Graphics/Animations/Weapons";

        public static WeaponData Create(string weaponName, WeaponType weaponType, WeaponOwnerType weaponOwnerType, ProjectileType projectileType)
        {
            if (weaponName == null || !Regex.IsMatch(weaponName, @"^([a-z])+(_([a-z])+)*$"))
            { 
                Debug.LogError("Weapon name is invalid. Use only lower-case letters and underscores.");
                return null;
            }

            var weaponPrefab = CreateWeaponPrefab(weaponName, weaponOwnerType);

            var weaponData = weaponType switch
            {
                WeaponType.Ranged => CreateRangedWeapon(weaponName, weaponOwnerType, projectileType, weaponPrefab),
                WeaponType.Melee => CreateMeleeWeapon(weaponName, weaponOwnerType, weaponPrefab),
                WeaponType.Conjuring => CreateConjuringWeapon(weaponName, weaponOwnerType, weaponPrefab),
                _ => ScriptableObject.CreateInstance<WeaponData>(),
            };

            if (weaponOwnerType == WeaponOwnerType.Player)
            {
                CreateDroppedWeapon(weaponName, weaponOwnerType, weaponData);
            }

            AssetDatabase.SaveAssets();

            return weaponData;
        }

        static void CreateDroppedWeapon(string weaponName, WeaponOwnerType weaponOwnerType, WeaponData weaponData)
        {
            var prefabOwnerPath = $"{ParentPath}/{weaponOwnerType.ToString()}";
            var prefabPath = $"{prefabOwnerPath}/{weaponName}";

            var droppedWeaponPrefab = new GameObject($"weapon_{weaponName}_dropped");
            droppedWeaponPrefab.layer = PhysicsLayer.ITEM;

            var droppedWeaponSpriteRenderer = droppedWeaponPrefab.AddComponent<SpriteRenderer>();
            droppedWeaponSpriteRenderer.sortingLayerName = "item";

            var droppedWeaponCollider = droppedWeaponPrefab.AddComponent<CircleCollider2D>();
            droppedWeaponCollider.isTrigger = true;

            var droppedWeaponItemComponent = droppedWeaponPrefab.AddComponent<ItemComponent>();

            droppedWeaponPrefab.AddComponent<DroppedWeaponComponent>();

            droppedWeaponItemComponent.itemDatas.Add(weaponData);

            PrefabUtility.SaveAsPrefabAsset(droppedWeaponPrefab, $"{prefabPath}/weapon_{weaponName}_dropped.prefab");

            weaponData.ItemPrefab =
                AssetDatabase.LoadAssetAtPath<GameObject>($"{prefabPath}/weapon_{weaponName}_dropped.prefab");

            Object.DestroyImmediate(droppedWeaponPrefab);
        }

        static ConjuringWeaponData CreateConjuringWeapon(string weaponName, WeaponOwnerType weaponOwnerType, GameObject weaponPrefab)
        {
            var prefabOwnerPath = $"{ParentPath}/{weaponOwnerType.ToString()}";
            var prefabPath = $"{prefabOwnerPath}/{weaponName}";

            var conjuringWeaponData = ScriptableObject.CreateInstance<ConjuringWeaponData>();
            conjuringWeaponData.weaponPrefab = weaponPrefab;

            weaponPrefab.AddComponent<SpawnerComponent>();

            conjuringWeaponData.weaponPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{prefabPath}/weapon_{weaponName}.prefab");

            AssetDatabase.CreateAsset(conjuringWeaponData, $"{prefabPath}/weapon_{weaponName}.asset");

            return conjuringWeaponData;
        }

        static MeleeWeaponData CreateMeleeWeapon(string weaponName, WeaponOwnerType weaponOwnerType, GameObject weaponPrefab)
        {
            var prefabOwnerPath = $"{ParentPath}/{weaponOwnerType.ToString()}";
            var prefabPath = $"{prefabOwnerPath}/{weaponName}";

            var meleeWeaponData = ScriptableObject.CreateInstance<MeleeWeaponData>();
            meleeWeaponData.weaponPrefab = weaponPrefab;

            var weaponHitbox = weaponPrefab.AddComponent<BoxCollider2D>();
            weaponHitbox.isTrigger = true;

            AssetDatabase.CreateAsset(meleeWeaponData, $"{prefabPath}/weapon_{weaponName}.asset");

            return meleeWeaponData;
        }

        static RangedWeaponData CreateRangedWeapon(string weaponName, WeaponOwnerType weaponOwnerType,
            ProjectileType projectileType, GameObject weaponPrefab)
        {
            var prefabOwnerPath = $"{ParentPath}/{weaponOwnerType.ToString()}";
            var prefabPath = $"{prefabOwnerPath}/{weaponName}";

            var rangedWeaponData = ScriptableObject.CreateInstance<RangedWeaponData>();

            AssetDatabase.CreateAsset(rangedWeaponData, $"{prefabPath}/weapon_{weaponName}.asset");

            var (_, projectilePrefab) =
                CreateProjectile(weaponName, weaponOwnerType, projectileType, rangedWeaponData);

            rangedWeaponData.weaponPrefab = weaponPrefab;
            rangedWeaponData.projectilePrefab = projectilePrefab;

            return rangedWeaponData;
        }

        static (ProjectileData, GameObject) CreateProjectile(string weaponName, WeaponOwnerType weaponOwnerType, ProjectileType projectileType, RangedWeaponData rangedWeaponData)
        {
            var prefabOwnerPath = $"{ParentPath}/{weaponOwnerType.ToString()}";
            var prefabPath = $"{prefabOwnerPath}/{weaponName}";

            var projectileDataType = projectileType == ProjectileType.Curved
                ? typeof(CurvedProjectileData)
                : typeof(LinearProjectileData);
            var projectileData = (ProjectileData)ScriptableObject.CreateInstance(projectileDataType);

            AssetDatabase.CreateAsset(projectileData, $"{prefabPath}/projectile_{weaponName}.asset");

            var projectilePrefab = CreateProjectilePrefab(weaponName, weaponOwnerType, projectileType, rangedWeaponData, projectileData);

            return (projectileData, projectilePrefab);
        }

        static GameObject CreateProjectilePrefab(string weaponName, WeaponOwnerType weaponOwnerType, ProjectileType projectileType, RangedWeaponData rangedWeaponData, ProjectileData projectileData)
        {
            var prefabOwnerPath = $"{ParentPath}/{weaponOwnerType.ToString()}";
            var prefabPath = $"{prefabOwnerPath}/{weaponName}";

            var projectilePrefab = new GameObject($"projectile_{weaponName}");
            projectilePrefab.layer = weaponOwnerType == WeaponOwnerType.Player
                ? PhysicsLayer.PROJECTILE
                : PhysicsLayer.ENEMY_PROJECTILE;

            var projectileSpriteRenderer = projectilePrefab.AddComponent<SpriteRenderer>();
            projectileSpriteRenderer.sortingLayerName = "projectile";

            var projectileController = CreateProjectileAnimations(weaponName, weaponOwnerType);
            var projectileAnimator = projectilePrefab.AddComponent<Animator>();
            projectileAnimator.runtimeAnimatorController = projectileController;

            var projectileRigidbody = projectilePrefab.AddComponent<Rigidbody2D>();
            projectileRigidbody.angularDrag = 0f;
            projectileRigidbody.gravityScale = 0f;
            projectileRigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            projectilePrefab.AddComponent<CapsuleCollider2D>();

            var projectileComponent = projectilePrefab.AddComponent<ProjectileComponent>();
            projectileComponent.projectileData = projectileData;
            projectileComponent.rangedWeaponData = rangedWeaponData;

            var enemyHitboxLayer = (weaponOwnerType == WeaponOwnerType.Player ? LayerMask.NameToLayer("enemyHitbox") : PhysicsLayer.PLAYER);
            var impactMask = 1 << PhysicsLayer.WALL | 1 << enemyHitboxLayer;

            projectileComponent.projectileData.impactMask = impactMask;

            PrefabUtility.SaveAsPrefabAsset(projectilePrefab, $"{prefabPath}/projectile_{weaponName}.prefab");

            Object.DestroyImmediate(projectilePrefab);

            projectilePrefab =
                AssetDatabase.LoadAssetAtPath<GameObject>($"{prefabPath}/projectile_{weaponName}.prefab");

            return projectilePrefab;
        }

        static GameObject CreateWeaponPrefab(string weaponName, WeaponOwnerType weaponOwnerType)
        {
            var prefabOwnerPath = $"{ParentPath}/{weaponOwnerType.ToString()}";
            var prefabPath = $"{prefabOwnerPath}/{weaponName}";

            AssetDatabase.CreateFolder(prefabOwnerPath, weaponName);

            var weaponController = CreateWeaponAnimations(weaponName, weaponOwnerType);

            var weaponLayer = LayerMask.NameToLayer(weaponOwnerType.ToString().ToLower());
            var weaponSortingLayer = weaponOwnerType.ToString().ToLower();

            var weaponPrefab = new GameObject($"weapon_{weaponName}")
            {
                layer = weaponLayer,
            };

            var weaponSpriteRenderer = weaponPrefab.AddComponent<SpriteRenderer>();
            weaponSpriteRenderer.sortingLayerName = weaponSortingLayer;
            weaponSpriteRenderer.sortingOrder = -1;

            var weaponAnimator = weaponPrefab.AddComponent<Animator>();
            weaponAnimator.runtimeAnimatorController = weaponController;

            weaponPrefab.AddComponent<WeaponAnimationComponent>();

            PrefabUtility.SaveAsPrefabAsset(weaponPrefab, $"{prefabPath}/weapon_{weaponName}.prefab");

            Object.DestroyImmediate(weaponPrefab);

            weaponPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{prefabPath}/weapon_{weaponName}.prefab");

            return weaponPrefab;
        }

        static AnimatorOverrideController CreateWeaponAnimations(string weaponName, WeaponOwnerType weaponOwnerType)
        {
            var animationOwnerPath = $"{AnimationParentPath}/{weaponOwnerType.ToString()}";
            var animationPath = $"{animationOwnerPath}/{weaponName}";
            var baseAnimationPath = $"{AnimationParentPath}/base";

            if (!AssetDatabase.IsValidFolder(animationPath)) AssetDatabase.CreateFolder(animationOwnerPath, weaponName);

            var weaponBaseController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>($"{baseAnimationPath}/controller_weapon_base.controller");
            var weaponBaseControllerIdle = AssetDatabase.LoadAssetAtPath<AnimationClip>($"{baseAnimationPath}/animation_weapon_base_idle.anim");
            var weaponBaseControllerAttack = AssetDatabase.LoadAssetAtPath<AnimationClip>($"{baseAnimationPath}/animation_weapon_base_attack.anim");
            var weaponBaseControllerImpact = AssetDatabase.LoadAssetAtPath<AnimationClip>($"{baseAnimationPath}/animation_weapon_base_impact.anim");

            var weaponAnimationIdle = new AnimationClip();
            var weaponAnimationAttack = new AnimationClip();
            var weaponAnimationImpact = new AnimationClip();

            AssetDatabase.CreateAsset(weaponAnimationIdle, $"{animationPath}/animation_weapon_{weaponName}_idle.anim");
            AssetDatabase.CreateAsset(weaponAnimationAttack, $"{animationPath}/animation_weapon_{weaponName}_attack.anim");
            AssetDatabase.CreateAsset(weaponAnimationImpact, $"{animationPath}/animation_weapon_{weaponName}_impact.anim");

            var weaponController = new AnimatorOverrideController(weaponBaseController);
            var weaponOverrides = new List<KeyValuePair<AnimationClip, AnimationClip>>
            {
                new KeyValuePair<AnimationClip, AnimationClip>(weaponBaseControllerIdle, weaponAnimationIdle),
                new KeyValuePair<AnimationClip, AnimationClip>(weaponBaseControllerAttack, weaponAnimationAttack),
                new KeyValuePair<AnimationClip, AnimationClip>(weaponBaseControllerImpact, weaponAnimationImpact),
            };

            weaponController.ApplyOverrides(weaponOverrides);
            AssetDatabase.CreateAsset(weaponController, $"{animationPath}/controller_weapon_{weaponName}.controller");

            return weaponController;
        }

        static AnimatorOverrideController CreateProjectileAnimations(string weaponName, WeaponOwnerType weaponOwnerType)
        {
            var animationOwnerPath = $"{AnimationParentPath}/{weaponOwnerType.ToString()}";
            var animationPath = $"{animationOwnerPath}/{weaponName}";
            var baseAnimationPath = $"{AnimationParentPath}/base";

            if (!AssetDatabase.IsValidFolder(animationPath)) AssetDatabase.CreateFolder(animationOwnerPath, weaponName);

            var projectileBaseController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>($"{baseAnimationPath}/controller_projectile_base.controller");
            var projectileBaseControllerIdle = AssetDatabase.LoadAssetAtPath<AnimationClip>($"{baseAnimationPath}/animation_projectile_base_idle.anim");
            var projectileBaseControllerAttack = AssetDatabase.LoadAssetAtPath<AnimationClip>($"{baseAnimationPath}/animation_projectile_base_attack.anim");
            var projectileBaseControllerImpact = AssetDatabase.LoadAssetAtPath<AnimationClip>($"{baseAnimationPath}/animation_projectile_base_impact.anim");
            var projectileBaseControllerStop = AssetDatabase.LoadAssetAtPath<AnimationClip>($"{baseAnimationPath}/animation_projectile_base_stop.anim");

            var projectileAnimationIdle = new AnimationClip();
            var projectileAnimationAttack = new AnimationClip();
            var projectileAnimationImpact = new AnimationClip();
            var projectileAnimationStop = new AnimationClip();

            AssetDatabase.CreateAsset(projectileAnimationIdle, $"{animationPath}/animation_projectile_{weaponName}_idle.anim");
            AssetDatabase.CreateAsset(projectileAnimationAttack, $"{animationPath}/animation_projectile_{weaponName}_attack.anim");
            AssetDatabase.CreateAsset(projectileAnimationImpact, $"{animationPath}/animation_projectile_{weaponName}_impact.anim");
            AssetDatabase.CreateAsset(projectileAnimationStop, $"{animationPath}/animation_projectile_{weaponName}_stop.anim");

            var projectileController = new AnimatorOverrideController(projectileBaseController);
            var projectileOverrides = new List<KeyValuePair<AnimationClip, AnimationClip>>
            {
                new KeyValuePair<AnimationClip, AnimationClip>(projectileBaseControllerIdle, projectileAnimationIdle),
                new KeyValuePair<AnimationClip, AnimationClip>(projectileBaseControllerAttack, projectileAnimationAttack),
                new KeyValuePair<AnimationClip, AnimationClip>(projectileBaseControllerImpact, projectileAnimationImpact),
                new KeyValuePair<AnimationClip, AnimationClip>(projectileBaseControllerStop, projectileAnimationStop),
            };

            projectileController.ApplyOverrides(projectileOverrides);
            AssetDatabase.CreateAsset(projectileController, $"{animationPath}/controller_projectile_{weaponName}.controller");

            return projectileController;
        }

        public static void Delete(WeaponData weaponData)
        {
            var assetPath = AssetDatabase.GetAssetPath(weaponData);
            var weaponDirectory = System.IO.Path.GetDirectoryName(assetPath);
            var weaponName = System.IO.Path.GetFileNameWithoutExtension(weaponDirectory);
            var weaponOwnerDirectory = System.IO.Path.GetDirectoryName(weaponDirectory);
            var weaponOwnerTypeName = System.IO.Path.GetFileName(weaponOwnerDirectory);
            var animationDirectory = $"{AnimationParentPath}/{weaponOwnerTypeName}/{weaponName}";

            var isValid = AssetDatabase.IsValidFolder(weaponDirectory);
            if (!isValid)
            {
                Debug.LogWarning($"Could not find directory \"{weaponDirectory}\"!");
                return;
            }

            isValid = AssetDatabase.IsValidFolder(animationDirectory);
            if (!isValid)
            {
                Debug.LogWarning($"Could not find directory \"{animationDirectory}\"!");
                return;
            }

            AssetDatabase.DeleteAsset(weaponDirectory);
            AssetDatabase.DeleteAsset(animationDirectory);
            AssetDatabase.SaveAssets();
        }

        public static void Rename(WeaponData weaponData, string newName)
        {
            if (newName == null || !Regex.IsMatch(newName, @"^([a-z])+(_([a-z])+)*$"))
            { 
                Debug.LogError("Weapon name is invalid. Use only lower-case letters and underscores.");
                return;
            }

            var assetPath = AssetDatabase.GetAssetPath(weaponData);
            var weaponDirectory = System.IO.Path.GetDirectoryName(assetPath);
            var weaponName = System.IO.Path.GetFileNameWithoutExtension(weaponDirectory);
            var weaponOwnerDirectory = System.IO.Path.GetDirectoryName(weaponDirectory);
            var weaponOwnerTypeName = System.IO.Path.GetFileName(weaponOwnerDirectory);
            var animationDirectory = $"{AnimationParentPath}/{weaponOwnerTypeName}/{weaponName}";

            var prefabAssets = GetUnityObjectsOfTypeFromPath<Object>(weaponDirectory);
            foreach (var prefabAsset in prefabAssets)
            {
                var path = AssetDatabase.GetAssetPath(prefabAsset);
                var newPath = path.Replace(weaponName ?? string.Empty, newName);
                var newFile = System.IO.Path.GetFileName(newPath);

                AssetDatabase.RenameAsset(path, newFile);
            }

            var animationAssets = GetUnityObjectsOfTypeFromPath<Object>(animationDirectory);
            foreach (var animationAsset in animationAssets)
            {
                var path = AssetDatabase.GetAssetPath(animationAsset);
                var newPath = path.Replace(weaponName ?? string.Empty, newName);
                var newFile = System.IO.Path.GetFileName(newPath);

                AssetDatabase.RenameAsset(path, newFile);
            }

            var newPrefabDirectory = $"{ParentPath}/{weaponOwnerTypeName}/{newName}";
            var newAnimationDirectory = $"{AnimationParentPath}/{weaponOwnerTypeName}/{newName}";

            AssetDatabase.MoveAsset(weaponDirectory, newPrefabDirectory);
            AssetDatabase.MoveAsset(animationDirectory, newAnimationDirectory);

            AssetDatabase.SaveAssets();
        }

        public static List<T> GetUnityObjectsOfTypeFromPath<T>(string path) where T : Object
        {
            var filePaths = System.IO.Directory.GetFiles(path);
            var assets = new List<T>();

            if (filePaths.Length <= 0) return assets;

            foreach (var file in filePaths)
            {
                var obj = AssetDatabase.LoadAssetAtPath(file, typeof(T));

                if (!(obj is T asset)) continue;
                if (assets.Contains(asset)) continue;

                assets.Add(asset);
            }

            return assets;
        }

        public enum WeaponType
        {
            Melee,
            Ranged,
            Conjuring,
            Targeting
        }

        public enum WeaponOwnerType
        {
            Player,
            Enemy,
        }

        public enum ProjectileType
        {
            Linear,
            Curved,
        }
    }
}