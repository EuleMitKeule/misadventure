using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using HotlineHyrule.Extensions;
using UnityEditor.Graphs;
using UnityEngine;

namespace HotlineHyrule.Entities
{
    public class SegmentComponent : MonoBehaviour
    {
        [SerializeField] float nodeDistance;
        [SerializeField] float segmentDistance;
        [SerializeField] float headRotationDamping;
        [SerializeField] float headMovementThreshold;
        [SerializeField] SegmentComponent parentSegment;
        [SerializeField] SegmentComponent childSegment;
        
        public LinkedList<CaterpillarNode> Nodes { get; } = new LinkedList<CaterpillarNode>();
        LinkedListNode<CaterpillarNode> TargetNode { get; set; }
        LinkedListNode<CaterpillarNode> LastNode { get; set; }
        public SegmentComponent ParentSegment { get; set; }
        public SegmentComponent ChildSegment { get; set; }
        SegmentComponent HeadSegment { get; set; }
        public SegmentComponent Head => IsHead ? this : HeadSegment;
        Quaternion TargetHeadRotation { get; set; }
        public bool IsHead => !ParentSegment;
        bool IsTail => !ChildSegment;
        int MinNodeDifference => Mathf.RoundToInt(segmentDistance / nodeDistance);
        float TraveledDistance => transform.position.DistanceTo(LastNode.Value.Position);
        float TraveledDistancePerNode => TraveledDistance / nodeDistance;
        Vector2 LastPosition { get; set; }
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
            
            AddNodeAt(transform.position);
        }

        void Update()
        {
            if (IsHead)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, TargetHeadRotation, TraveledDistancePerNode * headRotationDamping);
                
                var traveledDirection = LastPosition.DirectionTo(transform.position);
                var traveledDistance = LastPosition.DistanceTo(transform.position);
                
                if (traveledDirection != Vector2.zero && traveledDistance >= headMovementThreshold)
                {
                    var lookAngle = Vector3.SignedAngle(Vector3.up, traveledDirection, Vector3.forward);
                    TargetHeadRotation = Quaternion.Euler(0f, 0f, lookAngle);   
                }
                
                if (TraveledDistance < nodeDistance) return;

                AddNode();

                LastPosition = transform.position;
                return;
            }

            if (TargetNode == null || LastNode == null) return;
            
            var position = Vector2.Lerp(LastNode.Value.Position, TargetNode.Value.Position,
                HeadSegment.TraveledDistancePerNode);
            transform.position = position;

            var rotation = Quaternion.Lerp(LastNode.Value.Rotation, TargetNode.Value.Rotation,
                HeadSegment.TraveledDistancePerNode);
            transform.rotation = rotation;
        }

        void SetParentSegment(SegmentComponent segment)
        {
            if (ParentSegment) ParentSegment.TargetNodeReached -= OnParentTargetNodeReached;
            ParentSegment = segment;
            
            if (!segment) return;
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
                var position = lastNode.Value.Position + direction * (i * nodeDistance);
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
                if (node == TargetNode.Value) break;

                Nodes.AddLast(node);
            }
            
            ParentSegment.SetChildSegment(null);
            SetParentSegment(null);

            var segments = GetSegments();
            foreach (var segment in segments)
            {
                segment.HeadSegment = this;
                segment.TargetNode = Nodes.Find(segment.TargetNode.Value);
                segment.LastNode = Nodes.Find(segment.LastNode.Value);
            }
            
            
        }
    }
}
