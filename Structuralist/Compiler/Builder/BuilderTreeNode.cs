using System.Collections.Generic;
using System.Linq;

namespace tree_demo_back
{
    public class BuilderTreeNode
    {
        public TreeNodeType Type { get; set; }
        public List<BuilderTreeNode> Children { get; set; }
        public List<ItemsGroup> Content { get; set; }
        public List<IntegerItemsGroupValue> SavedValues { get; set; }
        public List<string> ModuleList { get; set; }

        public ModuleGenerateInstruction GenerateInstruction { get; set; }

        public BuilderTreeNode()
        {
            Children = new List<BuilderTreeNode>();
            Content = new List<ItemsGroup>();
            SavedValues = new List<IntegerItemsGroupValue>();
            ModuleList = new List<string>();
            GenerateInstruction = null;
        }

        public BuilderTreeNode(BuilderTreeNode source)
        {
            Type = source.Type;
            Content = source.Content?.Select(c => new ItemsGroup(c)).ToList();
            Children = source.Children?.Select(c => new BuilderTreeNode(c)).ToList();
            SavedValues = source.SavedValues.Select(c => new IntegerItemsGroupValue(c)).ToList();
            ModuleList = source.ModuleList?.Select(c => c).ToList();
            if (source.GenerateInstruction is not null)
            {
                GenerateInstruction = new ModuleGenerateInstruction(source.GenerateInstruction);
            }  
        }
    }
}