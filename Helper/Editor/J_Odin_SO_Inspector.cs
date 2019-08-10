using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;

namespace JReact
{
    public class ScriptableObjectButtonProcessor<T> : OdinPropertyProcessor<T>
        where T : ScriptableObject
    {
        private UnityEngine.Object[] _selection = new UnityEngine.Object[1];

        public override void ProcessMemberProperties(List<InspectorPropertyInfo> propertyInfos)
        {
            propertyInfos.AddDelegate("Select SO",
                                      () =>
                                      {
                                          // This code runs when the button is clicked
                                          var so = Property.ValueEntry.WeakSmartValue as T;
                                          EditorApplication.ExecuteMenuItem("Window/General/Project");

                                          // --------------- SELECTION --------------- //
                                          _selection[0]                 = so;
                                          UnityEditor.Selection.objects = _selection;
                                          UnityEditor.Selection.SetActiveObjectWithContext(so, so);

                                          //set the new active object and make sure the project window is open
                                          UnityEditor.Selection.activeObject = _selection[0];
                                          EditorGUIUtility.PingObject(so);
                                          Debug.Log("Selected " + so.name + "!");
                                      },
                                      int.MinValue, new ButtonAttribute(ButtonSizes.Medium),
                                      new BoxGroupAttribute("Editor", true, true, -1000));
        }
    }
}
#endif
