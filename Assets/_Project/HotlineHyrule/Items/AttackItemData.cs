using UnityEngine;

namespace HotlineHyrule.Items
{
    [CreateAssetMenu(menuName = "Item/New Attack Item")]
    public class AttackItemData : ItemData
    {
        [SerializeField] public float damageFactor;
        [SerializeField] public int damageBonus;
        [SerializeField] public float attackSpeed;
        [SerializeField] public int duration;
    }
}