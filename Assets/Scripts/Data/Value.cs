using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace DLIFR.Data
{
    [Serializable]
    public class Value<T>
    {
        public Variable<T> variable;

        [SerializeField, HideInInspector]
        private T _value;
        
        public T value 
        {
            get => variable is null ? _value : variable.value;

            set { if(variable is null) _value = value; else variable.value = value; }
        }

        public Value(T value)
        {
            this.value = value;
        }

        public static implicit operator T (Value<T> value)
        {
            return value.value;
        }

        public static implicit operator Value<T> (T value)
        {
            return new Value<T>(value);
        }
    }

    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(Value<>))]
    public class ValueEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            position.height = EditorGUIUtility.singleLineHeight;
            //var indent = EditorGUI.indentLevel;
            //EditorGUI.indentLevel = 0;

            SerializedProperty sp = property.FindPropertyRelative("variable");
            EditorGUI.PropertyField(position, sp, GUIContent.none);
            bool has = sp.objectReferenceValue != null;

            EditorGUI.BeginDisabledGroup(has);
            position.y += EditorGUIUtility.singleLineHeight + 2;
            if(!has)
                EditorGUI.PropertyField(position, property.FindPropertyRelative("_value"), GUIContent.none);
            else
                EditorGUI.TextField(position, sp.objectReferenceValue.ToString());
            EditorGUI.EndDisabledGroup();

            //EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2 + 2;
        }
    }
    #endif
}