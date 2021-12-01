using System.Collections.Generic;
using System.Text;

namespace tree_demo_back
{
    public class ConstraintModuleConsequence : ConstraintFeature
    {
        public StringBuilder ExpressionString { get; set; } = new StringBuilder("");
        public MathExpression Expression { get; set; }
        public string ModuleName { get; set; }
        public string ModuleAlias { get; set; }
    }
}
