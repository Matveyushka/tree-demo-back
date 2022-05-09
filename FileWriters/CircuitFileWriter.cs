using SpiceSharp;
using SpiceSharp.Components;
using SpiceSharp.Entities;

public class CircuitFileWriter
{
    private void WriteSubcircuit(StreamWriter writer, Subcircuit sub)
    {
        var header = $".SUBCKT {sub.Name}";
        foreach (var pin in sub.Parameters.Definition.Pins)
        {
            header += $" {pin}";
        }
        writer.WriteLine(header);

        WriteEntities(writer, sub.Parameters.Definition.Entities);

        writer.WriteLine($".ENDS");
        writer.WriteLine();
    }

    private void WriteEntities(StreamWriter writer, ICollection<IEntity> entities)
    {
        foreach (var item in entities)
        {
            writer.WriteLine(item switch
            {
                VoltageSource vs => $"{vs.Name} {vs.Nodes[0]} {vs.Nodes[1]} ac {vs.Parameters.AcMagnitude:f9}".Replace(',', '.'),
                Inductor i => $"{i.Name} {i.Nodes[0]} {i.Nodes[1]} {i.Parameters.Inductance:f9}".Replace(',', '.'),
                Capacitor c => $"{c.Name} {c.Nodes[0]} {c.Nodes[1]} {c.Parameters.Capacitance.Value:f9}".Replace(',', '.'),
                Resistor r => $"{r.Name} {r.Nodes[0]} {r.Nodes[1]} {r.Parameters.Resistance.Value:f9}".Replace(',', '.'),
                Subcircuit s => $"X{s.Name} {s.Nodes[0]} {s.Nodes[1]} {s.Name}",
                _ => "???"
            });
        }
    }

    public void Write(Circuit ckt, string path)
    {
        using (var writer = new StreamWriter(path))
        {
            var entities = ckt.OrderBy(item => item.Name);
            writer.WriteLine();

            foreach (var entity in entities)
            {
                if (entity is Subcircuit s)
                {
                    WriteSubcircuit(writer, s);
                }
            }

            WriteEntities(writer, entities.ToList());

            writer.WriteLine(".AC DEC 334 1E2 1E5");
            writer.WriteLine(".PLOT AC (2*V(LOAD))");
            writer.WriteLine(".OPTIONS TEMP=27 ");
            writer.WriteLine(".end");
        }
    }
}