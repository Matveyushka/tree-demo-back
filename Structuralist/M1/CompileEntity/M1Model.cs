namespace Structuralist.M1;

public class M1Model
{
    public List<Module> Modules { get; set; } = new List<Module>();
    public CreateCommand CreateCommand { get; set; } = new CreateCommand();
}