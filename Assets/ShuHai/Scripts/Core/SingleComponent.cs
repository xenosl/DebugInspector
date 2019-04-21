using System;
using UnityEngine;

namespace ShuHai
{
    public static class SingleComponent<T>
        where T : Component
    {
        public static T Instance { get { return GetInstance(true); } }

        public static T GetInstance(bool createIfNotFound)
        {
            if (instance != null)
                return instance;

            instance = UnityObjectUtil.FindUniqueSceneComponent<T>();

            if (instance == null)
            {
                if (createIfNotFound)
                {
                    var gameObject = new GameObject(typeof(T).Name);
                    instance = gameObject.AddComponent<T>();
                }
                else
                {
                    throw new InvalidOperationException(
                        string.Format(@"Instance of ""{0}"" not found.", typeof(T)));
                }
            }

            return instance;
        }

        private static T instance;
    }
}