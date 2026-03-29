using System;
using UnityEngine;

namespace VR.Electrical.Core
{
    /// <summary>
    /// Serializable pin descriptor used by electrical components for terminal-node mapping.
    /// </summary>
    [Serializable]
    public class PinTerminal
    {
        [Tooltip("Terminal index local to the component")]
        public int terminalIndex;

        [Tooltip("Node index in global circuit graph (-1 means unbound)")]
        public int nodeIndex = -1;

        [Tooltip("True when this terminal is currently attached to a breadboard node")]
        public bool isAttached;
    }
}
