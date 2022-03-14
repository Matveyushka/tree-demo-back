namespace Structuralist.M1;

public class GenerateRule
{
    public List<FeatureConstraint> Conditions { get; set; } = new List<FeatureConstraint>();
    public List<FeatureConstraint> Restrictions { get; set; } = new List<FeatureConstraint>();
    public List<GenerateCommand> Command { get; set; } = null!;
}