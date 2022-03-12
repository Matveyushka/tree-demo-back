using System.Collections.Generic;

namespace Structuralist.M2;

public class Module
{
    public string Name { get; set; } = null!;
    public PortMap PortMap { get; set; } = null!;

    public List<FeatureRule> FeatureRules { get; set; } = new List<FeatureRule>();
    public List<ModulePosition> PositionRules { get; set; } = new List<ModulePosition>();
}