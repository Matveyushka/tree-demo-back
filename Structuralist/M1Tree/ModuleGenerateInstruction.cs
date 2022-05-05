using Structuralist.M1;

namespace Structuralist.M1Tree;

public class ModuleGenerateInstruction
{
    public int ModuleIndex { get; set; }
    public int Amount { get; set; }
    public List<Feature> Restrictions { get; set; }
    public string? Alias { get; set; }

    public ModuleGenerateInstruction(int moduleIndex, int amount, List<Feature> restrictions, string? alias)
    {
        this.ModuleIndex = moduleIndex;
        this.Amount = amount;
        this.Restrictions = restrictions;
        this.Alias = alias;
    }
    public ModuleGenerateInstruction(ModuleGenerateInstruction source)
    {
        if (source is not null)
        {
            this.ModuleIndex = source.ModuleIndex;
            this.Amount = source.Amount;
            this.Restrictions = new List<Feature>(source.Restrictions);
            this.Alias = source.Alias;
        }
        else 
        {
            throw new NullReferenceException("Copy constructor argument of ModuleGenerateInstruction must not be null");
        }
    }
}