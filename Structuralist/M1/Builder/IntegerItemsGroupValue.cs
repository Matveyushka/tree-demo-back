namespace Structuralist.M1;

public class IntegerItemsGroupValue
{
    public string Name { get; set; }
    public int Value { get; set; }

    public IntegerItemsGroupValue(string name, int value)
    {
        this.Name = name;
        this.Value = value;
    }

    public IntegerItemsGroupValue(IntegerItemsGroupValue source)
    {
        this.Name = source.Name;
        this.Value = source.Value;
    }
}