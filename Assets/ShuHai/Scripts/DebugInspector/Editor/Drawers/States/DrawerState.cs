namespace ShuHai.DebugInspector.Editor
{
    public class DrawerState
    {
        public bool SelfVisible = Drawer.DefaultSelfVisible;
        public bool ChildrenVisible = Drawer.DefaultChildrenVisible;

        public override string ToString()
        {
            return string.Format(@"{0}:{{{1},{2}}}", GetType(), SelfVisible, ChildrenVisible);
        }
    }

    public class ValueDrawerState : DrawerState { }

    public class PrimitiveDrawerState : ValueDrawerState { }
}