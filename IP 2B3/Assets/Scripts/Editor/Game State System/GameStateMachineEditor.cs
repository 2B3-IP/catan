using System;
using B3.GameStateSystem;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace B3.EditorExtensions
{
    [CustomEditor(typeof(GameStateMachine))]
    internal sealed class GameStateMachineEditor : Editor
    {
        private SerializedProperty gameStatesProperty;
        
        public override VisualElement CreateInspectorGUI()
        {
            gameStatesProperty = serializedObject.FindProperty("gameStates");
            
            var root = new VisualElement();

            var buttonsRoot = initializeButtonsRoot();
            root.Add(buttonsRoot);

            root.Add(new PropertyField(gameStatesProperty));
            root.Add(new PropertyField(serializedObject.FindProperty("firstStateIndex")));
            root.Add(new PropertyField(serializedObject.FindProperty("secondStateIndex")));
            root.Add(new PropertyField(serializedObject.FindProperty("playersManager")));
            
            return root;
        }

        private VisualElement initializeButtonsRoot()
        {
            var root = new VisualElement();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var types = assembly.GetTypes();
                
                foreach (var type in types)
                {
                    if (!type.IsSubclassOf(typeof(GameStateBase))) 
                        continue;

                    var button = new Button(() =>
                    {
                        if (AlreadyExists(type))
                            return;
                        
                        int index = gameStatesProperty.arraySize;
                        gameStatesProperty.InsertArrayElementAtIndex(index);
                        
                        var element = gameStatesProperty.GetArrayElementAtIndex(index);
                        var gameState = (GameStateBase)Activator.CreateInstance(type);
                        
                        element.managedReferenceValue = gameState;
                        serializedObject.ApplyModifiedProperties();
                    });
                    
                    button.text = type.Name;
                    root.Add(button);
                }
            }

            return root;
        }

        private bool AlreadyExists(Type type)
        {
            for (int i = 0; i < gameStatesProperty.arraySize; i++)
            {
                var element = gameStatesProperty.GetArrayElementAtIndex(i);
                var value = element.managedReferenceValue;

                if (value != null && value.GetType() == type)
                    return true;
            }

            return false;
        }
    }
}