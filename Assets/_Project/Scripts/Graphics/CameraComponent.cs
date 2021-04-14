using UnityEngine;

namespace Scripts.Graphics
{
    public class CameraComponent : MonoBehaviour
    {
        [Range(0, 1)] [SerializeField] float _followDamping;

        Transform Transform { get; set; }
        Camera Camera { get; set; }

        void Awake()
        {
            Transform = transform;
            Camera = GetComponent<UnityEngine.Camera>();

            Locator.CameraComponent = this;
        }

        void LateUpdate()
        {
            var targetPosition = (Vector2)Locator.PlayerComponent.transform.position;

            targetPosition = Vector2.Lerp(transform.position, targetPosition, Time.deltaTime * 1 / _followDamping);

            Transform.position = new Vector3(targetPosition.x, targetPosition.y, Transform.position.z);
        }
    }
}
