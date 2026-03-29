using UnityEngine;
using VR.Electrical.Core;

namespace VR.Electrical.Components
{
    /// <summary>
    /// Switch (SPST) model generated from CircuitJS1 reference file SwitchElm.java.
    /// Simplified for Unity breadboard integration with stable stamping/step behavior.
    /// TODO(parity): compare against https://github.com/pfalstad/circuitjs1/blob/master/src/com/lushprojects/circuitjs1/client/SwitchElm.java for full behavioral parity.
    /// </summary>
    public class SwitchSpstComponent : ElectricalComponentBase
    {
[Header("Model Parameters")]
[Tooltip("Primary model parameter (ohms, gain, or equivalent scalar)")]
[SerializeField] private float primaryParameter = 1000f;

[Tooltip("Secondary model parameter used for simplified parity")]
[SerializeField] private float secondaryParameter = 1f;

[Tooltip("Output/source impedance in ohms")]
[SerializeField] private float outputImpedanceOhms = 1000f;

[Header("Discrete State")]
[Tooltip("True selects alternate conducting path")]
[SerializeField] private bool state;

[Tooltip("Closed-contact resistance (ohms)")]
[SerializeField] private float onResistanceOhms = 0.001f;

[Tooltip("Open-contact resistance (ohms)")]
[SerializeField] private float offResistanceOhms = 1000000000f;

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

        public override ElectricalComponentKind Kind => ElectricalComponentKind.SwitchSpst;

        public override int TerminalCount => 2;

        public override bool IsElectrical => true;

public void Toggle()
{
    state = !state;
}

        public override void OnValidate()
        {
            base.OnValidate();
            primaryParameter = Mathf.Max(0.0001f, primaryParameter);
            secondaryParameter = Mathf.Max(0f, secondaryParameter);
            outputImpedanceOhms = Mathf.Max(0.0001f, outputImpedanceOhms);
            onResistanceOhms = Mathf.Clamp(onResistanceOhms, 0.000001f, 1000f);
            offResistanceOhms = Mathf.Max(1000f, offResistanceOhms);
            if (offResistanceOhms <= onResistanceOhms)
            {
                offResistanceOhms = onResistanceOhms * 1000f;
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

            float resistance = state ? onResistanceOhms : offResistanceOhms;
            float conductance = 1f / resistance;
            matrix.StampConductance(NodeOrDefault(terminal0), NodeOrDefault(terminal1), conductance);
            debugState = state ? 1f : 0f;
        }

        public override void Step(float deltaTime, CircuitMatrix matrix)
        {
                        // No dynamic update required for this simplified model.

            if (TerminalCount > 0)
            {
                debugVoltageVolts = ReadVoltage(matrix, 0);
            }

            debugCurrent = debugCurrentAmps;
            debugInternalState = debugState;
        }
    }
}
