using UnityEngine;

namespace HotlineHyrule.Weapons
{
    [CreateAssetMenu(menuName = "Weapons/New Conjuring Weapon")]
    public class ConjuringWeaponData : WeaponData
    {
        [SerializeField] public int conjuringRadius;
        [SerializeField] public int conjuringAmount;
        [Range(0f, 1f)] [SerializeField] public float conjuringAmountOffset;
        [SerializeField] public string spawnTilemapName;
    }
}