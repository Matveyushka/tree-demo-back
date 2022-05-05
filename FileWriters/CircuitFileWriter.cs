using SpiceSharp;
using SpiceSharp.Components;

public class CircuitFileWriter
{
    public void Write(Circuit ckt, string path)
    {
        using (var writer = new StreamWriter(path))
        {
            var entities = ckt.OrderBy(item => item.Name);
            writer.WriteLine();
            foreach(var item in entities)
            {
                writer.WriteLine(item switch {
                    VoltageSource vs => $"{vs.Name} {vs.Nodes[0]} {vs.Nodes[1]} ac {vs.Parameters.AcMagnitude:f9}".Replace(',', '.'),
                    Inductor i => $"{i.Name} {i.Nodes[0]} {i.Nodes[1]} {i.Parameters.Inductance:f9}".Replace(',', '.'),
                    Capacitor c => $"{c.Name} {c.Nodes[0]} {c.Nodes[1]} {c.Parameters.Capacitance.Value:f9}".Replace(',', '.'),
                    Resistor r => $"{r.Name} {r.Nodes[0]} {r.Nodes[1]} {r.Parameters.Resistance.Value:f9}".Replace(',', '.'),
                    _ => "???"
                });
            }
            writer.WriteLine(".AC DEC 334 1E2 1E5");
            writer.WriteLine(".PLOT AC V(LOAD)");
            writer.WriteLine(".OPTIONS TEMP=27 ");
            writer.WriteLine(".end");
        }
    }
}