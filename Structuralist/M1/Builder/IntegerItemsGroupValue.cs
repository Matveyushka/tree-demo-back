namespace Structuralist.M1;

public class IntegerItemsGroupValue
{
    public string Name { get; set; }
    public int Value { get; set; }

    public IntegerItemsGroupValue()
    {
    }

    public IntegerItemsGroupValue(IntegerItemsGroupValue source)
    {
        this.Name = source.Name;
        this.Value = source.Value;
    }
}