using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace HotlineHyruleEditor
{
    public static class EnemyBuilder
    {
        public static string Path => "Assets/_Project/Prefabs/Entities/Enemies";
        public static string AnimationPath => "Assets/_Project/Graphics/Animations/Enemies";

        public static GameObject Create(string enemyName)
        {
            if (enemyName == null || !Regex.IsMatch(enemyName, @"^([a-z])+(_([a-z])+)*$"))
            { 
                Debug.LogError("Enemy name is invalid. Use only lower-case letters and underscores.");
                return null;
            }

            var enemyBase = AssetDatabase.LoadAssetAtPath<GameObject>($"{Path}/enemy_base.prefab");
            if (!enemyBase) return null;

            var animationPath = $"{AnimationPath}/{enemyName}";
            AssetDatabase.CreateFolder(AnimationPath, enemyName);
            
            var baseController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>($"{AnimationPath}/base/controller_enemy_base.controller");
            var baseIdleAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>($"{AnimationPath}/base/animation_enemy_base_idle.anim");
            var baseAttackAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>($"{AnimationPath}/base/animation_enemy_base_attack.anim");
            var baseAltAttackAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>($"{AnimationPath}/base/animation_enemy_base_attack_alt.anim");
            var baseMovingAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>($"{AnimationPath}/base/animation_enemy_base_moving.anim");
            var baseDyingAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>($"{AnimationPath}/base/animation_enemy_base_dying.anim");
            
            var controller = new AnimatorOverrideController(baseController);
            var idleAnim = new AnimationClip();
            var attackAnim = new AnimationClip();
            var altAttackAnim = new AnimationClip();
            var movingAnim = new AnimationClip();
            var dyingAnim = new AnimationClip();
            
            AssetDatabase.CreateAsset(idleAnim, $"{animationPath}/animation_{enemyName}_idle.anim");
            AssetDatabase.CreateAsset(attackAnim, $"{animationPath}/animation_{enemyName}_attack.anim");
            AssetDatabase.CreateAsset(altAttackAnim, $"{animationPath}/animation_{enemyName}_attack_alt.anim");
            AssetDatabase.CreateAsset(movingAnim, $"{animationPath}/animation_{enemyName}_moving.anim");
            AssetDatabase.CreateAsset(dyingAnim, $"{animationPath}/animation_{enemyName}_dying.anim");
            
            var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>
            {
                new KeyValuePair<AnimationClip, AnimationClip>(baseIdleAnim, idleAnim),
                new KeyValuePair<AnimationClip, AnimationClip>(baseAttackAnim, attackAnim),
                new KeyValuePair<AnimationClip, AnimationClip>(baseAltAttackAnim, altAttackAnim),
                new KeyValuePair<AnimationClip, AnimationClip>(baseMovingAnim, movingAnim),
                new KeyValuePair<AnimationClip, AnimationClip>(baseDyingAnim, dyingAnim),
            };
            
            controller.ApplyOverrides(overrides);
            AssetDatabase.CreateAsset(controller, $"{animationPath}/controller_{enemyName}.controller");

            var enemySource = (GameObject)PrefabUtility.InstantiatePrefab(enemyBase);
            var enemy = PrefabUtility.SaveAsPrefabAsset(enemySource, $"{Path}/{enemyName}.prefab");
            
            Object.DestroyImmediate(enemySource);
            
            var enemyAnimator = enemy.GetComponent<Animator>();
            enemyAnimator.runtimeAnimatorController = controller;

            AssetDatabase.SaveAssets();
            return enemy;
        }

        public static GameObject Rename(GameObject enemy, string newName)
        {
            if (newName == null || !Regex.IsMatch(newName, @"^([a-z])+(_([a-z])+)*$"))
            { 
                Debug.LogError("Enemy name is invalid. Use only lower-case letters and underscores.");
                return null;
            }
            
            var path = AssetDatabase.GetAssetPath(enemy);
            var oldName = enemy.name;
            
            AssetDatabase.RenameAsset(path, newName);
            AssetDatabase.MoveAsset($"{Path}/{oldName}", $"{Path}/{newName}");

            var animationPath = $"{AnimationPath}/{oldName}";
            var controllerPath = $"{animationPath}/controller_{oldName}.controller";
            var idleAnimPath = $"{animationPath}/animation_{oldName}_idle.anim";
            var attackAnimPath = $"{animationPath}/animation_{oldName}_attack.anim";
            var attackAltAnimPath = $"{animationPath}/animation_{oldName}_attack_alt.anim";
            var movingAnimPath = $"{animationPath}/animation_{oldName}_moving.anim";
            var dyingAnimPath = $"{animationPath}/animation_{oldName}_dying.anim";
            
            var controllerName = $"controller_{newName}";
            var idleAnimName = $"animation_{newName}_idle";
            var attackAnimName = $"animation_{newName}_attack";
            var attackAltAnimName = $"animation_{newName}_attack_alt";
            var movingAnimName = $"animation_{newName}_moving";
            var dyingAnimName = $"animation_{newName}_dying";

            AssetDatabase.RenameAsset(controllerPath, controllerName);
            AssetDatabase.RenameAsset(idleAnimPath, idleAnimName);
            AssetDatabase.RenameAsset(attackAnimPath, attackAnimName);
            AssetDatabase.RenameAsset(attackAltAnimPath, attackAltAnimName);
            AssetDatabase.RenameAsset(movingAnimPath, movingAnimName);
            AssetDatabase.RenameAsset(dyingAnimPath, dyingAnimName);
            AssetDatabase.MoveAsset(animationPath, $"{AnimationPath}/{newName}");
            
            AssetDatabase.SaveAssets();
            return enemy;
        }

        public static void Delete(GameObject enemy)
        {
            var name = enemy.name;
            AssetDatabase.DeleteAsset($"{Path}/{name}.prefab");

            var animationPath = $"{AnimationPath}/{name}";
            AssetDatabase.DeleteAsset(animationPath);
            
            AssetDatabase.SaveAssets();
        }

        public static AnimationClip GetIdleAnimation(GameObject enemy)
        {
            var animationPath = $"{AnimationPath}/{enemy.name}/animation_{enemy.name}_idle.anim";
            var anim = AssetDatabase.LoadAssetAtPath<AnimationClip>(animationPath);
            
            return anim;
        }

        public static AnimationClip GetAttackAnimation(GameObject enemy)
        {
            var animationPath = $"{AnimationPath}/{enemy.name}/animation_{enemy.name}_attack.anim";
            var anim = AssetDatabase.LoadAssetAtPath<AnimationClip>(animationPath);
            
            return anim;
        }

        public static AnimationClip GetAttackAltAnimation(GameObject enemy)
        {
            var animationPath = $"{AnimationPath}/{enemy.name}/animation_{enemy.name}_attack_alt.anim";
            var anim = AssetDatabase.LoadAssetAtPath<AnimationClip>(animationPath);
            
            return anim;
        }

        public static AnimationClip GetMovingAnimation(GameObject enemy)
        {
            var animationPath = $"{AnimationPath}/{enemy.name}/animation_{enemy.name}_moving.anim";
            var anim = AssetDatabase.LoadAssetAtPath<AnimationClip>(animationPath);
            
            return anim;
        }

        public static AnimationClip GetDyingAnimation(GameObject enemy)
        {
            var animationPath = $"{AnimationPath}/{enemy.name}/animation_{enemy.name}_dying.anim";
            var anim = AssetDatabase.LoadAssetAtPath<AnimationClip>(animationPath);
            
            return anim;
        }
    }
}