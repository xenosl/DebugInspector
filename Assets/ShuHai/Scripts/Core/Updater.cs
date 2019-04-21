using System;
using UnityEngine;

namespace ShuHai
{
    public class Updater : MonoBehaviour
    {
        public static Updater Instance { get { return SingleComponent<Updater>.Instance; } }

        public static event Action UpdateEvent
        {
            add { Instance.ThisUpdate += value; }
            remove { Instance.ThisUpdate -= value; }
        }

        private event Action ThisUpdate;

        private void Update() { ThisUpdate.NPInvoke(); }
    }
}