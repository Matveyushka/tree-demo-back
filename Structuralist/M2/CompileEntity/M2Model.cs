namespace Structuralist.M2;

public class M2Model
{
    public List<Module> Modules { get; set; } = new List<Module>();

    private Structuralist.M2.Output.PortIndex GeneratePortIndex(Port port)
    {
        if (int.TryParse(port.PortIndex, out int numberPortIndex))
        {
            return new Structuralist.M2.Output.SimplePortIndex(numberPortIndex);
        }
        else
        {
            var portNumber = int.Parse(port.PortIndex.Substring(0, port.PortIndex.Length - 1));
            var direction = port.PortIndex.Last() switch
            {
                'w' => PortDirection.WEST,
                'n' => PortDirection.NORTH,
                'e' => PortDirection.EAST,
                's' => PortDirection.SOUTH,
                _ => throw new Exception("Illegal port direction!")
            };
            return new Structuralist.M2.Output.DirectedPortIndex(portNumber, direction);
        }
    }

    private Structuralist.M2.Output.Port GeneratePort(
        Port port,
        int moduleIndex,
        int moduleQuantity)
    {
        if (port is SelfPort selfPort)
        {
            return new Structuralist.M2.Output.SelfPort(GeneratePortIndex(port));
        }
        else if (port is ChildPort childPort)
        {
            return new Structuralist.M2.Output.ChildPort(
                childPort.ModuleName,
                childPort.ModuleIndex.Calculate(new Dictionary<string, int>() { { "I", moduleIndex }, { "N", moduleQuantity } }),
                GeneratePortIndex(port));
        }
        else
        {
            throw new Exception("Port is invalid");
        }
    }

    public Structuralist.M2.Output.Module GenerateStructure(Structuralist.M1M2.ModuleIdentifier moduleIdentifier, int? x = null, int? y = null)
    {
        var M2Module = Modules.First(module => module.Name == moduleIdentifier.Name);

        var submodules = new Dictionary<string, List<Structuralist.M2.Output.Module>>();

        foreach (var submoduleName in moduleIdentifier.Submodules.Keys)
        {
            submodules.Add(submoduleName, new List<Structuralist.M2.Output.Module>());
            var submodulesQuantity = moduleIdentifier.Submodules[submoduleName].Count;
            var realIndex = 0;
            for (var submoduleIndex = 0; submoduleIndex != moduleIdentifier.Submodules[submoduleName].Count; submoduleIndex++)
            {
                var positionRules = M2Module
                    .PositionRules
                    .FirstOrDefault(positionRule => positionRule.ModuleName == submoduleName)?
                    .Rules
                    .FirstOrDefault(rule => rule.Conditions.All(condition => condition.Invoke(submoduleIndex, submodulesQuantity)));

                int? subX = null;
                int? subY = null;

                if (positionRules is not null)
                {
                    subX = positionRules.Position.X.Calculate(new Dictionary<string, int>() { { "I", realIndex }, { "N", submodulesQuantity } });
                    subY = positionRules.Position.Y.Calculate(new Dictionary<string, int>() { { "I", realIndex }, { "N", submodulesQuantity } });
                }

                if (moduleIdentifier.Submodules[submoduleName][submoduleIndex] is not null)
                {
                    submodules[submoduleName].Add(GenerateStructure(
                        moduleIdentifier.Submodules[submoduleName][submoduleIndex], subX, subY
                    ));
                    realIndex++;
                }
            }
        }

        var links = new List<Structuralist.M2.Output.Link>();

        foreach (var feature in moduleIdentifier.Features)
        {
            var featureRule = M2Module.FeatureRules.FirstOrDefault(rule => rule.FeatureName == feature.Key);
            var featureCase = featureRule?.Cases.FirstOrDefault(ruleCase => ruleCase.FeatureValue == feature.Value.ToString());
            if (featureCase is not default(FeatureRuleCase))
            {
                foreach (var moduleList in featureCase.ModuleLists)
                {
                    foreach (var linkRule in moduleList.LinkRules)
                    {
                        if (moduleIdentifier.Submodules.Keys.Contains(moduleList.Name) == false)
                        {
                            continue;
                        }
                        for (int i = 0; i != submodules[moduleList.Name].Count; i++)
                        {
                            bool appropriate = true;
                            foreach (var condition in linkRule.Conditions)
                            {
                                appropriate = appropriate && condition.Invoke(i, submodules[moduleList.Name].Count);
                            }
                            if (appropriate)
                            {
                                foreach (var link in linkRule.Links)
                                {
                                    links.Add(new Output.Link(
                                        GeneratePort(link.From, i, submodules[moduleList.Name].Count),
                                        GeneratePort(link.To, i, submodules[moduleList.Name].Count)
                                    ));
                                }
                            }
                        }
                    }
                }
            }
        }

        if (M2Module.PortMap is CirclePortMap circlePortMap)
        {
            return new Output.CircleModule(
                circlePortMap.Ports, 
                M2Module.Name, 
                submodules, 
                links, 
                moduleIdentifier.Features, 
                moduleIdentifier.Parameters,
                x, 
                y);
        }
        else if (M2Module.PortMap is RectanglePortMap rectanglePortMap)
        {
            return new Output.RectangleModule(
                rectanglePortMap.West, 
                rectanglePortMap.North, 
                rectanglePortMap.East, 
                rectanglePortMap.South, 
                M2Module.Name, 
                submodules, 
                links, 
                moduleIdentifier.Features,
                moduleIdentifier.Parameters, 
                x, 
                y);
        }
        else
        {
            throw new Exception("Portmap is invalid");
        }
    }
}