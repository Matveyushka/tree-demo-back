namespace Structuralist.M1;

public class FeatureRule
{
    public List<Feature> Conditions { get; set; } = new List<Feature>();
    public List<Feature> Consequences { get; set; } = new List<Feature>();

    public bool HasTopTreeFeatureInConditions(BuilderTreeNode tree) =>
        this.Conditions
            .FindIndex(condition => condition.Name == tree.TopContent.Name) != -1;

    public bool HasTopTreeFeatureInConsequence(BuilderTreeNode tree) =>
        this.Consequences
            .FindIndex(consequence => consequence.Name == tree.TopContent.Name) != -1;
}