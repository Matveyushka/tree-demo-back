using Structuralist.Parser;

namespace Structuralist.M2;

public static class M2Grammar
{
    public static Grammar Instance => new Grammar(new List<GrammarRule>() 
    {
        GrammarRule.FromString("START = MODULES", Actions.finish),

        GrammarRule.FromString("MODULES = MODULE", Actions.model),
        GrammarRule.FromString("MODULES = MODULES MODULE", Actions.fillModel),
        GrammarRule.FromString("MODULES = EMPTYMODULE", Actions.model),
        GrammarRule.FromString("MODULES = MODULES EMPTYMODULE", Actions.fillModel),

        GrammarRule.FromString("EMPTYMODULE = structura identifier PORTSDESCRIPTION", Actions.emptyModule),

        GrammarRule.FromString("MODULE = structura identifier PORTSDESCRIPTION FEATURE", Actions.moduleBeginWithFeature),
        GrammarRule.FromString("MODULE = MODULE FEATURE", Actions.addFeatureToModule),
        GrammarRule.FromString("MODULE = structura identifier PORTSDESCRIPTION POSITION", Actions.moduleBeginWithPosition),
        GrammarRule.FromString("MODULE = MODULE POSITION", Actions.addPositionToModule),

        GrammarRule.FromString("PORTSDESCRIPTION = number", Actions.circlePortMap),
        GrammarRule.FromString("PORTSDESCRIPTION = number - number - number - number", Actions.rectanglePortMap),

        GrammarRule.FromString("POSITION = position identifier POSITIONRULE", Actions.modulePosition),
        GrammarRule.FromString("POSITION = POSITION POSITIONRULE", Actions.addRuleToModulePosition),

        GrammarRule.FromString("POSITIONRULE = on RULES place MATHEXPRESSION , MATHEXPRESSION", Actions.positionRule),

        GrammarRule.FromString("FEATURE = feature identifier FEATURECASE", Actions.feature),
        GrammarRule.FromString("FEATURE = FEATURE FEATURECASE", Actions.addCaseToRule),

        GrammarRule.FromString("FEATURECASE = case enumvalue LIST", Actions.featureCase),
        GrammarRule.FromString("FEATURECASE = FEATURECASE LIST", Actions.addRuleToFeatureCase),

        GrammarRule.FromString("LIST = list identifier LINKRULE", Actions.moduleList),
        GrammarRule.FromString("LIST = LIST LINKRULE", Actions.addRuleToModuleList),

        GrammarRule.FromString("LINKRULE = on RULES link LINKS", Actions.createLinkRule),

        GrammarRule.FromString("RULES = RULE", Actions.createRuleList),
        GrammarRule.FromString("RULES = RULES , RULE", Actions.addRuleToList),

        GrammarRule.FromString("RULE = rule", Actions.ruleRule),
        GrammarRule.FromString("RULE = number", Actions.ruleNumber),
        GrammarRule.FromString("RULE = not number", Actions.ruleNotNumber),
        GrammarRule.FromString("RULE = not rule", Actions.ruleNotRule),

        GrammarRule.FromString("LINKS = LINK", Actions.createLinkList),
        GrammarRule.FromString("LINKS = LINKS , LINK", Actions.addLinkToList),

        GrammarRule.FromString("LINK = MODULEPORT - MODULEPORT", Actions.linkModules),

        GrammarRule.FromString("MODULEPORT = identifier [ MATHEXPRESSION ] ( number )", Actions.childPort),
        GrammarRule.FromString("MODULEPORT = identifier [ MATHEXPRESSION ] ( portindex )", Actions.childPort),
        GrammarRule.FromString("MODULEPORT = port number", Actions.selfPort),
        GrammarRule.FromString("MODULEPORT = port portindex", Actions.selfPort),
    });
}