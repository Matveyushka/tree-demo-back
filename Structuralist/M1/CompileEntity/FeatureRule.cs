namespace Structuralist.M1;

public class FeatureRule
{
    public List<Feature> Conditions { get; set; } = new List<Feature>();
    public List<Feature> Consequences { get; set; } = new List<Feature>();
}