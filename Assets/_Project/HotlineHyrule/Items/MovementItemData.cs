using UnityEngine;

namespace HotlineHyrule.Items
{
    [CreateAssetMenu(menuName = "Item/New Movement Item")]
    public class MovementItemData : ItemData
    {
        [SerializeField] public float movementFactor;
        [SerializeField] public int duration;
    }
}