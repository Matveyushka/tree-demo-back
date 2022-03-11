using System.Collections.Generic;

namespace Structuralist.M2;

public class FeatureRule
{
    public string FeatureName { get; set; }

    public List<FeatureRuleCase> Cases { get; set; } = new List<FeatureRuleCase>();
}