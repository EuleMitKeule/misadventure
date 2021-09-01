using System;
using UnityEngine;

namespace Misadventure.Entities
{
    public class EntityEventArgs : EventArgs
    {
        public GameObject EntityObject { get; }

        public EntityEventArgs(GameObject entityObject)
        {
            EntityObject = entityObject;
        }
    }
}