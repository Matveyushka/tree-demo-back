using Structuralist.Parser;

namespace Structuralist.M1;

public static class M1Grammar
{
    public static Grammar Instance => new Grammar(new List<GrammarRule>() 
    {
        GrammarRule.FromString("START = MODEL", Actions.finish),

        GrammarRule.FromString("MODEL = MODULES CREATE", Actions.model),

        GrammarRule.FromString("MODULES = MODULE", Actions.createModules),
        GrammarRule.FromString("MODULES = MODULES MODULE", Actions.fillModules),

        GrammarRule.FromString("MODULE = module identifier", Actions.createModule),
        GrammarRule.FromString("MODULE = MODULE PARAMETER", Actions.addParameterToModule),
        GrammarRule.FromString("MODULE = MODULE FEATURE", Actions.addFeatureToModule),
        GrammarRule.FromString("MODULE = MODULE FEATURERULE", Actions.addFeatureRuleToModule),
        GrammarRule.FromString("MODULE = MODULE GENERATE", Actions.addGenerateRuleToModule),

        GrammarRule.FromString("PARAMETER = parameter identifier real real", Actions.createParameter),
        GrammarRule.FromString("PARAMETER = parameter identifier number real", Actions.createParameter),
        GrammarRule.FromString("PARAMETER = parameter identifier real number", Actions.createParameter),
        GrammarRule.FromString("PARAMETER = parameter identifier number number", Actions.createParameter),

        GrammarRule.FromString("FEATURE = feature identifier FEATUREVALUE", Actions.createFeature),
        GrammarRule.FromString("FEATURE = FEATURE FEATUREVALUE", Actions.fillFeature),

        GrammarRule.FromString("FEATURERULE = CONDITIONS CONSEQUENCES", Actions.featureRule),

        GrammarRule.FromString("CONDITIONS = case FEATURECONSTRAINT", Actions.featureConstraintList),
        GrammarRule.FromString("CONDITIONS = CONDITIONS , FEATURECONSTRAINT", Actions.fillFeatureConstraintList),

        GrammarRule.FromString("CONSEQUENCES = constraint FEATURECONSTRAINT", Actions.featureConstraintList),
        GrammarRule.FromString("CONSEQUENCES = CONSEQUENCES , FEATURECONSTRAINT", Actions.fillFeatureConstraintList),

        GrammarRule.FromString("GENERATE = GENERATECOMMANDS", Actions.emptyGenerate),
        GrammarRule.FromString("GENERATE = CONDITIONS GENERATECOMMANDS", Actions.conditionedGenerate),
        GrammarRule.FromString("GENERATE = RESTRICTIONS GENERATECOMMANDS", Actions.restrictedGenerate),
        GrammarRule.FromString("GENERATE = CONDITIONS RESTRICTIONS GENERATECOMMANDS", Actions.fullGenerate),

        GrammarRule.FromString("RESTRICTIONS = restrict FEATURECONSTRAINT", Actions.featureConstraintList),
        GrammarRule.FromString("RESTRICTIONS = RESTRICTIONS , FEATURECONSTRAINT", Actions.fillFeatureConstraintList),

        GrammarRule.FromString("GENERATECOMMANDS = generate GENERATECOMMAND", Actions.createGenerateCommands),
        GrammarRule.FromString("GENERATECOMMANDS = GENERATECOMMANDS , GENERATECOMMAND", Actions.fillGenerateCommands),

        GrammarRule.FromString("GENERATECOMMAND = MATHEXPRESSION modules identifier", Actions.generateCommand),
        GrammarRule.FromString("GENERATECOMMAND = MATHEXPRESSION modules identifier identifier", Actions.generateCommandWithAlias),

        GrammarRule.FromString("FEATURECONSTRAINT = identifier FEATUREVALUE", Actions.createFeatureConstraint),
        GrammarRule.FromString("FEATURECONSTRAINT = FEATURECONSTRAINT FEATUREVALUE", Actions.fillFeatureConstraint),

        GrammarRule.FromString("FEATUREVALUE = number", Actions.featureValueFromNumber),
        GrammarRule.FromString("FEATUREVALUE = enumvalue", Actions.featureValueFromEnum),

        GrammarRule.FromString("CREATE = create identifier", Actions.createCommand),
        GrammarRule.FromString("CREATE = create identifier limit number", Actions.createCommandWithLimit),
    });
}