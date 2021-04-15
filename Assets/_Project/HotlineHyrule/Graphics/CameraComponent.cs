using UnityEngine;

namespace HotlineHyrule.Graphics
{
    /// <summary>
    /// Handles the movement behavior of the camera it's attached to.
    /// </summary>
    public class CameraComponent : MonoBehaviour
    {
        /// <summary>
        /// The strength of the damping applied to the camera's movement.
        /// </summary>
        [Range(0, 1)] [SerializeField] float followDamping;

        Transform Transform { get; set; }
        Camera Camera { get; set; }

        void Awake()
        {
            Transform = transform;
            Camera = GetComponent<Camera>();

            Locator.CameraComponent = this;
        }

        void LateUpdate()
        {
            var targetPosition = (Vector2)Locator.PlayerComponent.transform.position;

            targetPosition = Vector2.Lerp(transform.position, targetPosition, Time.deltaTime * 1 / followDamping);

            Transform.position = new Vector3(targetPosition.x, targetPosition.y, Transform.position.z);
        }
    }
}
