using B3.GameStateSystem;
using UnityEditor;
using UnityEngine;

namespace B3.EditorExtensions
{
    [CustomPropertyDrawer(typeof(GameStateBase), true)]
    internal sealed class GameStateBasePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var value = property.managedReferenceValue;
            
            label = value != null ? 
                new GUIContent(value.GetType().Name) : 
                new GUIContent("Null State");

            EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            EditorGUI.GetPropertyHeight(property, true);
    }
}