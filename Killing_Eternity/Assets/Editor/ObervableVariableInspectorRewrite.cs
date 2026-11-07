using UnityEditor;
using UnityEngine;
using KE;

[CustomPropertyDrawer(typeof(ObservableVariable<>), true)]
public class ObservableVariableDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Tell Unity to begin drawing the property
        EditorGUI.BeginProperty(position, label, property);

        // Find the private "_value" variable inside our ObservableVariable class
        SerializedProperty valueProp = property.FindPropertyRelative("_value");

        // Draw that specific variable using the original name (e.g., "Is Moving")
        if (valueProp != null)
        {
            EditorGUI.PropertyField(position, valueProp, label);
        }
        else
        {
            // Fallback just in case the variable name gets changed later
            EditorGUI.LabelField(position, label.text, "Value not found.");
        }

        EditorGUI.EndProperty();
    }
}
