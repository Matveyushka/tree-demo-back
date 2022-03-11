using System.Collections.Generic;
using System.Text;

namespace Structuralist.M1;

public class ConstraintModuleConsequence : ConstraintFeature
{
    public StringBuilder ExpressionString { get; set; } = new StringBuilder("");
    public MathExpression Expression { get; set; }
    public string ModuleName { get; set; }
    public string ModuleAlias { get; set; }
}
