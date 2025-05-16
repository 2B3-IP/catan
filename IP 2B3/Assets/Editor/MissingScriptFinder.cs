using UnityEditor;
using UnityEngine;


namespace Editor
{
    public class MissingScriptFinder
    {
        [MenuItem("Tools/Find Missing Scripts in Scene")]
        static void FindMissingScripts()
        {
            GameObject[] allGameObjects = GameObject.FindObjectsOfType<GameObject>();
            int count = 0;

            foreach (GameObject go in allGameObjects)
            {
                Component[] components = go.GetComponents<Component>();
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i] == null)
                    {
                        Debug.Log($"Missing script found on GameObject: {go.name}", go);
                        count++;
                    }
                }
            }

            Debug.Log($"Finished. Found {count} missing script(s) in the scene.");
        }
    }
}