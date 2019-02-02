using JReact;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JReact
{
    //this will be used to create a button to save all scriptable objects
    [CustomEditor(typeof(ScriptableObjectSaver))]
    public class ScriptableObjectSaverEditor : Editor
    {
#if UNITY_EDITOR
        public override void OnInspectorGUI()
        {
            //create the default inspector
            DrawDefaultInspector();

            //get the main reference
            var saverScript = (ScriptableObjectSaver)target;

            if (GUILayout.Button("Save All Objects"))
            {
                //un through all scriptable object and save them
                for (int i = 0; i < saverScript.objectsToSave.Length; i++)
                {
                    if (saverScript.objectsToSave[i] != null)
                        EditorUtility.SetDirty(saverScript.objectsToSave[i]);
                }
            }
        }
#endif
    }
}