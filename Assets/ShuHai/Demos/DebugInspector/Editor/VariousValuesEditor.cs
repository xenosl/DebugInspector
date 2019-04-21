using ShuHai.DebugInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace ShuHai.DebugInspector.Demos.Editor
{
    [CustomEditor(typeof(VariousValues))]
    public class VariousValuesEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Open Debug Inspector Window"))
                InspectorWindow.Open();
        }
    }
}