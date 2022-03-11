using System.Collections.Generic;

namespace Structuralist.M2;

public class FeatureRuleCase 
{
    public string FeatureValue { get; set; }

    public List<LinkRule> LinkRules { get; set; } = new List<LinkRule>();
}