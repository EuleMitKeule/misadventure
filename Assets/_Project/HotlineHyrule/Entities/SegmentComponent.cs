using HotlineHyrule.Extensions;
using UnityEngine;

namespace HotlineHyrule.Entities
{
    public class SegmentComponent : MonoBehaviour
    {
        [SerializeField] bool isHead;
        [SerializeField] Transform parentSegment;
        [SerializeField] float followDamping;
        [SerializeField] float rotationDamping;
        [SerializeField] float movementThreshold;
        [SerializeField] float minDistance;


        public Vector3 LastPosition { get; set; }
        Vector3? TargetPosition { get; set; }
        public Quaternion? TargetRotation { get; set; }
        public float TraveledDistance { get; set; }

        Rigidbody2D Rigidbody { get; set; }
        SegmentComponent ParentSegmentComponent { get; set; }

        void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            LastPosition = transform.position;

            SetParentSegment(parentSegment);
        }

        void SetParentSegment(Transform newParentSegment)
        {
            parentSegment = newParentSegment;

            if (parentSegment)
            {
                ParentSegmentComponent = parentSegment.GetComponent<SegmentComponent>();
            }
        }

        void FixedUpdate()
        {
            TraveledDistance = LastPosition.DistanceTo(transform.position);

            if (TargetRotation.HasValue)
            {
                transform.rotation =
                    Quaternion.Lerp(transform.rotation, TargetRotation.Value, Time.fixedDeltaTime / rotationDamping);
            }

            if (TargetPosition.HasValue)
            {
                transform.position =
                    Vector3.Lerp(transform.position, TargetPosition.Value, Time.fixedDeltaTime / followDamping);
            }

            if (parentSegment)
            {
                var targetDirection = parentSegment.up;
                TargetPosition = parentSegment.position - targetDirection * minDistance;

                TargetRotation = ParentSegmentComponent.TargetRotation;
            }

            if (isHead)
            {
                var traveledDirection = LastPosition.DirectionTo(transform.position);

                if (traveledDirection != Vector3.zero && TraveledDistance >= movementThreshold)
                {
                    var lookAngle = Vector3.SignedAngle(Vector3.up, traveledDirection, Vector3.forward);
                    TargetRotation = Quaternion.Euler(0f, 0f, lookAngle);
                }
            }

            LastPosition = transform.position;
        }
    }
}
