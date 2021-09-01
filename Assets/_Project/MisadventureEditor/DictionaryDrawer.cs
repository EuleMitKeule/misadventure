using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Misadventure.Extensions;
using Misadventure.Items;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace MisadventureEditor
{
    public abstract class DictionaryDrawer<TK, TV> : PropertyDrawer
    {

        private SerializableDictionary<TK, TV> _Dictionary;
        private bool _Foldout;
        private const float kButtonWidth = 18f;

        public DictionaryDrawer() { }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            CheckInitialize(property, label);
            if (_Foldout)
                return (_Dictionary.Count + 1) * 20f;
            return 17f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            CheckInitialize(property, label);

            position.height = 17f;

            var foldoutRect = position;
            foldoutRect.width -= 2 * kButtonWidth;
            EditorGUI.BeginChangeCheck();
            _Foldout = EditorGUI.Foldout(foldoutRect, _Foldout, label, true);
            if (EditorGUI.EndChangeCheck())
                EditorPrefs.SetBool(label.text, _Foldout);

            var buttonRect = position;
            buttonRect.x = position.width - kButtonWidth + position.x;
            buttonRect.width = kButtonWidth + 2;

            if (GUI.Button(buttonRect, new GUIContent("+", "Add item"), EditorStyles.miniButton))
            {
                AddNewItem();
            }

            buttonRect.x -= kButtonWidth;

            if (GUI.Button(buttonRect, new GUIContent("X", "Clear dictionary"), EditorStyles.miniButtonRight))
            {
                ClearDictionary();
            }

            if (!_Foldout)
                return;

            foreach (var item in _Dictionary)
            {
                var key = item.Key;
                var value = item.Value;

                position.y += 20f;

                var keyRect = position;
                keyRect.width /= 2;
                keyRect.width -= 4;
                EditorGUI.BeginChangeCheck();
                var newKey = DoField(keyRect, typeof(TK), key);
                if (EditorGUI.EndChangeCheck())
                {
                    try
                    {
                        _Dictionary.Remove(key);
                        _Dictionary.Add(newKey, value);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                    }
                    break;
                }

                var valueRect = position;
                valueRect.x = position.width / 2 + 8;
                valueRect.width = keyRect.width - kButtonWidth;
                EditorGUI.BeginChangeCheck();
                value = DoField(valueRect, typeof(TV), value);
                if (EditorGUI.EndChangeCheck())
                {
                    _Dictionary[key] = value;
                    break;
                }

                var removeRect = valueRect;
                removeRect.x = valueRect.xMax + 2;
                removeRect.width = kButtonWidth;
                if (GUI.Button(removeRect, new GUIContent("x", "Remove item"), EditorStyles.miniButtonRight))
                {
                    RemoveItem(key);
                    break;
                }
            }

            // EditorGUILayout.Space();

            try
            {
                if (typeof(UnityObject).IsAssignableFrom(typeof(TK)))
                    _Dictionary.DefaultKey = (TK)(object)EditorGUILayout.ObjectField("Default Key", (UnityObject)(object)_Dictionary.DefaultKey, typeof(TK), false);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void RemoveItem(TK key)
        {
            _Dictionary.Remove(key);
        }

        private void CheckInitialize(SerializedProperty property, GUIContent label)
        {
            if (_Dictionary == null)
            {
                var target = GetParent(property);
                _Dictionary = fieldInfo.GetValue(target) as SerializableDictionary<TK, TV>;
                if (_Dictionary == null)
                {
                    _Dictionary = new SerializableDictionary<TK, TV>();
                    fieldInfo.SetValue(property.serializedObject.targetObject, _Dictionary);
                }

                _Foldout = EditorPrefs.GetBool(label.text);
            }
        }

        private static readonly Dictionary<Type, Func<Rect, object, object>> _Fields =
            new Dictionary<Type, Func<Rect, object, object>>()
            {
                { typeof(int), (rect, value) => EditorGUI.IntField(rect, (int)value) },
                { typeof(float), (rect, value) => EditorGUI.FloatField(rect, (float)value) },
                { typeof(string), (rect, value) => EditorGUI.TextField(rect, (string)value) },
                { typeof(bool), (rect, value) => EditorGUI.Toggle(rect, (bool)value) },
                { typeof(Vector2), (rect, value) => EditorGUI.Vector2Field(rect, GUIContent.none, (Vector2)value) },
                {typeof(Vector2Int), (rect, value) => EditorGUI.Vector2IntField(rect, GUIContent.none, (Vector2Int) value)},
                { typeof(Vector3), (rect, value) => EditorGUI.Vector3Field(rect, GUIContent.none, (Vector3)value) },
                {typeof(Vector3Int), (rect, value) => EditorGUI.Vector3IntField(rect, GUIContent.none, (Vector3Int) value)},
                { typeof(Bounds), (rect, value) => EditorGUI.BoundsField(rect, (Bounds)value) },
                { typeof(Rect), (rect, value) => EditorGUI.RectField(rect, (Rect)value) },
            };

        private static T DoField<T>(Rect rect, Type type, T value)
        {
            Func<Rect, object, object> field;
            if (_Fields.TryGetValue(type, out field))
                return (T)field(rect, value);

            if (type.IsEnum)
                return (T)(object)EditorGUI.EnumPopup(rect, (Enum)(object)value);

            if (typeof(UnityObject).IsAssignableFrom(type))
                return (T)(object)EditorGUI.ObjectField(rect, (UnityObject)(object)value, type, true);

            Debug.Log("Type is not supported: " + type);
            return value;
        }

        private void ClearDictionary()
        {
            _Dictionary.Clear();
        }

        private void AddNewItem()
        {
            TK key;
            if (typeof(TK) == typeof(string))
                key = (TK)(object)"";
            else if (typeof(TK) == typeof(Vector3Int))
            {
                var defKey = Vector3Int.zero;
                while (_Dictionary.ContainsKey((TK)(object)defKey)) defKey = new Vector3Int(defKey.x + 1, 0, 0);
                key = (TK) (object) defKey;
            }
            else key = default(TK);

            if (key == null)
            {
                if (_Dictionary.DefaultKey != null) key = _Dictionary.DefaultKey;
            }

            var value = default(TV);
            try
            {
                _Dictionary.Add(key, value);
            }
            catch (ArgumentNullException)
            {
                if (typeof(TK) == typeof(ItemData))
                {
                    _Dictionary.Add((TK)(object)ScriptableObject.CreateInstance<ItemData>(), value);
                }
                // Debug.Log("You need to set a default key.");
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        public object GetParent(SerializedProperty prop)
        {
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements.Take(elements.Length - 1))
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue(obj, elementName, index);
                }
                else
                {
                    obj = GetValue(obj, element);
                }
            }
            return obj;
        }

        public object GetValue(object source, string name)
        {
            if (source == null)
                return null;
            var type = source.GetType();
            var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (f == null)
            {
                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p == null)
                    return null;
                return p.GetValue(source, null);
            }
            return f.GetValue(source);
        }

        public object GetValue(object source, string name, int index)
        {
            var enumerable = GetValue(source, name) as IEnumerable;
            var enm = enumerable.GetEnumerator();
            while (index-- >= 0)
                enm.MoveNext();
            return enm.Current;
        }
    }
}