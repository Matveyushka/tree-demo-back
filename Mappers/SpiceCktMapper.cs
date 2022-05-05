using SpiceSharp;
using SpiceSharp.Components;
using Structuralist.M2.Output;

public class SpiceCktMapper
{
    private void AddLc(Structuralist.M2.Output.Module module, Circuit ckt, List<List<Port>> cktLinks)
    {
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
                ckt.Add(new Inductor($"L{i + 1}", plusName, minusName, inductor.Parameters["Inductance"] * 1e-3));
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
                ckt.Add(new Capacitor($"C{i + 1}", plusName, minusName, capacitor.Parameters["Capacity"] * 1e-9));
            }
        }
    }

    public Circuit Map(Structuralist.M2.Output.Module module)
    {
        Circuit ckt = new Circuit(
            new VoltageSource("V1", "in", "0", 0)
            .SetParameter("acmag", 1.0)
        );

        var cktLinks = new List<List<Port>>();

        if (module.Name == "LPF" || module.Name == "HPF")
        {
            AddLc(module, ckt, cktLinks);

            ckt.Add(new Resistor("R1", "0", "load", 5.0e3));

            return ckt;
        }

        throw new Exception($"НЕ МОГУ РАСПОЗНАТЬ МОДУЛЬ {module.Name}");
    }
}