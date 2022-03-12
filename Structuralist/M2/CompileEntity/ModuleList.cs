namespace Structuralist.M2;

public class ModuleList
{
    public string Name { get; set; } = null!;
    public List<LinkRule> LinkRules { get; set; } = new List<LinkRule>();
}