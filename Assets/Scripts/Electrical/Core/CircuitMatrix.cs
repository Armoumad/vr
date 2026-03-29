using System.Collections.Generic;
using UnityEngine;

namespace VR.Electrical.Core
{
    /// <summary>
    /// Lightweight matrix API used by generated components to communicate with a solver implementation.
    /// </summary>
    public class CircuitMatrix
    {
        public readonly List<string> debugStamps = new List<string>();

        public void StampConductance(int nodeA, int nodeB, float conductance)
        {
            debugStamps.Add($"G({nodeA},{nodeB})={conductance}");
        }

        public void StampCurrentSource(int nodeA, int nodeB, float currentAmps)
        {
            debugStamps.Add($"I({nodeA}->{nodeB})={currentAmps}");
        }

        public void StampComment(string comment)
        {
            debugStamps.Add(comment);
        }

        public void StampGround(int node)
        {
            debugStamps.Add($"GND({node})");
        }

        public float ReadNodeVoltage(int node)
        {
            return 0f;
        }
    }
}
