namespace Structuralist.M1;

public class Module
{
    public string Name { get; set; } = null!;
    public List<RealParameter> Parameters { get; set; } = new List<RealParameter>();
    public List<Feature> Features { get; set; } = new List<Feature>();
    public List<FeatureRule> FeatureRules { get; set; } = new List<FeatureRule>();
    public List<GenerateRule> GenerateRules { get; set; } = new List<GenerateRule>();
}