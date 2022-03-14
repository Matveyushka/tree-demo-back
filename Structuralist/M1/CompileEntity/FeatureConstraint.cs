namespace Structuralist.M1;

public class FeatureConstraint
{
    public string FeatureName { get; set; } = null!;
    public List<string> ValidValues { get; set; } = new List<string>();
}