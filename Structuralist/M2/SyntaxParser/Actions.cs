using Structuralist.Parser;

namespace Structuralist.M2;
public static class Actions
{
    public static Func<List<object>, object> selfPort = values =>
        new SelfPort()
        {
            PortIndex = values[0].ToString() ?? ""
        };

    public static Func<List<object>, object> childPort = values =>
        new ChildPort()
        {
            ModuleName = ((Identifier)values[6]).Value,
            ModuleIndex = ((Structuralist.MathExpression.Expression)values[4]),
            PortIndex = values[1].ToString() ?? ""
        };

    public static Func<List<object>, LinkDescription> linkModules = value =>
        new LinkDescription()
        {
            From = (Port)value[2],
            To = (Port)value[0]
        };

    public static Func<List<object>, List<LinkDescription>> createLinkList = values =>
        new List<LinkDescription>() { (LinkDescription)values[0] };

    public static Func<List<object>, List<LinkDescription>> addLinkToList = values =>
        ((List<LinkDescription>)values[2]).Append((LinkDescription)values[0]).ToList();

    public static Func<List<object>, Func<int, int, bool>> ruleNumber = values =>
        (index, amount) => index == ((NumberLiteral)values[0]).Value;

    public static Func<List<object>, Func<int, int, bool>> ruleRule = values =>
        (index, amount) =>
        {
            var str = ((RuleLiteral)values[0]).Value;
            if (str == "First") return index == 0;
            if (str == "Last") return index == amount - 1;
            var num = int.Parse(str.Substring(1, str.Length - 1));
            return index % num == 0;
        };

    public static Func<List<object>, Func<int, int, bool>> ruleNotNumber = values =>
        (index, amount) => index != ((NumberLiteral)values[0]).Value;

    public static Func<List<object>, Func<int, int, bool>> ruleNotRule = values =>
        (index, amount) =>
        {
            var str = ((RuleLiteral)values[0]).Value;
            if (str == "First") return index != 0;
            if (str == "Last") return index != amount - 1;
            var num = int.Parse(str.Substring(1, str.Length - 1));
            return index % num != 0;
        };

    public static Func<List<object>, List<Func<int, int, bool>>> createRuleList = values =>
        new List<Func<int, int, bool>>() { (Func<int, int, bool>)values[0] };

    public static Func<List<object>, List<Func<int, int, bool>>> addRuleToList = values =>
        ((List<Func<int, int, bool>>)values[2]).Append((Func<int, int, bool>)values[0]).ToList();

    public static Func<List<object>, LinkRule> createLinkRule = values =>
        new LinkRule()
        {
            Conditions = (List<Func<int, int, bool>>)values[2],
            Links = (List<LinkDescription>)values[0]
        };

    public static Func<List<object>, ModuleList> moduleList = values =>
        new ModuleList()
        {
            Name = ((Identifier)values[1]).Value,
            LinkRules = new List<LinkRule>() { (LinkRule)values[0] }
        };

    public static Func<List<object>, ModuleList> addRuleToModuleList = values =>
        {
            ((ModuleList)values[1]).LinkRules.Add((LinkRule)values[0]);
            return (ModuleList)values[1];
        };

    public static Func<List<object>, FeatureRuleCase> featureCase = values =>
        new FeatureRuleCase()
        {
            FeatureValue = ((Identifier)values[1]).Value,
            ModuleLists = new List<ModuleList>() { (ModuleList)values[0] }
        };

    public static Func<List<object>, FeatureRuleCase> addRuleToFeatureCase = values =>
        {
            ((FeatureRuleCase)values[1]).ModuleLists.Add((ModuleList)values[0]);
            return (FeatureRuleCase)values[1];
        };

    public static Func<List<object>, FeatureRule> feature = values =>
        new FeatureRule()
        {
            FeatureName = ((Identifier)values[1]).Value,
            Cases = new List<FeatureRuleCase>() { (FeatureRuleCase)values[0] }
        };

    public static Func<List<object>, ModulePosition> modulePosition = values =>
        new ModulePosition()
        {
            ModuleName = ((Identifier)values[1]).Value,
            Rules = new List<PositionRule>() { (PositionRule)values[0] }
        };

    public static Func<List<object>, ModulePosition> addRuleToModulePosition = values =>
        {
            ((ModulePosition)values[1]).Rules.Add((PositionRule)values[0]);
            return (ModulePosition)values[1];
        };


    public static Func<List<object>, PositionRule> positionRule = values =>
        new PositionRule()
        {
            Conditions = (List<Func<int, int, bool>>)values[4],
            Position = new Position((Structuralist.MathExpression.Expression)values[2], (Structuralist.MathExpression.Expression)values[0])
        };

    public static Func<List<object>, FeatureRule> addCaseToRule = values =>
        {
            ((FeatureRule)values[1]).Cases.Add((FeatureRuleCase)values[0]);
            return (FeatureRule)values[1];
        };

    public static Func<List<object>, CirclePortMap> circlePortMap = values =>
        new CirclePortMap()
        {
            Ports = ((NumberLiteral)values[0]).Value
        };

    public static Func<List<object>, RectanglePortMap> rectanglePortMap = values =>
        new RectanglePortMap()
        {
            West = ((NumberLiteral)values[6]).Value,
            North = ((NumberLiteral)values[4]).Value,
            East = ((NumberLiteral)values[2]).Value,
            South = ((NumberLiteral)values[0]).Value,
        };

    public static Func<List<object>, Module> moduleBeginWithFeature = values =>
        new Module()
        {
            Name = ((Identifier)values[2]).Value,
            PortMap = (PortMap)values[1],
            FeatureRules = new List<FeatureRule>() { (FeatureRule)values[0] }
        };

    public static Func<List<object>, Module> moduleBeginWithPosition = values =>
        new Module()
        {
            Name = ((Identifier)values[2]).Value,
            PortMap = (PortMap)values[1],
            PositionRules = new List<ModulePosition>() { (ModulePosition)values[0] }
        };


    public static Func<List<object>, Module> emptyModule = values =>
        new Module()
        {
            Name = ((Identifier)values[1]).Value,
            PortMap = (PortMap)values[0],
            FeatureRules = new List<FeatureRule>()
        };

    public static Func<List<object>, Module> addPositionToModule = values =>
        {
            ((Module)values[1]).PositionRules.Add((ModulePosition)values[0]);
            return (Module)values[1];
        };


    public static Func<List<object>, Module> addFeatureToModule = values =>
        {
            ((Module)values[1]).FeatureRules.Add((FeatureRule)values[0]);
            return (Module)values[1];
        };

    public static Func<List<object>, M2Model> model = values =>
        new M2Model()
        {
            Modules = new List<Module>() {
                (Module)values[0]
            }
        };

    public static Func<List<object>, M2Model> fillModel = values =>
    {
        ((M2Model)values[1]).Modules.Add((Module)values[0]);
        return (M2Model)values[1];
    };

    public static Func<List<object>, M2Model> finish = values => (M2Model)values[0];
}