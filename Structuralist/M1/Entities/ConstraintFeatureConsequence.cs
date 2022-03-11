using System.Collections.Generic;

namespace Structuralist.M1;

public class ConstraintFeatureConsequence : ConstraintFeature{
    public string FeatureName { get; set; }
    public List<string> ValidOptions { get; set; }
}