using System.Collections.Generic;
using System.Linq;

public enum ItemsGroupType
{
    ENUM,
    INTEGER
}

public class ItemsGroup 
{
    public string Name { get; set; }

    public ItemsGroupType Type { get; set; }

    public List<string> Items { get; set; }

    public ItemsGroup() {}

    public ItemsGroup(ItemsGroup source)
    {
        Name = source.Name;
        Items = source.Items.Select(x => x).ToList();
    }
}