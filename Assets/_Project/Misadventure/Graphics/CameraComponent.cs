using UnityEngine;

namespace Misadventure.Graphics
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

        /// <summary>
        /// The player's current position.
        /// </summary>
        Vector2 TargetPosition => Locator.PlayerComponent.transform.position;
        /// <summary>
        /// The next smoothed transition position the camera should apply.
        /// </summary>
        Vector2 SmoothedTargetPosition =>
            Vector2.Lerp(transform.position, TargetPosition, Time.deltaTime * 1 / followDamping);
        /// <summary>
        /// The smoothed target position on the camera's plane.
        /// </summary>
        Vector3 SmoothedCameraPosition =>
            new Vector3(SmoothedTargetPosition.x, SmoothedTargetPosition.y, transform.position.z);

        Camera Camera { get; set; }

        void Awake()
        {
            Camera = GetComponent<Camera>();

            Locator.CameraComponent = this;
        }

        void LateUpdate()
        {
            transform.position = SmoothedCameraPosition;
        }
    }
}
