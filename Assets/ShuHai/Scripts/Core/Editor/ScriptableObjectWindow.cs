using UnityEditor;
using UnityEngine;

namespace ShuHai.Editor
{
    public abstract class ScriptableObjectWindow<T> : EditorWindow
        where T : UnityEditor.Editor
    {
        protected T editor { get; private set; }

        protected abstract T CreateEditor();

        protected virtual void Awake() { editor = CreateEditor(); }

        protected virtual void OnDestroy()
        {
            DestroyImmediate(editor);
            editor = null;
        }

        private Vector2 scrollPosition;

        protected virtual void OnGUI()
        {
            using (var scope = new EditorGUILayout.ScrollViewScope(scrollPosition))
            {
                editor.OnInspectorGUI();
                scrollPosition = scope.scrollPosition;
            }
        }
    }
}