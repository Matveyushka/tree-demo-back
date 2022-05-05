using Genetic;
using Structuralist.M1M2;
using Structuralist.M1Tree;
using Structuralist.M2;

namespace BenchmarkTest;

static class ModuleEstimator
{
    public static double EstimateModule(Structuralist.M2.Output.Module module)
    {
        double estimation = 0;
        foreach (var submodules in module.Submodules)
        {
            foreach (var submodule in submodules.Value)
            {
                estimation += EstimateModule(submodule);
            }
        }
        return estimation + 1 + module.Links.Count;
    }

    public static Func<Genotype, double> GetEstimator(TreeNode[] tree, M2Model m2model){
        var treeList = tree.ToList();
        return id => EstimateModule(m2model.GenerateStructure(ModuleIdentifier.ExtractFrom(treeList, id)!));
    }  
}