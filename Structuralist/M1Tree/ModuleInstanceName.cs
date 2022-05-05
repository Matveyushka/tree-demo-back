namespace Structuralist.M1Tree;

public class ModuleInstanceName
{
    public string Name { get; set; } = null!;
    public int Index { get; set; }

    public ModuleInstanceName() { }

    public ModuleInstanceName(ModuleInstanceName source)
    {
        this.Name = source.Name;
        this.Index = source.Index;
    }

    public override bool Equals(object? obj)
    {
        if (obj is ModuleInstanceName anotherInstanceName)
        {
            return this.Name == anotherInstanceName.Name && this.Index == anotherInstanceName.Index;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode() ^ Index.GetHashCode() ^ 137;
    }
}
