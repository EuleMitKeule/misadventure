using System;
using UnityEngine;

namespace HotlineHyrule
{
    public class StationaryChildComponent : MonoBehaviour
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public Vector3 Velocity { get; set; }

        Transform Transform { get; set; }
        
        void Awake()
        {
            Transform = GetComponent<Transform>();
        }

        void FixedUpdate()
        {
            Position += Velocity * Time.fixedDeltaTime;
        }
        
        void LateUpdate()
        {
            Transform.position = Position;
            Transform.rotation = Rotation;
            Transform.localScale = Scale;
        }
    }
}