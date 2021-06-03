using System;
using System.Collections.Generic;
using HotlineHyrule.Extensions;
using UnityEngine;

namespace HotlineHyrule.Entities
{
    // public class SegmentComponent : MonoBehaviour
    // {
    //     [SerializeField] bool isHead;
    //     [SerializeField] Transform parentSegment;
    //     [SerializeField] float followDamping;
    //     [SerializeField] float rotationDamping;
    //     [SerializeField] float movementThreshold;
    //     [SerializeField] float minDistance;
    //
    //
    //     public Vector3 LastPosition { get; set; }
    //     Vector3? TargetPosition { get; set; }
    //     public Quaternion? TargetRotation { get; set; }
    //     public float TraveledDistance { get; set; }
    //
    //     Rigidbody2D Rigidbody { get; set; }
    //     SegmentComponent ParentSegmentComponent { get; set; }
    //
    //     void Awake()
    //     {
    //         Rigidbody = GetComponent<Rigidbody2D>();
    //         LastPosition = transform.position;
    //
    //         SetParentSegment(parentSegment);
    //     }
    //
    //     void SetParentSegment(Transform newParentSegment)
    //     {
    //         parentSegment = newParentSegment;
    //
    //         if (parentSegment)
    //         {
    //             ParentSegmentComponent = parentSegment.GetComponent<SegmentComponent>();
    //         }
    //     }
    //
    //     void FixedUpdate()
    //     {
    //         TraveledDistance = LastPosition.DistanceTo(transform.position);
    //
    //         if (TargetRotation.HasValue)
    //         {
    //             transform.rotation =
    //                 Quaternion.Lerp(transform.rotation, TargetRotation.Value, Time.fixedDeltaTime / rotationDamping);
    //         }
    //
    //         if (TargetPosition.HasValue)
    //         {
    //             transform.position =
    //                 Vector3.Lerp(transform.position, TargetPosition.Value, Time.fixedDeltaTime / followDamping);
    //         }
    //
    //         if (parentSegment)
    //         {
    //             var targetDirection = parentSegment.up;
    //             TargetPosition = parentSegment.position - targetDirection * minDistance;
    //
    //             TargetRotation = ParentSegmentComponent.TargetRotation;
    //         }
    //
    //         if (isHead)
    //         {
    //             var traveledDirection = LastPosition.DirectionTo(transform.position);
    //
    //             if (traveledDirection != Vector3.zero && TraveledDistance >= movementThreshold)
    //             {
    //                 var lookAngle = Vector3.SignedAngle(Vector3.up, traveledDirection, Vector3.forward);
    //                 TargetRotation = Quaternion.Euler(0f, 0f, lookAngle);
    //             }
    //         }
    //
    //         LastPosition = transform.position;
    //     }
    // }

    public class SegmentComponent : MonoBehaviour
    {
        [SerializeField] float nodeDistance;
        [SerializeField] float segmentDistance;
        [SerializeField] SegmentComponent parentSegment;
        [SerializeField] SegmentComponent childSegment;
        
        LinkedList<CaterpillarNode> Nodes { get; } = new LinkedList<CaterpillarNode>();
        LinkedListNode<CaterpillarNode> TargetNode { get; set; }
        LinkedListNode<CaterpillarNode> LastNode { get; set; }
        public SegmentComponent ParentSegment { get; private set; }
        public SegmentComponent ChildSegment { get; private set; }
        SegmentComponent HeadSegment { get; set; }
        public bool IsHead => !ParentSegment;
        bool IsTail => !ChildSegment;
        int MinNodeDifference => Mathf.RoundToInt(segmentDistance / nodeDistance);
        float TraveledDistance => transform.position.DistanceTo(LastNode.Value.Position);
        float TraveledDistancePerNode => TraveledDistance / nodeDistance;
        event EventHandler<NodeEventArgs> TargetNodeReached;

        void Awake()
        {
            SetParentSegment(parentSegment);
            SetChildSegment(childSegment);
        }

        void Start()
        {
            HeadSegment = GetHeadSegment();
            
            if (!IsHead) return;
            
            AddNode();
        }

        void FixedUpdate()
        {
            if (IsHead)
            {
                if (TraveledDistance < nodeDistance) return;

                AddNode();
                return;
            }

            if (TargetNode == null || LastNode == null) return;
            
            var position = Vector2.Lerp(LastNode.Value.Position, TargetNode.Value.Position,
                HeadSegment.TraveledDistancePerNode);
            transform.position = position;
        }

        void SetParentSegment(SegmentComponent segment)
        {
            if (!segment) return;
            if (ParentSegment) ParentSegment.TargetNodeReached -= OnParentTargetNodeReached;
            
            ParentSegment = segment;
            ParentSegment.TargetNodeReached += OnParentTargetNodeReached;
        }

        void SetChildSegment(SegmentComponent segment) => ChildSegment = segment;

        SegmentComponent GetHeadSegment()
        {
            var currentSegment = this;

            while (currentSegment.ParentSegment)
            {
                currentSegment = currentSegment.ParentSegment;
            }

            return currentSegment;
        }

        void OnParentTargetNodeReached(object sender, NodeEventArgs e)
        {
            if (TargetNode == null)
            {
                var currentNode = e.Node;

                while (currentNode!.Previous != null)
                {
                    currentNode = e.Node.Previous;
                }

                TargetNode = currentNode;
            }

            var difference = e.Node.Value.Index - TargetNode.Value.Index;

            if (difference < MinNodeDifference) return;
            
            LastNode = TargetNode;
            TargetNode = TargetNode.Next ?? TargetNode;

            TargetNodeReached?.Invoke(this, new NodeEventArgs(LastNode));
        }

        void AddNode()
        {
            var node = new CaterpillarNode(Nodes.Count, transform.position, transform.eulerAngles.z);
            var linkedNode = Nodes.AddLast(node);
            
            LastNode = linkedNode;
            
            TargetNodeReached?.Invoke(this, new NodeEventArgs(LastNode));
        }
    }
}
