namespace Structuralist.M1;

public class GenerateRule
{
    public List<Feature> Conditions { get; set; } = new List<Feature>();
    public List<Feature> Restrictions { get; set; } = new List<Feature>();
    public List<GenerateCommand> Command { get; set; } = null!;
}