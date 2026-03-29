using UnityEngine;
using VR.Electrical.Core;

namespace VR.Electrical.Components
{
    /// <summary>
    /// VCCS model generated from CircuitJS1 reference file VCCSElm.java.
    /// Simplified for Unity breadboard integration with stable stamping/step behavior.
    /// TODO(parity): compare against https://github.com/pfalstad/circuitjs1/blob/master/src/com/lushprojects/circuitjs1/client/VCCSElm.java for full behavioral parity.
    /// </summary>
    public class VccsComponent : ElectricalComponentBase
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

        [Tooltip("Terminal index 1")]
        [SerializeField] private int terminal1 = 1;

        [Tooltip("Terminal index 2")]
        [SerializeField] private int terminal2 = 2;

        [Tooltip("Terminal index 3")]
        [SerializeField] private int terminal3 = 3;

        [Header("Runtime Debug")]
        [Tooltip("Last simulated terminal differential voltage")]
        [SerializeField] private float debugVoltageVolts;

        [Tooltip("Last simulated branch current")]
        [SerializeField] private float debugCurrent;

        [Tooltip("Last internal state scalar")]
        [SerializeField] private float debugInternalState;

        public override ElectricalComponentKind Kind => ElectricalComponentKind.Vccs;

        public override int TerminalCount => 4;

        public override bool IsElectrical => true;

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

            if (!HasValidNodes(terminal0, terminal1))
            {
                return;
            }

            float conductance = 1f / Mathf.Max(0.0001f, primaryParameter);
            matrix.StampConductance(NodeOrDefault(terminal0), NodeOrDefault(terminal1), conductance);

                        // TODO(parity): refine multi-pin coupling from CircuitJS1 model.
                        if (HasValidNodes(terminal0, terminal2)) matrix.StampConductance(NodeOrDefault(terminal0), NodeOrDefault(terminal2), conductance * 0.25f);
                        if (HasValidNodes(terminal0, terminal3)) matrix.StampConductance(NodeOrDefault(terminal0), NodeOrDefault(terminal3), conductance * 0.25f);
        }

        public override void Step(float deltaTime, CircuitMatrix matrix)
        {
            if (TerminalCount >= 2)
            {
                float va = ReadVoltage(matrix, terminal0);
                float vb = ReadVoltage(matrix, terminal1);
                debugCurrentAmps = (va - vb) / Mathf.Max(0.0001f, primaryParameter);
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
