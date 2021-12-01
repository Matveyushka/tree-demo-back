namespace tree_demo_back
{
    public class ModuleGenerateInstruction
    {
        public int ModuleIndex { get; set; }
        public int Amount { get; set; }

        public ModuleGenerateInstruction(int moduleIndex, int amount)
        {
            this.ModuleIndex = moduleIndex;
            this.Amount = amount;
        }
        public ModuleGenerateInstruction(ModuleGenerateInstruction source)
        {
            if (source is not null)
            {
                this.ModuleIndex = source.ModuleIndex;
                this.Amount = source.Amount;
            }
        }
    }
}