namespace Structuralist.M1;

public class CreateCommand
{
    public string ModuleName { get; set; } = null!;
    public int Limit { get; set; } = 5;
}