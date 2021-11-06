using System.Collections.Generic;
using System.Linq;

public class ItemsGroup 
{
    public string Name { get; set; }

    public List<string> Items { get; set; }

    public ItemsGroup() {}

    public ItemsGroup(ItemsGroup source)
    {
        Name = source.Name;
        Items = source.Items.Select(x => x).ToList();
    }
}