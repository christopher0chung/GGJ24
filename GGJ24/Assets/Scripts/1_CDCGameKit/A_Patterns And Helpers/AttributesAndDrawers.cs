using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDCGameKit
{
    public struct k
    {
        public string key;
    }

    public class ArrayElementTitleAttribute : PropertyAttribute
    {
        public string Varname, Varname2, Prefix, Bridge, Suffix;
        public ArrayElementTitleAttribute(string ElementTitleVar, string Prefix = "", string Suffix = "", string ElementTitleVar2 = "", string Bridge = "")
        {
            Varname = ElementTitleVar;
            Varname2 = ElementTitleVar2;
            this.Prefix = Prefix;
            this.Bridge = Bridge;
            this.Suffix = Suffix;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ArrayElementTitleAttribute))]
    public class ArrayElementTitleDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        protected virtual ArrayElementTitleAttribute Attribute
        {
            get { return (ArrayElementTitleAttribute)attribute; }
        }
        SerializedProperty TitleNameProp, TitleNameProp2;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool addSecond = Attribute.Varname2.Length > 0;

            string FullPathName = property.propertyPath + "." + Attribute.Varname;
            TitleNameProp = property.serializedObject.FindProperty(FullPathName);
            string newlabel = GetTitle(TitleNameProp);

            string FullPathName2 = property.propertyPath + "." + Attribute.Varname2;
            TitleNameProp2 = property.serializedObject.FindProperty(FullPathName2);
            string newlabel2 = addSecond ? GetTitle(TitleNameProp2) : "";

            if (!addSecond)
            {
                if (string.IsNullOrEmpty(newlabel))
                    newlabel = label.text;
                else
                    newlabel = Attribute.Prefix + newlabel + Attribute.Suffix;

                EditorGUI.PropertyField(position, property, new GUIContent(newlabel, label.tooltip), true);
            }
            else
            {
                string combined = "";
                if (string.IsNullOrEmpty(newlabel) && string.IsNullOrEmpty(newlabel2))
                    combined = label.text + " - propery 1 and 2 null or empty";
                else if (string.IsNullOrEmpty(newlabel))
                    combined = label.text + " - propery 1 null or empty";
                else if (string.IsNullOrEmpty(newlabel2))
                    combined = label.text + " - propery 2 null or empty";
                else
                    combined = Attribute.Prefix + newlabel + Attribute.Bridge + newlabel2 + Attribute.Suffix;

                EditorGUI.PropertyField(position, property, new GUIContent(combined, label.tooltip), true);
            }
        }
        private string GetTitle(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Generic:
                    break;
                case SerializedPropertyType.Integer:
                    return property.intValue.ToString();
                case SerializedPropertyType.Boolean:
                    return property.boolValue.ToString();
                case SerializedPropertyType.Float:
                    return property.floatValue.ToString();
                case SerializedPropertyType.String:
                    return property.stringValue;
                case SerializedPropertyType.Color:
                    return property.colorValue.ToString();
                case SerializedPropertyType.ObjectReference:
                    return property.objectReferenceValue.ToString();
                case SerializedPropertyType.LayerMask:
                    break;
                case SerializedPropertyType.Enum:
                    return property.enumNames[TitleNameProp.enumValueIndex];
                case SerializedPropertyType.Vector2:
                    return property.vector2Value.ToString();
                case SerializedPropertyType.Vector3:
                    return property.vector3Value.ToString();
                case SerializedPropertyType.Vector4:
                    return property.vector4Value.ToString();
                case SerializedPropertyType.Rect:
                    break;
                case SerializedPropertyType.ArraySize:
                    break;
                case SerializedPropertyType.Character:
                    break;
                case SerializedPropertyType.AnimationCurve:
                    break;
                case SerializedPropertyType.Bounds:
                    break;
                case SerializedPropertyType.Gradient:
                    break;
                case SerializedPropertyType.Quaternion:
                    break;
                default:
                    break;
            }
            return "";
        }
    }
#endif

    /* ---------------------------- EXAMPLE USAGE ----------------------------
     * [System.Serializable]
        public struct MyStruct
        {
            public enum MyEnum { hello, world }
            public MyEnum m_MyEnum;
        }
        [ArrayElementTitle("m_MyEnum")]
        public MyStruct[] m_MyStruct;
    */
}