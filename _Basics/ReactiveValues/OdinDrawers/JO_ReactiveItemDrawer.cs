#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace JReact.OdinDrawers
{
    public class JO_ReactiveBoolDrawer : OdinValueDrawer<J_ReactiveBool>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            //get the value and draw the element
            J_ReactiveBool value = this.ValueEntry.SmartValue;

            //just shows the empty field if the SO is missing
            if (value == null)
            {
                this.CallNextDrawer(label);
                return;
            }

            var rect = EditorGUILayout.GetControlRect();
            //NAME
            GUI.Label(rect.AlignLeft(100f), label);
            //VALUE
            value.CurrentValue = EditorGUI.Toggle(rect.AlignCenter(20f), value.CurrentValue);
            //REFERENCE
            this.ValueEntry.SmartValue =
                (J_ReactiveBool) SirenixEditorFields.UnityObjectField(rect.AlignRight(100f), value, typeof(J_ReactiveBool), false);
        }
    }

    public class JO_ReactiveIntDrawer : OdinValueDrawer<J_ReactiveInt>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            //get the value and draw the element
            J_ReactiveInt value = this.ValueEntry.SmartValue;

            //just shows the empty field if the SO is missing
            if (value == null)
            {
                this.CallNextDrawer(label);
                return;
            }

            var rect = EditorGUILayout.GetControlRect();
            //NAME
            GUI.Label(rect.AlignLeft(100f), label);
            //VALUE
            value.CurrentValue = SirenixEditorFields.IntField(rect.AlignCenter(200f), value.CurrentValue);
            //REFERENCE
            this.ValueEntry.SmartValue =
                (J_ReactiveInt) SirenixEditorFields.UnityObjectField(rect.AlignRight(100f), value, typeof(J_ReactiveBool), false);
        }
    }

    public class JO_ReactiveFloatDrawer : OdinValueDrawer<J_ReactiveFloat>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            //get the value and draw the element
            J_ReactiveFloat value = this.ValueEntry.SmartValue;

            //just shows the empty field if the SO is missing
            if (value == null)
            {
                this.CallNextDrawer(label);
                return;
            }

            var rect = EditorGUILayout.GetControlRect();
            //NAME
            GUI.Label(rect.AlignLeft(100f), label);
            //VALUE
            value.CurrentValue = SirenixEditorFields.FloatField(rect.AlignCenter(200f), value.CurrentValue);
            //REFERENCE
            this.ValueEntry.SmartValue =
                (J_ReactiveFloat) SirenixEditorFields.UnityObjectField(rect.AlignRight(100f), value, typeof(J_ReactiveBool), false);
        }
    }

    public class JO_ReactiveStringDrawer : OdinValueDrawer<J_ReactiveString>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            //get the value and draw the element
            J_ReactiveString value = this.ValueEntry.SmartValue;

            //just shows the empty field if the SO is missing
            if (value == null)
            {
                this.CallNextDrawer(label);
                return;
            }

            var rect = EditorGUILayout.GetControlRect();
            //NAME
            GUI.Label(rect.AlignLeft(100f), label);

            //REFERENCE
            this.ValueEntry.SmartValue =
                (J_ReactiveString) SirenixEditorFields.UnityObjectField(rect.AlignRight(100f), value, typeof(J_ReactiveBool), false);

            rect = EditorGUILayout.GetControlRect(false, 50f);
            //VALUE
            value.CurrentValue = SirenixEditorFields.TextField(rect, value.CurrentValue);
        }
    }
}

#endif
