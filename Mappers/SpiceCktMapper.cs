using SpiceSharp;
using SpiceSharp.Components;
using SpiceSharp.Entities;
using Structuralist.M2.Output;

public class SpiceCktMapper
{
    private List<List<Port>> LinkNodes(Structuralist.M2.Output.Module module)
    {
        var cktLinks = new List<List<Port>>();

        module.Links.ForEach(link =>
        {
            var cktLinkFrom = cktLinks.FirstOrDefault(l => l.Contains(link.From));
            var cktLinkTo = cktLinks.FirstOrDefault(l => l.Contains(link.To));

            if (cktLinkFrom is not null && cktLinkTo is not null)
            {
                if (cktLinkFrom != cktLinkTo)
                {
                    foreach (var port in cktLinkTo)
                    {
                        if (cktLinkFrom.Contains(port) == false)
                        {
                            cktLinkFrom.Add(port);
                        }
                    }
                    cktLinks.Remove(cktLinkTo);
                }
            }
            else if (cktLinkFrom is not null)
            {
                cktLinkFrom.Add(link.To);
            }
            else if (cktLinkTo is not null)
            {
                cktLinkTo.Add(link.From);
            }
            else if (cktLinkFrom is null && cktLinkTo is null)
            {
                cktLinks.Add(new List<Port> { link.From, link.To });
            }
            else
            {
                throw new Exception($"НИ ПАНЯТНА");
            }
        });

        return cktLinks;
    }

    private Subcircuit GetLc(Structuralist.M2.Output.Module module, int index, string pos, string neg)
    {
        if (module.Name != "HPF" && module.Name != "LPF" && module.Name != "BPF")
        {
            throw new ArgumentException("Module must be LC");
        }

        var entityCollection = new EntityCollection();

        var cktLinks = LinkNodes(module);

        var inputPort = new SelfPort(new DirectedPortIndex(0, Structuralist.M2.PortDirection.WEST));
        var loadPort = new SelfPort(new DirectedPortIndex(0, Structuralist.M2.PortDirection.EAST));
        var zeroPort = new SelfPort(new DirectedPortIndex(1, Structuralist.M2.PortDirection.WEST));

        int inIndex = cktLinks.IndexOf(cktLinks.First(cktLink => cktLink.Contains(inputPort)));
        int loadIndex = cktLinks.IndexOf(cktLinks.First(cktLink => cktLink.Contains(loadPort)));
        int zeroIndex = cktLinks.IndexOf(cktLinks.First(cktLink => cktLink.Contains(zeroPort)));

        if (module.Submodules.ContainsKey("Inductor"))
        {
            var inductors = module.Submodules["Inductor"];
            for (int i = 0; i != inductors.Count; i++)
            {
                var inductor = inductors[i];
                var minus = cktLinks.IndexOf(
                    cktLinks.First(
                        cktLink => cktLink.Where(
                            port => port is ChildPort cport
                            && cport.SubmoduleName == inductor.Name
                            && cport.SubmoduleIndex == i
                            && cport.PortIndex is DirectedPortIndex dindex
                            && dindex.PortDirection == Structuralist.M2.PortDirection.WEST).Count() > 0));
                var plus = cktLinks.IndexOf(
                    cktLinks.First(
                        cktLink => cktLink.Where(
                            port => port is ChildPort cport
                            && cport.SubmoduleName == inductor.Name
                            && cport.SubmoduleIndex == i
                            && cport.PortIndex is DirectedPortIndex dindex
                            && dindex.PortDirection == Structuralist.M2.PortDirection.EAST).Count() > 0));
                var minusName = minus == inIndex
                    ? "in"
                    : minus == loadIndex
                    ? "load"
                    : minus == zeroIndex
                    ? "0"
                    : $"_{minus}";
                var plusName = plus == inIndex
                    ? "in"
                    : plus == loadIndex
                    ? "load"
                    : plus == zeroIndex
                    ? "0"
                    : $"_{plus}";
                entityCollection.Add(new Inductor($"L{(index + 1) * 100 + i + 1}{module.Name}", plusName, minusName, inductor.Parameters["Inductance"] * 1e-3));
            }
        }

        if (module.Submodules.ContainsKey("Capacitor"))
        {
            var capacitors = module.Submodules["Capacitor"];
            for (int i = 0; i != capacitors.Count; i++)
            {
                var capacitor = capacitors[i];
                var minus = cktLinks.IndexOf(
                    cktLinks.First(
                        cktLink => cktLink.Where(
                            port => port is ChildPort cport
                            && cport.SubmoduleName == capacitor.Name
                            && cport.SubmoduleIndex == i
                            && cport.PortIndex is DirectedPortIndex dindex
                            && dindex.PortDirection == Structuralist.M2.PortDirection.NORTH).Count() > 0));
                var plus = cktLinks.IndexOf(
                    cktLinks.First(
                        cktLink => cktLink.Where(
                            port => port is ChildPort cport
                            && cport.SubmoduleName == capacitor.Name
                            && cport.SubmoduleIndex == i
                            && cport.PortIndex is DirectedPortIndex dindex
                            && dindex.PortDirection == Structuralist.M2.PortDirection.SOUTH).Count() > 0));
                var minusName = minus == inIndex
                    ? "in"
                    : minus == loadIndex
                    ? "load"
                    : minus == zeroIndex
                    ? "0"
                    : $"_{minus}";
                var plusName = plus == inIndex
                    ? "in"
                    : plus == loadIndex
                    ? "load"
                    : plus == zeroIndex
                    ? "0"
                    : $"_{plus}";
                entityCollection.Add(new Capacitor($"C{(index + 1) * 100 + i + 1}{module.Name}", plusName, minusName, capacitor.Parameters["Capacity"] * 1e-9));
            }
        }

        var cktSubDef = new SubcircuitDefinition(entityCollection, "load", "in");

        return new Subcircuit($"{module.Name}{index}", cktSubDef, pos, neg);
    }

    public Circuit Map(Structuralist.M2.Output.Module module)
    {
        Circuit ckt = new Circuit(
            new VoltageSource("V1", "_in", "0", 0)
            .SetParameter("acmag", 1.0)
        );

        var cktLinks = new List<List<Port>>();

        if (module.Name == "LPF" || module.Name == "HPF" || module.Name == "BPF")
        {
            ckt.Add(GetLc(module, 0, "load", "in"));
        }
        else if (module.Name == "Filter")
        {
            int intermediateNodeIndex = 0;
            var first = module.Submodules.First().Value.First();
            var last = module.Submodules.Last().Value.Last();
            foreach (var sub in module.Submodules)
            {          
                foreach (var value in sub.Value)
                {
                    var inIndex = value == first ? "in" : $"inter{intermediateNodeIndex++}";
                    var outIndex = value == last ? "load" : $"inter{intermediateNodeIndex}";
                    ckt.Add(GetLc(value, 0, outIndex, inIndex));
                }   
            }
        }
        else
        {
            throw new Exception($"НЕ МОГУ РАСПОЗНАТЬ МОДУЛЬ {module.Name}");
        }

        ckt.Add(new Resistor("RL", "0", "load", 5.0e3));
        ckt.Add(new Resistor("RIn", "in", "_in", 5.0e3));

        return ckt;
    }
}