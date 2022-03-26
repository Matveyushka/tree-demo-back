namespace Structuralist.M1;

public enum FeatureType
{
    ENUM,
    INTEGER
}

public class Feature
{
    public string Name { get; set; } = null!;
    public List<string> Values { get; set; } = new List<string>();
    public FeatureType Type { get; set; }

    public Feature() { }

    public Feature(Feature feature)
    {
        this.Name = feature.Name;
        this.Values = new List<string>(feature.Values);
        this.Type = feature.Type;
    }
}