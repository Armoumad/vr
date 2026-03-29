using UnityEngine;
using VR.Electrical.Core;

namespace VR.Electrical.Components
{
    /// <summary>
    /// AND Gate model generated from CircuitJS1 reference file AndGateElm.java.
    /// Simplified for Unity breadboard integration with stable stamping/step behavior.
    /// TODO(parity): compare against https://github.com/pfalstad/circuitjs1/blob/master/src/com/lushprojects/circuitjs1/client/AndGateElm.java for full behavioral parity.
    /// </summary>
    public class AndGateComponent : ElectricalComponentBase
    {
[Header("Model Parameters")]
[Tooltip("Primary model parameter (ohms, gain, or equivalent scalar)")]
[SerializeField] private float primaryParameter = 1000f;

[Tooltip("Secondary model parameter used for simplified parity")]
[SerializeField] private float secondaryParameter = 1f;

[Tooltip("Output/source impedance in ohms")]
[SerializeField] private float outputImpedanceOhms = 1000f;

[Header("Logic Levels")]
[Tooltip("Minimum voltage interpreted as logic HIGH")]
[SerializeField] private float logicHighThreshold = 2.5f;

[Tooltip("Output voltage when logic state is HIGH")]
[SerializeField] private float outputHighVoltage = 5f;

[Tooltip("Output voltage when logic state is LOW")]
[SerializeField] private float outputLowVoltage = 0f;

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

        public override ElectricalComponentKind Kind => ElectricalComponentKind.AndGate;

        public override int TerminalCount => 2;

        public override bool IsElectrical => true;

        public override void OnValidate()
        {
            base.OnValidate();
            primaryParameter = Mathf.Max(0.0001f, primaryParameter);
            secondaryParameter = Mathf.Max(0f, secondaryParameter);
            outputImpedanceOhms = Mathf.Max(0.0001f, outputImpedanceOhms);
            logicHighThreshold = Mathf.Clamp(logicHighThreshold, -100f, 100f);
            outputHighVoltage = Mathf.Clamp(outputHighVoltage, -1000f, 1000f);
            outputLowVoltage = Mathf.Clamp(outputLowVoltage, -1000f, 1000f);
        }

        public override void Stamp(CircuitMatrix matrix)
        {
            if (!IsElectrical)
            {
                return;
            }

            if (!HasValidNodes(terminal0))
            {
                return;
            }

            matrix.StampConductance(NodeOrDefault(terminal0), -1, 0.000001f);
        }

        public override void Step(float deltaTime, CircuitMatrix matrix)
        {
            float vin = ReadVoltage(matrix, terminal0);
            bool high = vin >= logicHighThreshold;
            debugState = high ? 1f : 0f;

            if (TerminalCount > 1 && HasValidNodes(terminal1))
            {
                float target = high ? outputHighVoltage : outputLowVoltage;
                float gain = 1f / Mathf.Max(0.0001f, outputImpedanceOhms);
                matrix.StampCurrentSource(NodeOrDefault(terminal1), NodeOrDefault(terminal0), target * gain);
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
