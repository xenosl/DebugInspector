using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ShuHai.DebugInspector.Editor
{
    public static class DIDebug
    {
        public static void LogDrawerHierarchy(Drawer drawer, DrawerActionScope scope)
        {
            Debug.Log(StringUtil.Join(drawer.EnumerateByScope(scope), d => d.HierarchicalName, "\n"));
        }

        public static void LogDrawerHierarchy(IEnumerable<Drawer> drawers, DrawerActionScope scope)
        {
            var list = drawers.SelectMany(d => d.EnumerateByScope(scope));
            Debug.Log(StringUtil.Join(list, d => d.HierarchicalName, "\n"));
        }
    }
}