using System.Collections.Generic;

namespace Structuralist.M1;

public class Constraint 
{
    public List<ConstraintCondition> Conditions { get; set; }
    public ConstraintFeature Consequence { get; set; }
}