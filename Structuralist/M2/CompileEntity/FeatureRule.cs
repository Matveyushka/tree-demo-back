using System.Collections.Generic;

namespace Structuralist.M2;

public class FeatureRule
{
    public string FeatureName { get; set; } = null!;

    public List<FeatureRuleCase> Cases { get; set; } = new List<FeatureRuleCase>();
}