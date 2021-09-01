using System;
using System.Collections.Generic;
using HotlineHyrule.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HotlineHyrule.Entities
{
    public class SegmentComponent : MonoBehaviour
    {
        public LinkedList<CaterpillarNode> Nodes { get; } = new LinkedList<CaterpillarNode>();
        LinkedListNode<CaterpillarNode> TargetNode { get; set; }
        LinkedListNode<CaterpillarNode> LastNode { get; set; }
        [FoldoutGroup("Debug")]
        [ShowInInspector]
        [ReadOnly]
        public SegmentComponent ParentSegment { get; set; }
        [FoldoutGroup("Debug")]
        [ShowInInspector]
        [ReadOnly]
        public SegmentComponent ChildSegment { get; set; }
        [FoldoutGroup("Debug")]
        [ShowInInspector]
        [ReadOnly]
        SegmentComponent HeadSegment { get; set; }
        public SegmentComponent Head => IsHead ? this : HeadSegment;
        Quaternion TargetHeadRotation { get; set; }
        [FoldoutGroup("Debug")]
        [ShowInInspector]
        [ReadOnly]
        public bool IsHead => !ParentSegment;
        [FoldoutGroup("Debug")]
        [ShowInInspector]
        [ReadOnly]
        public bool IsTail => !ChildSegment;
        public RaycastHit2D WallAbove => Physics2D.Raycast(
            transform.position, LookDirection, CaterpillarComponent.WallCheckDistance, CaterpillarComponent.WallMask);
        public RaycastHit2D WallLeft => Physics2D.Raycast(
            transform.position, LookDirection.Rotate(90f), CaterpillarComponent.WallCheckDistance, CaterpillarComponent.WallMask);
        public RaycastHit2D WallRight => Physics2D.Raycast(
            transform.position, LookDirection.Rotate(-90f), CaterpillarComponent.WallCheckDistance, CaterpillarComponent.WallMask);
        int MinNodeDifference => Mathf.RoundToInt(CaterpillarComponent.SegmentDistance / CaterpillarComponent.NodeDistance);
        float TraveledDistance => transform.position.DistanceTo(LastNode.Value.Position);
        float TraveledDistancePerNode => TraveledDistance / CaterpillarComponent.NodeDistance;
        Vector2 LastPosition { get; set; }
        public Vector2 LookDirection => Rigidbody.velocity != Vector2.zero ? Rigidbody.velocity : (Vector2)transform.up;
        event EventHandler<NodeEventArgs> TargetNodeReached;
        public event EventHandler<PlayerEventArgs> PlayerColliding;
        
        Rigidbody2D Rigidbody { get; set; }
        CaterpillarComponent CaterpillarComponent { get; set; }

        void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            CaterpillarComponent = GetComponentInParent<CaterpillarComponent>();
        }

        void Start()
        {
            HeadSegment = GetHeadSegment();
            
            if (!IsHead) return;
            
            AddNodeAt(transform.position);
        }

        void Update()
        {
            if (IsHead)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, TargetHeadRotation,
                    Mathf.Clamp(TraveledDistancePerNode * CaterpillarComponent.HeadRotationDamping, 0.001f, 0.999f));
                
                var traveledDirection = LastPosition.DirectionTo(transform.position);
                var traveledDistance = LastPosition.DistanceTo(transform.position);
                
                if (traveledDirection != Vector2.zero && traveledDistance >= CaterpillarComponent.HeadMovementThreshold)
                {
                    var lookAngle = Vector3.SignedAngle(Vector3.up, traveledDirection, Vector3.forward);
                    TargetHeadRotation = Quaternion.Euler(0f, 0f, lookAngle);   
                }
                
                if (TraveledDistance < CaterpillarComponent.NodeDistance) return;

                AddNode();

                LastPosition = transform.position;
                return;
            }

            if (TargetNode == null || LastNode == null) return;
            
            var position = Vector2.Lerp(LastNode.Value.Position, TargetNode.Value.Position,
                HeadSegment.TraveledDistancePerNode);
            Rigidbody.position = position;

            var rotation = Quaternion.Lerp(LastNode.Value.Rotation, TargetNode.Value.Rotation,
                HeadSegment.TraveledDistancePerNode);
            transform.rotation = rotation;
        }

        public void SetParentSegment(SegmentComponent segment)
        {
            if (ParentSegment) ParentSegment.TargetNodeReached -= OnParentTargetNodeReached;
            ParentSegment = segment;
            
            if (!segment) return;
            ParentSegment.TargetNodeReached += OnParentTargetNodeReached;
        }

        public void SetChildSegment(SegmentComponent segment) => ChildSegment = segment;

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

            if (IsTail &&
                LastNode != null &&
                LastNode.Previous == null)
            {
                HeadSegment.Nodes.RemoveFirst();
            }
            
            LastNode = TargetNode;
            TargetNode = TargetNode.Next ?? TargetNode;

            TargetNodeReached?.Invoke(this, new NodeEventArgs(LastNode));
        }

        void AddNode()
        {
            var direction = LastNode.Value.Position.DirectionTo(transform.position);
            var nodeCount = Mathf.FloorToInt(TraveledDistancePerNode);
            var lastNode = LastNode;
            
            for (var i = 1; i <= nodeCount; i++)
            {
                var position = lastNode.Value.Position + direction * (i * CaterpillarComponent.NodeDistance);
                AddNodeAt(position);
            }
        }

        void AddNodeAt(Vector2 position)
        {
            var index = Nodes.Last?.Value.Index + 1 ?? 0;
            var node = new CaterpillarNode(index, position, transform.rotation);
            var linkedNode = Nodes.AddLast(node);
                
            LastNode = linkedNode;
            TargetNodeReached?.Invoke(this, new NodeEventArgs(LastNode));
        }

        public List<SegmentComponent> GetSegments()
        {
            if (!IsHead) return Head.GetSegments();

            var segments = new List<SegmentComponent>();
            var currentSegment = this;

            do
            {
                segments.Add(currentSegment);
                currentSegment = currentSegment.ChildSegment;
            }
            while (currentSegment);

            return segments;
        }

        [ContextMenu("Split here")]
        public void SplitHere()
        {
            var nodes = Head.Nodes;

            foreach (var node in nodes)
            {
                if (TargetNode == null) continue;
                if (node == TargetNode.Value) break;

                Nodes.AddLast(node);
            }
            
            ParentSegment.SetChildSegment(null);
            SetParentSegment(null);

            var segments = GetSegments();
            foreach (var segment in segments)
            {
                segment.HeadSegment = this;
                
                if (segment.TargetNode == null) continue;
                segment.TargetNode = Nodes.Find(segment.TargetNode.Value);
                
                if (segment.LastNode == null) continue;
                segment.LastNode = Nodes.Find(segment.LastNode.Value);
            }
        }

        void OnTriggerStay2D(Collider2D other)
        {
            var playerComponent = other.GetComponent<PlayerComponent>();
            if (!playerComponent) return;

            PlayerColliding?.Invoke(this, new PlayerEventArgs(playerComponent));
        }
    }
}
