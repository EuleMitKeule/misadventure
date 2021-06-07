using System;
using System.Collections.Generic;
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
            var animationOwnerPath = $"{AnimationParentPath}/{weaponOwnerType.ToString()}";
            var prefabOwnerPath = $"{ParentPath}/{weaponOwnerType.ToString()}";
            var animationPath = $"{animationOwnerPath}/{weaponName}";
            var prefabPath = $"{prefabOwnerPath}/{weaponName}";

            AssetDatabase.CreateFolder(animationOwnerPath, weaponName);
            AssetDatabase.CreateFolder(prefabOwnerPath, weaponName);

            //create weapon animator controller
            var weaponBaseController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/_Project/Graphics/Animations/Weapons/base/controller_weapon_base.controller");
            var weaponBaseControllerIdle = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/_project/Graphics/Animations/Weapons/base/animation_weapon_base_idle.anim");
            var weaponBaseControllerAttack = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/_project/Graphics/Animations/Weapons/base/animation_weapon_base_attack.anim");
            var weaponBaseControllerImpact = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/_project/Graphics/Animations/Weapons/base/animation_weapon_base_impact.anim");

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
                new KeyValuePair<AnimationClip, AnimationClip>(weaponBaseControllerImpact, weaponAnimationImpact)
            };

            weaponController.ApplyOverrides(weaponOverrides);
            AssetDatabase.CreateAsset(weaponController, $"{animationPath}/controller_weapon_{weaponName}.controller");

            //create weapon prefab
            var weaponPrefab = new GameObject($"weapon_{weaponName}");
            weaponPrefab.layer = LayerMask.NameToLayer(weaponOwnerType.ToString().ToLower());

            var weaponSpriteRenderer = weaponPrefab.AddComponent<SpriteRenderer>();
            weaponSpriteRenderer.sortingLayerName = weaponOwnerType.ToString().ToLower();
            weaponSpriteRenderer.sortingOrder = -1;

            var weaponAnimator = weaponPrefab.AddComponent<Animator>();
            weaponAnimator.runtimeAnimatorController = weaponController;

            weaponPrefab.AddComponent<WeaponAnimationComponent>();

            var weaponData = ScriptableObject.CreateInstance<WeaponData>();

            switch (weaponType)
            {
                //create weapon data
                case WeaponType.Ranged:
                {
                    var rangedWeaponData = ScriptableObject.CreateInstance<RangedWeaponData>();
                    weaponData = rangedWeaponData;
                    rangedWeaponData.weaponPrefab = weaponPrefab;

                    //create projectile animator controller
                    var projectileBaseController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/_Project/Graphics/Animations/Weapons/base/controller_projectile_base.controller");
                    var projectileBaseControllerIdle = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/_project/Graphics/Animations/Weapons/base/animation_projectile_base_idle.anim");
                    var projectileBaseControllerAttack = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/_project/Graphics/Animations/Weapons/base/animation_projectile_base_attack.anim");
                    var projectileBaseControllerImpact = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/_project/Graphics/Animations/Weapons/base/animation_projectile_base_impact.anim");
                    var projectileBaseControllerStop = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/_project/Graphics/Animations/Weapons/base/animation_projectile_base_stop.anim");

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
                        new KeyValuePair<AnimationClip, AnimationClip>(projectileBaseControllerStop, projectileAnimationStop)
                    };

                    projectileController.ApplyOverrides(projectileOverrides);
                    AssetDatabase.CreateAsset(projectileController, $"{animationPath}/controller_projectile_{weaponName}.controller");

                    //create projectile prefab
                    var projectilePrefab = new GameObject($"projectile_{weaponName}");
                    projectilePrefab.layer = weaponOwnerType == WeaponOwnerType.Player ? PhysicsLayer.PROJECTILE : PhysicsLayer.ENEMY_PROJECTILE;

                    var projectileSpriteRenderer = projectilePrefab.AddComponent<SpriteRenderer>();
                    projectileSpriteRenderer.sortingLayerName = "projectile";

                    var projectileAnimator = projectilePrefab.AddComponent<Animator>();
                    projectileAnimator.runtimeAnimatorController = projectileController;

                    var projectileRigidbody = projectilePrefab.AddComponent<Rigidbody2D>();
                    projectileRigidbody.angularDrag = 0f;
                    projectileRigidbody.gravityScale = 0f;
                    projectileRigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

                    projectilePrefab.AddComponent<CapsuleCollider2D>();

                    var projectileComponent = projectilePrefab.AddComponent<ProjectileComponent>();

                    //create projectile data
                    switch (projectileType)
                    {
                        case ProjectileType.Curved:
                        {
                            var curvedProjectileData = ScriptableObject.CreateInstance<CurvedProjectileData>();
                            AssetDatabase.CreateAsset(curvedProjectileData, $"{prefabPath}/projectile_{weaponName}.asset");

                            projectileComponent.projectileData = curvedProjectileData;
                            break;
                        }
                        case ProjectileType.Linear:
                        {
                            var linearProjectileData = ScriptableObject.CreateInstance<LinearProjectileData>();
                            AssetDatabase.CreateAsset(linearProjectileData, $"{prefabPath}/projectile_{weaponName}.asset");

                            projectileComponent.projectileData = linearProjectileData;
                            break;
                        }
                    }

                    projectileComponent.projectileData.impactMask = 1 << PhysicsLayer.WALL |
                                                                    1 << (weaponOwnerType == WeaponOwnerType.Player
                                                                        ? PhysicsLayer.ENEMY
                                                                        : PhysicsLayer.PLAYER);

                    PrefabUtility.SaveAsPrefabAsset(projectilePrefab, $"{prefabPath}/projectile_{weaponName}.prefab");
                    PrefabUtility.SaveAsPrefabAsset(weaponPrefab, $"{prefabPath}/weapon_{weaponName}.prefab");

                    rangedWeaponData.projectilePrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{prefabPath}/projectile_{weaponName}.prefab");
                    rangedWeaponData.weaponPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{prefabPath}/weapon_{weaponName}.prefab");

                    AssetDatabase.CreateAsset(rangedWeaponData, $"{prefabPath}/weapon_{weaponName}.asset");

                    projectileComponent.rangedWeaponData = rangedWeaponData;

                    PrefabUtility.SaveAsPrefabAsset(projectilePrefab, $"{prefabPath}/projectile_{weaponName}.prefab");

                    Object.DestroyImmediate(projectilePrefab);
                    break;
                }
                case WeaponType.Melee:
                {
                    var meleeWeaponData = ScriptableObject.CreateInstance<MeleeWeaponData>();
                    weaponData = meleeWeaponData;
                    meleeWeaponData.weaponPrefab = weaponPrefab;

                    var weaponHitbox = weaponPrefab.AddComponent<BoxCollider2D>();
                    weaponHitbox.isTrigger = true;

                    PrefabUtility.SaveAsPrefabAsset(weaponPrefab, $"{prefabPath}/weapon_{weaponName}.prefab");

                    meleeWeaponData.weaponPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{prefabPath}/weapon_{weaponName}.prefab");

                    AssetDatabase.CreateAsset(meleeWeaponData, $"{prefabPath}/weapon_{weaponName}.asset");
                    break;
                }
                case WeaponType.Conjuring:
                {
                    var conjuringWeaponData = ScriptableObject.CreateInstance<ConjuringWeaponData>();
                    weaponData = conjuringWeaponData;
                    conjuringWeaponData.weaponPrefab = weaponPrefab;

                    weaponPrefab.AddComponent<SpawnerComponent>();

                    PrefabUtility.SaveAsPrefabAsset(weaponPrefab, $"{prefabPath}/weapon_{weaponName}.prefab");

                    conjuringWeaponData.weaponPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{prefabPath}/weapon_{weaponName}.prefab");

                    AssetDatabase.CreateAsset(conjuringWeaponData, $"{prefabPath}/weapon_{weaponName}.asset");
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(weaponType), weaponType, null);
            }

            //create dropped weapon prefab
            if (weaponOwnerType == WeaponOwnerType.Player)
            {
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

                weaponData.droppedWeaponPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{prefabPath}/weapon_{weaponName}_dropped.prefab");

                Object.DestroyImmediate(droppedWeaponPrefab);
            }

            AssetDatabase.SaveAssets();

            Object.DestroyImmediate(weaponPrefab);

            return weaponData;
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