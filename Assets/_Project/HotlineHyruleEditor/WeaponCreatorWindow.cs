// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text.RegularExpressions;
// using HotlineHyrule;
// using HotlineHyrule.Entities;
// using HotlineHyrule.Items;
// using HotlineHyrule.Weapons;
// using HotlineHyrule.Weapons.Projectiles;
// using Unity.Collections.LowLevel.Unsafe;
// using UnityEditor;
// using UnityEngine;
//
// namespace HotlineHyruleEditor
// {
//     public class WeaponCreatorWindow : EditorWindow
//     {
//         enum EntityType { Player, Enemy }
//         enum WeaponType { Melee, Ranged, Conjuring }
//         enum ProjectileType { Linear, Curved }
//         static string Name { get; set; }
//         static EntityType SelectedEntityType { get; set; }
//         static WeaponType SelectedWeaponType { get; set; }
//         static ProjectileType SelectedProjectileType { get; set; }
//         static bool IsPlayer => SelectedEntityType == EntityType.Player;
//         static bool IsRanged => SelectedWeaponType == WeaponType.Ranged;
//         static bool IsMelee => SelectedWeaponType == WeaponType.Melee;
//         static bool IsConjuring => SelectedWeaponType == WeaponType.Conjuring;
//         static bool IsCurved => SelectedProjectileType == ProjectileType.Curved;
//
//         [MenuItem("Window/Weapon Creator")]
//         public static void ShowWindow()
//         {
//             GetWindow<WeaponCreatorWindow>("Weapon Creator");
//         }
//
//         void OnGUI()
//         {
//             Name = EditorGUILayout.TextField("Name", Name);
//
//             SelectedEntityType = (EntityType)EditorGUILayout.EnumPopup(SelectedEntityType);
//             SelectedWeaponType = (WeaponType)EditorGUILayout.EnumPopup(SelectedWeaponType);
//             if (IsRanged) SelectedProjectileType = (ProjectileType)EditorGUILayout.EnumPopup(SelectedProjectileType);
//
//             if (GUILayout.Button("Generate")) GenerateWeapon();
//         }
//
//         static void GenerateWeapon()
//         {
//             if (Name == null || !Regex.IsMatch(Name, @"^([a-z])+(_([a-z])+)*$"))
//             {
//                 Debug.LogError("Weapon name is invalid. Use only lower-case letters and underscores.");
//                 return;
//             }
//
//             var animationParentPath = $"Assets/_Project/Graphics/Animations/Weapons/{(IsPlayer ? "Player" : "Enemy")}";
//             var prefabParentPath = $"Assets/_Project/Prefabs/Weapons/{(IsPlayer ? "Player" : "Enemy")}";
//             var animationPath = $"{animationParentPath}/{Name}";
//             var prefabPath = $"{prefabParentPath}/{Name}";
//
//             AssetDatabase.CreateFolder(animationParentPath, Name);
//             AssetDatabase.CreateFolder(prefabParentPath, Name);
//
//             //create weapon animator controller
//             var weaponBaseController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/_Project/Graphics/Animations/Weapons/base/controller_weapon_base.controller");
//             var weaponBaseControllerIdle = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/_project/Graphics/Animations/Weapons/base/animation_weapon_base_idle.anim");
//             var weaponBaseControllerAttack = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/_project/Graphics/Animations/Weapons/base/animation_weapon_base_attack.anim");
//             var weaponBaseControllerImpact = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/_project/Graphics/Animations/Weapons/base/animation_weapon_base_impact.anim");
//
//             var weaponAnimationIdle = new AnimationClip();
//             var weaponAnimationAttack = new AnimationClip();
//             var weaponAnimationImpact = new AnimationClip();
//
//             AssetDatabase.CreateAsset(weaponAnimationIdle, $"{animationPath}/animation_weapon_{Name}_idle.anim");
//             AssetDatabase.CreateAsset(weaponAnimationAttack, $"{animationPath}/animation_weapon_{Name}_attack.anim");
//             AssetDatabase.CreateAsset(weaponAnimationImpact, $"{animationPath}/animation_weapon_{Name}_impact.anim");
//
//             var weaponController = new AnimatorOverrideController(weaponBaseController);
//             var weaponOverrides = new List<KeyValuePair<AnimationClip, AnimationClip>>
//             {
//                 new KeyValuePair<AnimationClip, AnimationClip>(weaponBaseControllerIdle, weaponAnimationIdle),
//                 new KeyValuePair<AnimationClip, AnimationClip>(weaponBaseControllerAttack, weaponAnimationAttack),
//                 new KeyValuePair<AnimationClip, AnimationClip>(weaponBaseControllerImpact, weaponAnimationImpact)
//             };
//
//             weaponController.ApplyOverrides(weaponOverrides);
//             AssetDatabase.CreateAsset(weaponController, $"{animationPath}/controller_weapon_{Name}.controller");
//
//             //create weapon prefab
//             var weaponPrefab = new GameObject($"weapon_{Name}");
//             weaponPrefab.layer = IsPlayer ? PhysicsLayer.PLAYER : PhysicsLayer.ENEMY;
//
//             var weaponSpriteRenderer = weaponPrefab.AddComponent<SpriteRenderer>();
//             weaponSpriteRenderer.sortingLayerName = IsPlayer ? "player" : "enemy";
//             weaponSpriteRenderer.sortingOrder = -1;
//
//             var weaponAnimator = weaponPrefab.AddComponent<Animator>();
//             weaponAnimator.runtimeAnimatorController = weaponController;
//
//             weaponPrefab.AddComponent<WeaponAnimationComponent>();
//
//             var weaponData = CreateInstance<WeaponData>();
//
//             //create weapon data
//             if (IsRanged)
//             {
//                 var rangedWeaponData = CreateInstance<RangedWeaponData>();
//                 weaponData = rangedWeaponData;
//                 rangedWeaponData.weaponPrefab = weaponPrefab;
//
//                 //create projectile animator controller
//                 var projectileBaseController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/_Project/Graphics/Animations/Weapons/base/controller_projectile_base.controller");
//                 var projectileBaseControllerIdle = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/_project/Graphics/Animations/Weapons/base/animation_projectile_base_idle.anim");
//                 var projectileBaseControllerAttack = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/_project/Graphics/Animations/Weapons/base/animation_projectile_base_attack.anim");
//                 var projectileBaseControllerImpact = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/_project/Graphics/Animations/Weapons/base/animation_projectile_base_impact.anim");
//                 var projectileBaseControllerStop = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/_project/Graphics/Animations/Weapons/base/animation_projectile_base_stop.anim");
//
//                 var projectileAnimationIdle = new AnimationClip();
//                 var projectileAnimationAttack = new AnimationClip();
//                 var projectileAnimationImpact = new AnimationClip();
//                 var projectileAnimationStop = new AnimationClip();
//
//                 AssetDatabase.CreateAsset(projectileAnimationIdle, $"{animationPath}/animation_projectile_{Name}_idle.anim");
//                 AssetDatabase.CreateAsset(projectileAnimationAttack, $"{animationPath}/animation_projectile_{Name}_attack.anim");
//                 AssetDatabase.CreateAsset(projectileAnimationImpact, $"{animationPath}/animation_projectile_{Name}_impact.anim");
//                 AssetDatabase.CreateAsset(projectileAnimationStop, $"{animationPath}/animation_projectile_{Name}_stop.anim");
//
//                 var projectileController = new AnimatorOverrideController(projectileBaseController);
//                 var projectileOverrides = new List<KeyValuePair<AnimationClip, AnimationClip>>
//                 {
//                     new KeyValuePair<AnimationClip, AnimationClip>(projectileBaseControllerIdle, projectileAnimationIdle),
//                     new KeyValuePair<AnimationClip, AnimationClip>(projectileBaseControllerAttack, projectileAnimationAttack),
//                     new KeyValuePair<AnimationClip, AnimationClip>(projectileBaseControllerImpact, projectileAnimationImpact),
//                     new KeyValuePair<AnimationClip, AnimationClip>(projectileBaseControllerStop, projectileAnimationStop)
//                 };
//
//                 projectileController.ApplyOverrides(projectileOverrides);
//                 AssetDatabase.CreateAsset(projectileController, $"{animationPath}/controller_projectile_{Name}.controller");
//
//                 //create projectile prefab
//                 var projectilePrefab = new GameObject($"projectile_{Name}");
//                 projectilePrefab.layer = IsPlayer ? PhysicsLayer.PROJECTILE : PhysicsLayer.ENEMY_PROJECTILE;
//
//                 var projectileSpriteRenderer = projectilePrefab.AddComponent<SpriteRenderer>();
//                 projectileSpriteRenderer.sortingLayerName = "projectile";
//
//                 var projectileAnimator = projectilePrefab.AddComponent<Animator>();
//                 projectileAnimator.runtimeAnimatorController = projectileController;
//
//                 var projectileRigidbody = projectilePrefab.AddComponent<Rigidbody2D>();
//                 projectileRigidbody.angularDrag = 0f;
//                 projectileRigidbody.gravityScale = 0f;
//                 projectileRigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
//
//                 projectilePrefab.AddComponent<CapsuleCollider2D>();
//
//                 var projectileComponent = projectilePrefab.AddComponent<ProjectileComponent>();
//
//                 //create projectile data
//                 if (IsCurved)
//                 {
//                     var curvedProjectileData = CreateInstance<CurvedProjectileData>();
//                     AssetDatabase.CreateAsset(curvedProjectileData, $"{prefabPath}/projectile_{Name}.asset");
//
//                     projectileComponent.projectileData = curvedProjectileData;
//                 }
//                 else
//                 {
//                     var linearProjectileData = CreateInstance<LinearProjectileData>();
//                     AssetDatabase.CreateAsset(linearProjectileData, $"{prefabPath}/projectile_{Name}.asset");
//
//                     projectileComponent.projectileData = linearProjectileData;
//                 }
//
//                 projectileComponent.projectileData.impactMask = 1 << PhysicsLayer.WALL |
//                                                                 1 << (IsPlayer
//                                                                     ? PhysicsLayer.ENEMY
//                                                                     : PhysicsLayer.PLAYER);
//
//                 PrefabUtility.SaveAsPrefabAsset(projectilePrefab, $"{prefabPath}/projectile_{Name}.prefab");
//                 PrefabUtility.SaveAsPrefabAsset(weaponPrefab, $"{prefabPath}/weapon_{Name}.prefab");
//
//                 rangedWeaponData.projectilePrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{prefabPath}/projectile_{Name}.prefab");
//                 rangedWeaponData.weaponPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{prefabPath}/weapon_{Name}.prefab");
//
//                 AssetDatabase.CreateAsset(rangedWeaponData, $"{prefabPath}/weapon_{Name}.asset");
//
//                 projectileComponent.rangedWeaponData = rangedWeaponData;
//
//                 PrefabUtility.SaveAsPrefabAsset(projectilePrefab, $"{prefabPath}/projectile_{Name}.prefab");
//
//                 DestroyImmediate(projectilePrefab);
//             }
//             else if (IsMelee)
//             {
//                 var meleeWeaponData = CreateInstance<MeleeWeaponData>();
//                 weaponData = meleeWeaponData;
//                 meleeWeaponData.weaponPrefab = weaponPrefab;
//
//                 var weaponHitbox = weaponPrefab.AddComponent<BoxCollider2D>();
//                 weaponHitbox.isTrigger = true;
//
//                 PrefabUtility.SaveAsPrefabAsset(weaponPrefab, $"{prefabPath}/weapon_{Name}.prefab");
//
//                 meleeWeaponData.weaponPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{prefabPath}/weapon_{Name}.prefab");
//
//                 AssetDatabase.CreateAsset(meleeWeaponData, $"{prefabPath}/weapon_{Name}.asset");
//             }
//             else if (IsConjuring)
//             {
//                 var conjuringWeaponData = CreateInstance<ConjuringWeaponData>();
//                 weaponData = conjuringWeaponData;
//                 conjuringWeaponData.weaponPrefab = weaponPrefab;
//
//                 weaponPrefab.AddComponent<SpawnerComponent>();
//
//                 PrefabUtility.SaveAsPrefabAsset(weaponPrefab, $"{prefabPath}/weapon_{Name}.prefab");
//
//                 conjuringWeaponData.weaponPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{prefabPath}/weapon_{Name}.prefab");
//
//                 AssetDatabase.CreateAsset(conjuringWeaponData, $"{prefabPath}/weapon_{Name}.asset");
//             }
//
//             //create dropped weapon prefab
//             if (IsPlayer)
//             {
//                 var droppedWeaponPrefab = new GameObject($"weapon_{Name}_dropped");
//                 droppedWeaponPrefab.layer = PhysicsLayer.ITEM;
//
//                 var droppedWeaponSpriteRenderer = droppedWeaponPrefab.AddComponent<SpriteRenderer>();
//                 droppedWeaponSpriteRenderer.sortingLayerName = "item";
//
//                 var droppedWeaponCollider = droppedWeaponPrefab.AddComponent<CircleCollider2D>();
//                 droppedWeaponCollider.isTrigger = true;
//
//                 var droppedWeaponItemComponent = droppedWeaponPrefab.AddComponent<ItemComponent>();
//
//                 droppedWeaponPrefab.AddComponent<DroppedWeaponComponent>();
//
//                 droppedWeaponItemComponent.itemDatas.Add(weaponData);
//
//                 PrefabUtility.SaveAsPrefabAsset(droppedWeaponPrefab, $"{prefabPath}/weapon_{Name}_dropped.prefab");
//
//                 weaponData.droppedWeaponPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{prefabPath}/weapon_{Name}_dropped.prefab");
//
//                 DestroyImmediate(droppedWeaponPrefab);
//             }
//
//             AssetDatabase.SaveAssets();
//
//             DestroyImmediate(weaponPrefab);
//         }
//     }
// }