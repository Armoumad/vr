using System.Collections.Generic;
using UnityEngine;

namespace VR.Electrical.Core
{
    /// <summary>
    /// Provides attach/detach lifecycle hooks for breadboard snapping.
    /// </summary>
    public abstract class BreadboardAttachable : MonoBehaviour
    {
        public bool IsSnapped { get; protected set; }

        public abstract void OnSnapped(Dictionary<int, int> terminalToNode);

        public abstract void OnUnsnapped();
    }
}
