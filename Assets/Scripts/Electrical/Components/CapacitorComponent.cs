using UnityEngine;
using VR.Electrical.Core;

namespace VR.Electrical.Components
{
    /// <summary>
    /// Capacitor model generated from CircuitJS1 reference file CapacitorElm.java.
    /// Simplified for Unity breadboard integration with stable stamping/step behavior.
    /// TODO(parity): compare against https://github.com/pfalstad/circuitjs1/blob/master/src/com/lushprojects/circuitjs1/client/CapacitorElm.java for full behavioral parity.
    /// </summary>
    public class CapacitorComponent : ElectricalComponentBase
    {
[Header("Model Parameters")]
[Tooltip("Capacitance in farads")]
[SerializeField] private float capacitanceFarads = 0.000001f;

[Tooltip("Secondary model parameter used for simplified parity")]
[SerializeField] private float secondaryParameter = 1f;

[Tooltip("Output/source impedance in ohms")]
[SerializeField] private float outputImpedanceOhms = 1000f;

[Tooltip("Last capacitor voltage for stable companion stamping")]
[SerializeField] private float previousCapacitorVoltage;

[Tooltip("Last simulation timestep received from Step()")]
[SerializeField] private float lastStepDeltaTime = 0.02f;

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

        public override ElectricalComponentKind Kind => ElectricalComponentKind.Capacitor;

        public override int TerminalCount => 2;

        public override bool IsElectrical => true;

        public override void OnValidate()
        {
            base.OnValidate();
            capacitanceFarads = Mathf.Max(0.000000000001f, capacitanceFarads);
            secondaryParameter = Mathf.Max(0f, secondaryParameter);
            outputImpedanceOhms = Mathf.Max(0.0001f, outputImpedanceOhms);
            lastStepDeltaTime = Mathf.Max(0.000001f, lastStepDeltaTime);
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

            float dt = Mathf.Max(lastStepDeltaTime, 0.000001f);
            float companionConductance = (2f * capacitanceFarads) / dt;
            float companionCurrent = companionConductance * previousCapacitorVoltage;
            int nodeA = NodeOrDefault(terminal0);
            int nodeB = NodeOrDefault(terminal1);

            matrix.StampConductance(nodeA, nodeB, companionConductance);
            matrix.StampCurrentSource(nodeA, nodeB, companionCurrent);
        }

        public override void Step(float deltaTime, CircuitMatrix matrix)
        {
            if (deltaTime > 0f)
            {
                lastStepDeltaTime = deltaTime;
            }

            if (TerminalCount >= 2)
            {
                float va = ReadVoltage(matrix, terminal0);
                float vb = ReadVoltage(matrix, terminal1);
                float dv = va - vb;
                float dt = Mathf.Max(lastStepDeltaTime, 0.000001f);
                debugCurrentAmps = capacitanceFarads * ((dv - previousCapacitorVoltage) / dt);
                previousCapacitorVoltage = dv;
                debugState = dv;
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
