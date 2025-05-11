using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
public class ConditionalHidePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var condHAtt = (ConditionalHideAttribute)attribute;
        var sourceProp = property.serializedObject.FindProperty(condHAtt.ConditionalSourceField);

        if (ShouldShow(sourceProp, condHAtt.CompareValue)) {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var condHAtt = (ConditionalHideAttribute)attribute;
        var sourceProp = property.serializedObject.FindProperty(condHAtt.ConditionalSourceField);

        return ShouldShow(sourceProp, condHAtt.CompareValue) ?
            EditorGUI.GetPropertyHeight(property, label, true) : 0f;
    }

    private bool ShouldShow(SerializedProperty sourceProp, object compareValue)
    {
        if (sourceProp == null) return true;

        switch (sourceProp.propertyType) {
            case SerializedPropertyType.Boolean:
                return sourceProp.boolValue.Equals(compareValue);
            case SerializedPropertyType.Enum:
                return sourceProp.enumNames[sourceProp.enumValueIndex] == compareValue.ToString();
            default:
                return true;
        }
    }
}
