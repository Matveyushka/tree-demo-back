namespace Structuralist.M1;

public class FeatureRule
{
    public List<FeatureConstraint> Conditions { get; set; } = new List<FeatureConstraint>();
    public List<FeatureConstraint> Consequences { get; set; } = new List<FeatureConstraint>();
}