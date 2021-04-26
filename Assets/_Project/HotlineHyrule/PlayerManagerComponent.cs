using System;
using UnityEngine;

namespace HotlineHyrule
{
    public class PlayerManagerComponent : MonoBehaviour
    {
        public static GameObject Player { get; private set; }

        void Awake()
        {
            Player = GameObject.FindWithTag("Player");
        }
    }
}