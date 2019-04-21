using System;

namespace ShuHai.DebugInspector.Editor
{
    public class DrawerUpdateParameters
    {
        public static readonly DrawerUpdateParameters Default = new DrawerUpdateParameters();

        public const int DefaultMaxDepth = int.MaxValue;

        public int MaxDepth = DefaultMaxDepth;

        /// <summary>
        ///     Children will be created during update if possible if the value is set to <see langword="true" />, this basically
        ///     means create the whole hierarchy with its leaf represents primitive types.
        /// </summary>
        public bool DeepUpdate = false;

        public Func<Drawer, bool> ChildrenUpdatePrecondition;
    }
}