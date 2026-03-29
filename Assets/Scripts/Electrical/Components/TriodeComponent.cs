using UnityEngine;
using VR.Electrical.Core;

namespace VR.Electrical.Components
{
    /// <summary>
    /// Triode model generated from CircuitJS1 reference file TriodeElm.java.
    /// Simplified for Unity breadboard integration with stable stamping/step behavior.
    /// TODO(parity): compare against https://github.com/pfalstad/circuitjs1/blob/master/src/com/lushprojects/circuitjs1/client/TriodeElm.java for full behavioral parity.
    /// </summary>
    public class TriodeComponent : ElectricalComponentBase
    {
[Header("Model Parameters")]
[Tooltip("Primary model parameter (ohms, gain, or equivalent scalar)")]
[SerializeField] private float primaryParameter = 1000f;

[Tooltip("Secondary model parameter used for simplified parity")]
[SerializeField] private float secondaryParameter = 1f;

[Tooltip("Output/source impedance in ohms")]
[SerializeField] private float outputImpedanceOhms = 1000f;

[Header("Triode Model")]
[Tooltip("Triode amplification factor (mu)")]
[SerializeField] private float triodeMu = 20f;

[Tooltip("Triode transconductance scalar")]
[SerializeField] private float triodeGain = 0.002f;

        [Tooltip("Terminal index 0")]
        [SerializeField] private int terminal0 = 0;

        [Tooltip("Terminal index 1")]
        [SerializeField] private int terminal1 = 1;

        [Tooltip("Terminal index 2")]
        [SerializeField] private int terminal2 = 2;

        [Header("Runtime Debug")]
        [Tooltip("Last simulated terminal differential voltage")]
        [SerializeField] private float debugVoltageVolts;

        [Tooltip("Last simulated branch current")]
        [SerializeField] private float debugCurrent;

        [Tooltip("Last internal state scalar")]
        [SerializeField] private float debugInternalState;

        public override ElectricalComponentKind Kind => ElectricalComponentKind.Triode;

        public override int TerminalCount => 3;

        public override bool IsElectrical => true;

        public override void OnValidate()
        {
            base.OnValidate();
            primaryParameter = Mathf.Max(0.0001f, primaryParameter);
            secondaryParameter = Mathf.Max(0f, secondaryParameter);
            outputImpedanceOhms = Mathf.Max(0.0001f, outputImpedanceOhms);
            triodeMu = Mathf.Max(0.1f, triodeMu);
            triodeGain = Mathf.Max(0.000001f, triodeGain);
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

            if (!HasValidNodes(terminal0, terminal1, terminal2))
            {
                return;
            }

            int plateNode = NodeOrDefault(terminal0);
            int gridNode = NodeOrDefault(terminal1);
            int cathodeNode = NodeOrDefault(terminal2);
            float plateConductance = 1f / Mathf.Max(0.0001f, outputImpedanceOhms);
            float plateVoltage = ReadVoltage(matrix, terminal0);
            float gridVoltage = ReadVoltage(matrix, terminal1);
            float cathodeVoltage = ReadVoltage(matrix, terminal2);
            float gridCathode = gridVoltage - cathodeVoltage;
            float plateCurrent = triodeGain * Mathf.Max(0f, gridCathode + (plateVoltage - cathodeVoltage) / triodeMu);

            matrix.StampConductance(plateNode, cathodeNode, plateConductance);
            matrix.StampCurrentSource(plateNode, cathodeNode, plateCurrent);
        }

        public override void Step(float deltaTime, CircuitMatrix matrix)
        {
            if (TerminalCount >= 3)
            {
                float va = ReadVoltage(matrix, terminal0);
                float vb = ReadVoltage(matrix, terminal1);
                float vc = ReadVoltage(matrix, terminal2);
                debugCurrentAmps = triodeGain * Mathf.Max(0f, (vb - vc) + (va - vc) / triodeMu);
                debugState = vb - vc;
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
