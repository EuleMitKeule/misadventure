using UnityEngine;

namespace HotlineHyrule.Items
{
    [CreateAssetMenu(menuName = "Item/New Health Item")]
    public class HealthItemData : ItemData
    {
        [SerializeField] public int healTotal;
        [SerializeField] public int healAmount;
        [SerializeField] public float healRate;
    }
}