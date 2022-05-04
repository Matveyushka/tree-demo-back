using SpiceSharp;
using SpiceSharp.Simulations;

public class CircuitSimulator
{
    public List<double> Simulate(Circuit ckt)
    {
        var result = new List<double>();

        var ac = new AC("AC 1", new LinearSweep(1.0e2, 1.0e4, 100));

        var exportVoltage = new ComplexVoltageExport(ac, "load");

        ac.ExportSimulationData += (sender, args) =>
        {
            var output = exportVoltage.Value;
            var decs = 10.0 * Math.Log10(output.Real * output.Real + output.Imaginary * output.Imaginary);
            result.Add(decs);
        };
        ac.Run(ckt);

        return result;
    }
}