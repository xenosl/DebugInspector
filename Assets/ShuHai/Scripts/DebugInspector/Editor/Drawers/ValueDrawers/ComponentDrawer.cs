using UnityEngine;

namespace ShuHai.DebugInspector.Editor
{
    public class ComponentDrawer<TOwner, TValue> : UnityObjectDrawer<TOwner, TValue>
        where TValue : Component { }
}