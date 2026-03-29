using System.Collections.Generic;
using UnityEngine;

namespace VR.Electrical.Core
{
    /// <summary>
    /// Shared base for generated electrical and annotation components.
    /// Handles terminal declarations, breadboard snapping, validation, and debug telemetry.
    /// </summary>
    public abstract class ElectricalComponentBase : BreadboardAttachable, IStampable
    {
        [Header("Metadata")]
        [SerializeField] private string csvComponentName;

        [Header("Terminals")]
        [SerializeField] protected List<PinTerminal> terminals = new List<PinTerminal>();

        [Header("Runtime Debug")]
        [SerializeField] protected float[] debugTerminalVoltages = new float[0];
        [SerializeField] protected float debugCurrentAmps;
        [SerializeField] protected float debugState;

        public abstract ElectricalComponentKind Kind { get; }

        public abstract int TerminalCount { get; }

        public virtual bool IsElectrical => true;

        public IReadOnlyList<PinTerminal> Terminals => terminals;

        protected virtual void Reset()
        {
            csvComponentName = Kind.ToString();
            EnsureTerminalConfiguration();
        }

        public override void OnSnapped(Dictionary<int, int> terminalToNode)
        {
            if (terminalToNode == null || terminalToNode.Count != TerminalCount)
            {
                Debug.LogWarning($"{name}: snap rejected due to incomplete terminal map.");
                return;
            }

            for (int i = 0; i < TerminalCount; i++)
            {
                if (!terminalToNode.TryGetValue(i, out int nodeId) || nodeId < 0)
                {
                    Debug.LogWarning($"{name}: snap rejected due to missing terminal {i}.");
                    return;
                }
            }

            EnsureTerminalConfiguration();
            for (int i = 0; i < TerminalCount; i++)
            {
                terminals[i].nodeIndex = terminalToNode[i];
                terminals[i].isAttached = true;
            }

            IsSnapped = true;
        }

        public override void OnUnsnapped()
        {
            for (int i = 0; i < terminals.Count; i++)
            {
                terminals[i].nodeIndex = -1;
                terminals[i].isAttached = false;
            }

            IsSnapped = false;
        }

        public virtual void OnValidate()
        {
            EnsureTerminalConfiguration();
        }

        protected void EnsureTerminalConfiguration()
        {
            int count = Mathf.Max(0, TerminalCount);
            while (terminals.Count < count)
            {
                terminals.Add(new PinTerminal { terminalIndex = terminals.Count });
            }

            if (terminals.Count > count)
            {
                terminals.RemoveRange(count, terminals.Count - count);
            }

            for (int i = 0; i < terminals.Count; i++)
            {
                terminals[i].terminalIndex = i;
            }

            if (debugTerminalVoltages == null || debugTerminalVoltages.Length != count)
            {
                debugTerminalVoltages = new float[count];
            }
        }

        protected int NodeOrDefault(int terminalIndex)
        {
            if (terminalIndex < 0 || terminalIndex >= terminals.Count)
            {
                return -1;
            }

            return terminals[terminalIndex].nodeIndex;
        }

        protected bool HasValidNodes(params int[] terminalIndexes)
        {
            foreach (int index in terminalIndexes)
            {
                if (NodeOrDefault(index) < 0)
                {
                    return false;
                }
            }

            return true;
        }

        protected float ReadVoltage(CircuitMatrix matrix, int terminalIndex)
        {
            int node = NodeOrDefault(terminalIndex);
            float value = node >= 0 ? matrix.ReadNodeVoltage(node) : 0f;
            if (terminalIndex >= 0 && terminalIndex < debugTerminalVoltages.Length)
            {
                debugTerminalVoltages[terminalIndex] = value;
            }

            return value;
        }

        public virtual void Stamp(CircuitMatrix matrix)
        {
            if (!IsElectrical)
            {
                return;
            }

            matrix.StampComment($"TODO(parity): {Kind} default stamp should be specialized.");
        }

        public virtual void Step(float deltaTime, CircuitMatrix matrix)
        {
        }
    }
}
