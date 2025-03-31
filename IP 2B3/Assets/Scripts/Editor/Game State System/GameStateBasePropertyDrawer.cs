using UnityEngine;
using UnityEditor;
using B3.GameStateSystem;

namespace B3.EditorExtensions
{
    [CustomPropertyDrawer(typeof(GameStateBase))]
    internal sealed class GameStateBasePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.managedReferenceValue is null)
            {
                EditorGUI.LabelField(position, "Null State");
                return;
            }

            var stateType = property.managedReferenceValue.GetType();
            string stateName = stateType.Name;

            EditorGUI.PropertyField(position, property, new GUIContent(stateName), true);
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            EditorGUI.GetPropertyHeight(property, label, true);
    }
}