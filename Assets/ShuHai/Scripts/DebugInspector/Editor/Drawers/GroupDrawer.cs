using UnityEditor;

namespace ShuHai.DebugInspector.Editor
{
    /// <summary>
    ///     Represents a drawer container as parent drawer.
    /// </summary>
    public class GroupDrawer : Drawer
    {
        public override bool SelfVisible
        {
            // Hide self if this is the only child.
            get { return !isOnlyChild && base.SelfVisible; }
            set { base.SelfVisible = value; }
        }

        public override bool ChildrenVisible
        {
            // Children visibility is controlled by parent if this is the only child, so here always return true.
            get { return isOnlyChild || base.ChildrenVisible; } 
            set { base.ChildrenVisible = value; }
        }

        private bool isOnlyChild { get { return Parent != null && Parent.ChildCount == 1; } }

        protected override void SelfGUI()
        {
            if (ChildCount > 0)
            {
                EditorGUILayout.BeginHorizontal();
                ChildrenVisible = DIGUI.FoldoutToggle(ChildrenVisible, false);
                EditorGUILayout.LabelField(Name);
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}