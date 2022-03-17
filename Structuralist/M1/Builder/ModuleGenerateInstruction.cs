namespace Structuralist.M1;

public class ModuleGenerateInstruction
{
    public int ModuleIndex { get; set; }
    public int Amount { get; set; }
    public List<Feature> Restrictions { get; set; }

    public ModuleGenerateInstruction(int moduleIndex, int amount, List<Feature> restrictions)
    {
        this.ModuleIndex = moduleIndex;
        this.Amount = amount;
        this.Restrictions = restrictions;
    }
    public ModuleGenerateInstruction(ModuleGenerateInstruction source)
    {
        if (source is not null)
        {
            this.ModuleIndex = source.ModuleIndex;
            this.Amount = source.Amount;
            this.Restrictions = new List<Feature>(source.Restrictions);
        }
    }
}