using System;

namespace HotlineHyrule.Weapons
{
    public class WeaponEventArgs : EventArgs
    {
        public WeaponData Weapon { get; }

        public WeaponEventArgs(WeaponData weapon) => Weapon = weapon;
    }
}