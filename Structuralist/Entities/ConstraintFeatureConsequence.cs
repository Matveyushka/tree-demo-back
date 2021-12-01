using System.Collections.Generic;

public class ConstraintFeatureConsequence : ConstraintFeature{
    public string FeatureName { get; set; }
    public List<string> ValidOptions { get; set; }
}