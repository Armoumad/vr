using UnityEngine;
using VR.Electrical.Core;

namespace VR.Electrical.Components
{
    /// <summary>
    /// FM Source model generated from CircuitJS1 reference file FMElm.java.
    /// Simplified for Unity breadboard integration with stable stamping/step behavior.
    /// TODO(parity): compare against https://github.com/pfalstad/circuitjs1/blob/master/src/com/lushprojects/circuitjs1/client/FMElm.java for full behavioral parity.
    /// </summary>
    public class FmSourceComponent : ElectricalComponentBase
    {
[Header("Model Parameters")]
[Tooltip("Primary model parameter (ohms, gain, or equivalent scalar)")]
[SerializeField] private float primaryParameter = 1000f;

[Tooltip("Secondary model parameter used for simplified parity")]
[SerializeField] private float secondaryParameter = 1f;

[Tooltip("Output/source impedance in ohms")]
[SerializeField] private float outputImpedanceOhms = 1000f;

[Header("Source Model")]
[Tooltip("Generated source voltage in volts")]
[SerializeField] private float sourceVoltageVolts = 5f;

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

        public override ElectricalComponentKind Kind => ElectricalComponentKind.FmSource;

        public override int TerminalCount => 2;

        public override bool IsElectrical => true;

        public override void OnValidate()
        {
            base.OnValidate();
            primaryParameter = Mathf.Max(0.0001f, primaryParameter);
            secondaryParameter = Mathf.Max(0f, secondaryParameter);
            outputImpedanceOhms = Mathf.Max(0.0001f, outputImpedanceOhms);
            sourceVoltageVolts = Mathf.Clamp(sourceVoltageVolts, -1000f, 1000f);
        }

        public override void Stamp(CircuitMatrix matrix)
        {
            if (!IsElectrical)
            {
                return;
            }

            if (TerminalCount < 2 || !HasValidNodes(terminal0, terminal1))
            {
                return;
            }

            float sourceCurrent = sourceVoltageVolts / Mathf.Max(0.0001f, sourceImpedanceOhms);
            matrix.StampCurrentSource(NodeOrDefault(terminal0), NodeOrDefault(terminal1), sourceCurrent);
        }

        public override void Step(float deltaTime, CircuitMatrix matrix)
        {
            debugState = sourceVoltageVolts;

            if (TerminalCount > 0)
            {
                debugVoltageVolts = ReadVoltage(matrix, 0);
            }

            debugCurrent = debugCurrentAmps;
            debugInternalState = debugState;
        }
    }
}
