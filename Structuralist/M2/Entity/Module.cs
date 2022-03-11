using System.Collections.Generic;

namespace Structuralist.M2;

public class Module
{
    public string Name { get; set; }
    public PortMap PortMap { get; set; }

    public List<FeatureRule> FeatureRules { get; set; } = new List<FeatureRule>();
}