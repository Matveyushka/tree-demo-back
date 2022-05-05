using Structuralist.M1;

namespace Structuralist.M1Tree;

class BasicTreeCreator
{
    private List<ModuleInstanceName> moduleList;
    private List<Feature> moduleFeatures;

    public BasicTreeCreator(
        Module module,
        List<ModuleInstanceName>? moduleList = null,
        List<Feature>? restrictions = null,
        int moduleNumber = 0,
        string? alias = null)
    {
        this.moduleFeatures = GetModuleFeatures(
            module,
            restrictions);

        this.moduleList = GetCurrentModuleList(
            module,
            moduleList ?? new List<ModuleInstanceName>(),
            moduleNumber,
            alias);
    }

    List<Feature> GetRestrictedModuleFeatures(Module module, List<Feature> restrictions)
    {
        var restrictedNames = restrictions
            .Select(feature => feature.Name);

        return module
            .Features
            .Select(feature => restrictedNames.Contains(feature.Name)
                ? restrictions.First(f => f.Name == feature.Name)
                : feature)
            .ToList();
    }

    List<Feature> GetModuleFeatures(Module module, List<Feature>? restrictions)
    {
        var restrictionsAreNotEmpty = restrictions is not null && restrictions.Any();

        return restrictionsAreNotEmpty
            ? GetRestrictedModuleFeatures(module, restrictions!)
            : module.Features;
    }

    ModuleInstanceName GetCurrentModuleInstanceName(
        Module module,
        int moduleNumber,
        string? alias) => new ModuleInstanceName
        {
            Name = alias ?? module.Name,
            Index = moduleNumber
        };

    List<ModuleInstanceName> GetCurrentModuleList(
        Module module,
        List<ModuleInstanceName> moduleList,
        int moduleNumber,
        string? alias) => moduleList
            .Concat(new List<ModuleInstanceName>() {
                GetCurrentModuleInstanceName(module, moduleNumber, alias)
            }).ToList();

    public BuilderTreeNode CreateBasicTree()
    {
        var tree = CreateRootNode();

        var treeNodePointer = tree;

        for (int featureIndex = 0; featureIndex != this.moduleFeatures.Count; featureIndex++)
        {
            treeNodePointer
                .Children
                .Add(CreateChoiceNode(featureIndex));

            treeNodePointer = CreateNextLevelOfTree(
                treeNodePointer,
                featureIndex);
        }

        return tree;
    }

    BuilderTreeNode CreateRootNode()
    {
        var root = new BuilderTreeNode()
        {
            Type = TreeNodeType.AND,
            Content = this.moduleFeatures.Select(feautre => new TreeNodeValue()
            {
                ModuleList = this.moduleList,
                Value = new Feature(feautre)
            }).ToList()
        };

        root.Content.Add(new TreeNodeValue()
        {
            ModuleList = this.moduleList,
            Value = null
        });

        return root;
    }

    private BuilderTreeNode CreateNextLevelOfTree(
        BuilderTreeNode treeNodePointer,
        int featureIndex)
    {
        if (featureIndex != this.moduleFeatures.Count - 1)
        {
            var nextAndNode = CreateNextLevelRoot(
                featureIndex);

            treeNodePointer.Children.Add(nextAndNode);
            treeNodePointer = nextAndNode;
        }

        return treeNodePointer;
    }

    private BuilderTreeNode CreateNextLevelRoot(int featureIndex) => new BuilderTreeNode()
    {
        Type = TreeNodeType.AND,
        Children = new List<BuilderTreeNode>(),
        Content = this.moduleFeatures.Where((group, index) => index > featureIndex)
                .Select(feature => new TreeNodeValue()
                {
                    ModuleList = this.moduleList,
                    Value = new Feature(feature)
                }).ToList()
    };

    private BuilderTreeNode CreateChoiceNode(int featureIndex) => new BuilderTreeNode()
    {
        Type = TreeNodeType.OR,
        Children = this.moduleFeatures[featureIndex]
                .Values
                .Select(
                    item => CreateChoiceOptionNode(
                        featureIndex,
                        item)
                ).ToList(),
    };

    private BuilderTreeNode CreateChoiceOptionNode(
        int featureIndex,
        string item) => new BuilderTreeNode()
        {
            Type = TreeNodeType.OR,
            Children = new List<BuilderTreeNode>(),
            Content = new List<TreeNodeValue>()
                {
                    new TreeNodeValue() {
                        ModuleList = this.moduleList,
                        Value = new Feature() {
                            Name = this.moduleFeatures[featureIndex].Name,
                            Values = new List<string>() { item },
                            Type = this.moduleFeatures[featureIndex].Type
                        }
                    }
                }
        };
}