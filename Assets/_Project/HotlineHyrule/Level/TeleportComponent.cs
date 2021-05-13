using UnityEngine;

namespace HotlineHyrule.Level
{
    public class TeleportComponent : MonoBehaviour
    {
        void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.layer.IsPlayer()) return;

            Locator.GameComponent.LoadNextScene();
        }
    }
}