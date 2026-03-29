namespace VR.Electrical.Core
{
    /// <summary>
    /// Solver contract for components that stamp and step in the circuit simulation loop.
    /// </summary>
    public interface IStampable
    {
        void Stamp(CircuitMatrix matrix);
        void Step(float deltaTime, CircuitMatrix matrix);
    }
}
