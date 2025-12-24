using UnityEngine;
using UnityEditor;
using Core.Utilities;

namespace Core.Utilities.Editor
{
    /// <summary>
    /// Custom property drawer for AutoWire attribute.
    /// Shows a button in the Inspector to manually trigger auto-wiring.
    /// </summary>
    [CustomPropertyDrawer(typeof(AutoWireAttribute))]
    public class AutoWirePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var autoWireAttribute = (AutoWireAttribute)attribute;
            
            // Draw the property normally
            EditorGUI.PropertyField(position, property, label, true);

            // If the field is null, show a button to auto-wire
            if (property.objectReferenceValue == null)
            {
                var buttonRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y + EditorGUIUtility.singleLineHeight, 
                    position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
                
                if (GUI.Button(buttonRect, $"Auto-Wire ({autoWireAttribute.Type})"))
                {
                    var targetObject = property.serializedObject.targetObject as Component;
                    if (targetObject != null)
                    {
                        AutoWireHelper.WireAllFields(targetObject);
                        EditorUtility.SetDirty(targetObject);
                    }
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var baseHeight = EditorGUI.GetPropertyHeight(property, label);
            if (property.objectReferenceValue == null)
            {
                return baseHeight + EditorGUIUtility.singleLineHeight + 2;
            }
            return baseHeight;
        }
    }
}


