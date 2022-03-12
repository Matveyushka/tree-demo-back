namespace Structuralist.M2;

public class ModulePosition 
{
    public string ModuleName { get; set; } = null!;

    public List<PositionRule> Rules { get; set; } = new List<PositionRule>();
}