using System;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using B3.GameStateSystem;

namespace B3.EditorExtensions
{
    [CustomEditor(typeof(GameStateMachine))]
    internal sealed class GameStateMachineEditor : Editor
    {
        private SerializedProperty _gameStatesProperty;
        private PropertyField _gameStatesField;
        
        public override VisualElement CreateInspectorGUI()
        {
            _gameStatesProperty = serializedObject.FindProperty("gameStates");
            _gameStatesField = new PropertyField(_gameStatesProperty);
            
            var root = new VisualElement();
            
            var buttonsRoot = GetButtonRoot();
            root.Add(buttonsRoot);
            
            root.Add(_gameStatesField);
            return root;
        }

        private VisualElement AddStateButton(Type stateType)
        {
            var button = new Button(() =>
            {
                if(AlreadyExists(stateType))
                    return;
                
                serializedObject.Update();
                
                int arraySize = _gameStatesProperty.arraySize++;
                var stateProperty = _gameStatesProperty.GetArrayElementAtIndex(arraySize);
                
                stateProperty.managedReferenceValue = Activator.CreateInstance(stateType);
                serializedObject.ApplyModifiedProperties();
            });
            
            button.text = stateType.Name;
            return button;
        }
        
        private VisualElement GetButtonRoot()
        {
            var root = new VisualElement();
            
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var stateType = typeof(GameStateBase);
            
            foreach (Assembly assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.BaseType != stateType)
                        continue;
                    
                    var button = AddStateButton(type);
                    root.Add(button);
                }
            }

            return root;
        }

        private bool AlreadyExists(Type stateType)
        {
            for (int i = 0; i < _gameStatesProperty.arraySize; i++)
            {
                var stateProperty = _gameStatesProperty.GetArrayElementAtIndex(i);

                var referenceValue = stateProperty.managedReferenceValue;
                if (referenceValue is not null && referenceValue.GetType() == stateType)
                    return true;
            }

            return false;
        }
    }
}