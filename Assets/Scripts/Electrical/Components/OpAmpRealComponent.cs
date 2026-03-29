using UnityEngine;
using VR.Electrical.Core;

namespace VR.Electrical.Components
{
    /// <summary>
    /// Op-Amp (Real) model generated from CircuitJS1 reference file OpAmpRealElm.java.
    /// Simplified for Unity breadboard integration with stable stamping/step behavior.
    /// TODO(parity): compare against https://github.com/pfalstad/circuitjs1/blob/master/src/com/lushprojects/circuitjs1/client/OpAmpRealElm.java for full behavioral parity.
    /// </summary>
    public class OpAmpRealComponent : ElectricalComponentBase
    {
[Header("Model Parameters")]
[Tooltip("Primary model parameter (ohms, gain, or equivalent scalar)")]
[SerializeField] private float primaryParameter = 1000f;

[Tooltip("Secondary model parameter used for simplified parity")]
[SerializeField] private float secondaryParameter = 1f;

[Tooltip("Output/source impedance in ohms")]
[SerializeField] private float outputImpedanceOhms = 1000f;

[Header("Op-Amp Limits")]
[Tooltip("Open-loop gain used for differential amplification")]
[SerializeField] private float openLoopGain = 100000f;

[Tooltip("Positive saturation rail in volts")]
[SerializeField] private float positiveRailVoltage = 12f;

[Tooltip("Negative saturation rail in volts")]
[SerializeField] private float negativeRailVoltage = -12f;

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

        public override ElectricalComponentKind Kind => ElectricalComponentKind.OpAmpReal;

        public override int TerminalCount => 3;

        public override bool IsElectrical => true;

        public override void OnValidate()
        {
            base.OnValidate();
            primaryParameter = Mathf.Max(0.0001f, primaryParameter);
            secondaryParameter = Mathf.Max(0f, secondaryParameter);
            outputImpedanceOhms = Mathf.Max(0.0001f, outputImpedanceOhms);
            openLoopGain = Mathf.Max(1f, openLoopGain);
            positiveRailVoltage = Mathf.Clamp(positiveRailVoltage, -1000f, 1000f);
            negativeRailVoltage = Mathf.Clamp(negativeRailVoltage, -1000f, 1000f);
            if (Mathf.Approximately(positiveRailVoltage, negativeRailVoltage))
            {
                positiveRailVoltage += 0.001f;
            }
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

            float vp = ReadVoltage(matrix, terminal0);
            float vn = ReadVoltage(matrix, terminal1);
            float differential = vp - vn;
            float lowerRail = Mathf.Min(negativeRailVoltage, positiveRailVoltage);
            float upperRail = Mathf.Max(negativeRailVoltage, positiveRailVoltage);
            float commandedOutput = Mathf.Clamp(differential * openLoopGain, lowerRail, upperRail);
            float outputConductance = 1f / Mathf.Max(0.0001f, outputImpedanceOhms);
            int outputNode = NodeOrDefault(terminal2);

            matrix.StampConductance(outputNode, -1, outputConductance);
            matrix.StampCurrentSource(outputNode, -1, commandedOutput * outputConductance);
        }

        public override void Step(float deltaTime, CircuitMatrix matrix)
        {
            if (TerminalCount >= 3)
            {
                float va = ReadVoltage(matrix, terminal0);
                float vb = ReadVoltage(matrix, terminal1);
                float differential = va - vb;
                float lowerRail = Mathf.Min(negativeRailVoltage, positiveRailVoltage);
                float upperRail = Mathf.Max(negativeRailVoltage, positiveRailVoltage);
                float commandedOutput = Mathf.Clamp(differential * openLoopGain, lowerRail, upperRail);
                debugCurrentAmps = commandedOutput / Mathf.Max(0.0001f, outputImpedanceOhms);
                debugState = differential;
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
