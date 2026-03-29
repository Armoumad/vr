using UnityEngine;
using VR.Electrical.Core;

namespace VR.Electrical.Components
{
    /// <summary>
    /// Labeled Node model generated from CircuitJS1 reference file LabeledNodeElm.java.
    /// Simplified for Unity breadboard integration with stable stamping/step behavior.
    /// TODO(parity): compare against https://github.com/pfalstad/circuitjs1/blob/master/src/com/lushprojects/circuitjs1/client/LabeledNodeElm.java for full behavioral parity.
    /// </summary>
    public class LabeledNodeComponent : ElectricalComponentBase
    {
[Header("Model Parameters")]
[Tooltip("Primary model parameter (ohms, gain, or equivalent scalar)")]
[SerializeField] private float primaryParameter = 1000f;

[Tooltip("Secondary model parameter used for simplified parity")]
[SerializeField] private float secondaryParameter = 1f;

[Tooltip("Output/source impedance in ohms")]
[SerializeField] private float outputImpedanceOhms = 1000f;

        [Tooltip("Terminal index 0")]
        [SerializeField] private int terminal0 = 0;

        [Header("Runtime Debug")]
        [Tooltip("Last simulated terminal differential voltage")]
        [SerializeField] private float debugVoltageVolts;

        [Tooltip("Last simulated branch current")]
        [SerializeField] private float debugCurrent;

        [Tooltip("Last internal state scalar")]
        [SerializeField] private float debugInternalState;

        public override ElectricalComponentKind Kind => ElectricalComponentKind.LabeledNode;

        public override int TerminalCount => 1;

        public override bool IsElectrical => false;

        public override void OnValidate()
        {
            base.OnValidate();
            primaryParameter = Mathf.Max(0.0001f, primaryParameter);
            secondaryParameter = Mathf.Max(0f, secondaryParameter);
            outputImpedanceOhms = Mathf.Max(0.0001f, outputImpedanceOhms);
        }

        public override void Stamp(CircuitMatrix matrix)
        {
            if (!IsElectrical)
            {
                return;
            }

            // Visualization-only component. No matrix stamping required.
        }

        public override void Step(float deltaTime, CircuitMatrix matrix)
        {
            debugState = 0f;

            if (TerminalCount > 0)
            {
                debugVoltageVolts = ReadVoltage(matrix, 0);
            }

            debugCurrent = debugCurrentAmps;
            debugInternalState = debugState;
        }
    }
}
