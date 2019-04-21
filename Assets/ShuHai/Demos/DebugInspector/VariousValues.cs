using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShuHai.DebugInspector.Demos
{
    using UObject = UnityEngine.Object;

    public interface IVariousValues { }

    [ExecuteInEditMode]
    public class VariousValues : MonoBehaviour, IVariousValues, IEnumerable<object>
    {
        public Int32 Int32Field;

        protected UInt64 UInt64Field = UInt64.MaxValue;

        public Single SingleField;

        public string StringField = "Text Value...";

        internal object ObjectField;

        public object ObjectProperty { get { return ObjectField; } set { ObjectField = value; } }

        protected internal UObject UnityObjectField;

        public UObject UnityObjectProperty { get { return UnityObjectField; } set { UnityObjectField = value; } }

        public UObject PropertyWithErrorLogs
        {
            get
            {
                Debug.LogError("Error occurs when getting property.");
                Debug.LogError("Some other errors...");
                return UnityObjectField;
            }
            set
            {
                Debug.LogError("Error occurs when setting property.");
                Debug.LogError("Unable to set value");
                UnityObjectField = value;
            }
        }

        internal UObject PropertyWithException
        {
            get { throw new NotSupportedException("Exception thrown when getting property."); }
            set { throw new NotSupportedException("Exception thrown when setting property."); }
        }

        public UObject PropertyAccessFail
        {
            get
            {
                Debug.LogError("Error occurs when getting property.");
                Debug.LogError("Some other errors...");
                return UnityObjectField;
            }
            set
            {
                Debug.LogError("Error occurs when setting property.");
                Debug.LogError("Unable to set value");
                UnityObjectField = value;
                throw new MissingFieldException("Unity Object filed not found.");
            }
        }

        public List<object> ObjectList = new List<object> { 23, "Str" };

        public object[] ObjectArray = { 222, "1sst", string.Empty, new object() };

        public ICollection<Component> Components
        {
            get { return componentSet ?? (componentSet = new HashSet<Component>(GetComponents<Component>())); }
        }

        private HashSet<Component> componentSet = new HashSet<Component>();

        [NonSerialized] public List<GameObject> ActiveGameObjects;

        public MeshFilter MeshFilter;

        private void Update()
        {
            if (ActiveGameObjects == null)
            {
                ActiveGameObjects = new List<GameObject>();
                var rootObjects = gameObject.scene.GetRootGameObjects();
                foreach (var obj in rootObjects)
                    AddActiveChildren(obj);
            }
        }

        private void AddActiveChildren(GameObject obj)
        {
            foreach (Transform t in obj.transform)
            {
                var childObj = t.gameObject;
                if (childObj.activeInHierarchy)
                    ActiveGameObjects.Add(childObj);
                AddActiveChildren(childObj);
            }
        }

        #region IEnumerator

        public IEnumerator<object> GetEnumerator()
        {
            yield return UnityObjectField;
            yield return ObjectList;
            yield return PropertyAccessFail;
            yield return UnityObjectField;
            yield return null;
            yield return null;
            yield return UnityObjectField;
            yield return null;
            yield return UnityObjectField;
            yield return null;
            yield return UnityObjectField;
            yield return UnityObjectField;
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        #endregion IEnumerator
    }
}