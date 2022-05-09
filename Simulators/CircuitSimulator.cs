using SpiceSharp;
using SpiceSharp.Simulations;

public class CircuitSimulator
{
    public Dictionary<double, double> Simulate(Circuit ckt)
    {
        var result = new Dictionary<double, double>();

        var ac = new AC("AC 1", new DecadeSweep(1e2, 1e5, 50));

        var exportVoltage = new ComplexVoltageExport(ac, "load");

        ac.ExportSimulationData += (sender, args) =>
        {
            var output = exportVoltage.Value;
            var decs = 20.0 * Math.Log10(Math.Sqrt(output.Real * output.Real + output.Imaginary * output.Imaginary) * 2);
            result.Add(args.Frequency, decs);
        };
        ac.Run(ckt);

        return result;
    }
}