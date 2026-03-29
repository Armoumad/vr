using UnityEngine;
using VR.Electrical.Core;

namespace VR.Electrical.Components
{
    /// <summary>
    /// Resistor model generated from CircuitJS1 reference file ResistorElm.java.
    /// Simplified for Unity breadboard integration with stable stamping/step behavior.
    /// TODO(parity): compare against https://github.com/pfalstad/circuitjs1/blob/master/src/com/lushprojects/circuitjs1/client/ResistorElm.java for full behavioral parity.
    /// </summary>
    public class ResistorComponent : ElectricalComponentBase
    {
[Header("Model Parameters")]
[Tooltip("Resistance in ohms")]
[SerializeField] private float resistanceOhms = 1000f;

[Tooltip("Secondary model parameter used for simplified parity")]
[SerializeField] private float secondaryParameter = 1f;

[Tooltip("Output/source impedance in ohms")]
[SerializeField] private float outputImpedanceOhms = 1000f;

        [Tooltip("Terminal index 0")]
        [SerializeField] private int terminal0 = 0;

        [Tooltip("Terminal index 1")]
        [SerializeField] private int terminal1 = 1;

        [Header("Runtime Debug")]
        [Tooltip("Last simulated terminal differential voltage")]
        [SerializeField] private float debugVoltageVolts;

        [Tooltip("Last simulated branch current")]
        [SerializeField] private float debugCurrent;

        [Tooltip("Last internal state scalar")]
        [SerializeField] private float debugInternalState;

        public override ElectricalComponentKind Kind => ElectricalComponentKind.Resistor;

        public override int TerminalCount => 2;

        public override bool IsElectrical => true;

        public override void OnValidate()
        {
            base.OnValidate();
            resistanceOhms = Mathf.Max(0.0001f, resistanceOhms);
            secondaryParameter = Mathf.Max(0f, secondaryParameter);
            outputImpedanceOhms = Mathf.Max(0.0001f, outputImpedanceOhms);
        }

        public override void Stamp(CircuitMatrix matrix)
        {
            if (!IsElectrical)
            {
                return;
            }

            if (!HasValidNodes(terminal0, terminal1))
            {
                return;
            }

            float conductance = 1f / resistanceOhms;
            matrix.StampConductance(NodeOrDefault(terminal0), NodeOrDefault(terminal1), conductance);
        }

        public override void Step(float deltaTime, CircuitMatrix matrix)
        {
            if (TerminalCount >= 2)
            {
                float va = ReadVoltage(matrix, terminal0);
                float vb = ReadVoltage(matrix, terminal1);
                debugCurrentAmps = (va - vb) / resistanceOhms;
                debugState = va - vb;
            }

            if (TerminalCount > 0)
            {
                debugVoltageVolts = ReadVoltage(matrix, 0);
            }

            debugCurrent = debugCurrentAmps;
            debugInternalState = debugState;
        }
    }
}
