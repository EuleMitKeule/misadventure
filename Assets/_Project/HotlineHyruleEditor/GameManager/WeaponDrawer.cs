using HotlineHyrule.Level;
using HotlineHyrule.Weapons;
using Sirenix.OdinInspector;
using UnityEditor;

namespace HotlineHyruleEditor.GameManager
{
    public class WeaponDrawer : ScriptableObjectDrawer<WeaponData>
    {
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
            }
        }

        public override void SetPath(string newPath) { }
    }
}