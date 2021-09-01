using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace MisadventureEditor.GameManager
{
    public class EnemyDrawer
    {
        [LabelWidth(50)]
        [PropertyOrder(-1)]
        [LabelText("Name")]
        [PropertySpace(5f)]
        [TitleGroup("Tools")]
        [HorizontalGroup("Tools/Main")]
        [VerticalGroup("Tools/Main/Vertical")]
        [BoxGroup("Tools/Main/Vertical/Create")]
        [ShowInInspector]
        string NameForNew { get; set; }

        [HideLabel]
        [PropertySpace]
        [HorizontalGroup("Tools/Main/Vertical/General/Rename", Order = 2)]
        [PropertyOrder(0)]
        [InlineButton("RenameSelected", "Rename Object")]
        [ShowInInspector]
        [GUIColor(0.65f, 0.65f, 1f)]
        string RenameName { get; set; }

        [PropertySpace(5)]
        [PropertyOrder(-1)]
        [BoxGroup("Tools/Main/Vertical/General")]
        [Button]
        void Select() => Selection.activeObject = Selected;
        
        [PropertySpace(10f)]
        [PropertyOrder(1)]
        [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
        [ShowInInspector]
        [TitleGroup("Tools/Main/Vertical/Settings")]
        GameObject Selected { get; set; }
        
        [PropertyOrder(0)]
        [BoxGroup("Tools/Main/Vertical/Create")]
        [PropertySpace(5, 5)]
        [Button]
        [GUIColor(0, 0.9f, 0)]
        public void CreateNew()
        {
            var enemy = EnemyBuilder.Create(NameForNew);
            SetSelected(enemy);
        }

        public void RenameSelected()
        {
            var enemy = EnemyBuilder.Rename(Selected, RenameName);
            SetSelected(enemy);
        }

        [LabelText("Delete Object")]
        [PropertyOrder(2)]
        [BoxGroup("Tools/Main/Vertical/General")]
        [Button]
        [GUIColor(1, 0.2f, 0)]
        [PropertySpace(SpaceAfter = 5)]
        public void DeleteSelected()
        {
            if (!Selected) return;

            var message =
                $"Are you sure you want to delete the item \"{Selected.name}\"?\nThis will also delete the prefab file.";
            var isSure = EditorUtility.DisplayDialog("Delete Item", message, "Yes", "Cancel");

            if (!isSure) return;
            
            EnemyBuilder.Delete(Selected);
        }

        [LabelText("Idle")]
        [ShowIf("@IdleAnimation != null")]
        [BoxGroup("Tools/Main/Animation")]
        [PropertySpace(5, 5)]
        [PropertyOrder(2)]
        [Button]
        void SelectIdleAnimation() => SelectAnimation(IdleAnimation);
        AnimationClip IdleAnimation => Selected ? EnemyBuilder.GetIdleAnimation(Selected) : null;

        [LabelText("Attack")]
        [ShowIf("@AttackAnimation != null")]
        [BoxGroup("Tools/Main/Animation")]
        [PropertySpace(5, 5)]
        [PropertyOrder(2)]
        [Button]
        void SelectAttackAnimation() => SelectAnimation(AttackAnimation);
        AnimationClip AttackAnimation => Selected ? EnemyBuilder.GetAttackAnimation(Selected) : null;

        [LabelText("AttackAlt")]
        [ShowIf("@AttackAltAnimation != null")]
        [BoxGroup("Tools/Main/Animation")]
        [PropertySpace(5, 5)]
        [PropertyOrder(2)]
        [Button]
        void SelectAttackAltAnimation() => SelectAnimation(AttackAltAnimation);
        AnimationClip AttackAltAnimation => Selected ? EnemyBuilder.GetAttackAltAnimation(Selected) : null;

        [LabelText("Moving")]
        [ShowIf("@MovingAnimation != null")]
        [BoxGroup("Tools/Main/Animation")]
        [PropertySpace(5, 5)]
        [PropertyOrder(2)]
        [Button]
        void SelectMovingAnimation() => SelectAnimation(MovingAnimation);
        AnimationClip MovingAnimation => Selected ? EnemyBuilder.GetMovingAnimation(Selected) : null;

        [LabelText("Dying")]
        [ShowIf("@DyingAnimation != null")]
        [BoxGroup("Tools/Main/Animation")]
        [PropertySpace(5, 5)]
        [PropertyOrder(2)]
        [Button]
        void SelectDyingAnimation() => SelectAnimation(DyingAnimation);
        AnimationClip DyingAnimation => Selected ? EnemyBuilder.GetDyingAnimation(Selected) : null;

        public void SetSelected(object item)
        {
            var attempt = item as GameObject;
            if (!attempt) return;

            Selected = attempt;

            NameForNew = attempt.name;
            RenameName = attempt.name;
        }

        static void SelectAnimation(AnimationClip animation)
        {
            Selection.activeObject = animation;
            EditorApplication.ExecuteMenuItem("Window/Animation/Animation");
        }
    }
}