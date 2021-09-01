using System;
using UnityEngine;
using Object = System.Object;

namespace Misadventure.Extensions
{
    [Serializable]
    public class SerializableSystemType
    {
        [SerializeField] private string m_Name;
        public string Name { get { return m_Name; } }

        [SerializeField] private string m_AssemblyQualifiedName;
        public string AssemblyQualifiedName { get { return m_AssemblyQualifiedName; } }

        private Type m_SystemType;
        public Type SystemType
        {
            get
            {
                if (m_SystemType == null)
                {
                    GetSystemType();
                }
                return m_SystemType;
            }
        }

        private void GetSystemType()
        {
            m_SystemType = string.IsNullOrEmpty(m_AssemblyQualifiedName) ? null : Type.GetType(m_AssemblyQualifiedName);
        }

        public SerializableSystemType(Type _SystemType)
        {
            if (_SystemType == null)
                throw new ArgumentNullException("_SystemType");
            m_SystemType = _SystemType;
            m_Name = _SystemType.Name;
            m_AssemblyQualifiedName = _SystemType.AssemblyQualifiedName;
        }

        public override bool Equals(Object obj)
        {
            SerializableSystemType temp = obj as SerializableSystemType;
            if ((object)temp == null)
            {
                return false;
            }
            return Equals(temp);
        }

        public bool Equals(SerializableSystemType _Object)
        {
            if (_Object == null)
                return false;
            return SystemType.Equals(_Object.SystemType);
        }

        public static bool operator ==(SerializableSystemType a, SerializableSystemType b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(SerializableSystemType a, SerializableSystemType b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return SystemType.GetHashCode();
        }

        public static implicit operator SerializableSystemType(Type type)
        {
            return new SerializableSystemType(type);
        }

        public static implicit operator Type(SerializableSystemType type)
        {
            return type.SystemType;
        }
    }
}