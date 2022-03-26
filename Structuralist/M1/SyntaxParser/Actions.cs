using Structuralist.MathExpression;
using Structuralist.Parser;

namespace Structuralist.M1;
public static class Actions
{
    public static Func<List<object>, CreateCommand> createCommand = values =>
        new CreateCommand()
        {
            ModuleName = ((Identifier)values[0]).Value
        };

    public static Func<List<object>, CreateCommand> createCommandWithLimit = values =>
        new CreateCommand()
        {
            ModuleName = ((Identifier)values[2]).Value,
            Limit = ((NumberLiteral)values[0]).Value
        };

    public static Func<List<object>, string> featureValueFromNumber = values =>
        ((NumberLiteral)values[0]).Value.ToString();

    public static Func<List<object>, string> featureValueFromEnum = values =>
        ((EnumLiteral)values[0]).Value;

    public static Func<List<object>, Feature> createFeatureConstraint = values =>
        new Feature() {
            Name = ((Identifier)values[1]).Value,
            Values = new List<string>() { (string)values[0] },
            Type = FeatureType.ENUM
        };

    public static Func<List<object>, Feature> fillFeatureConstraint = values =>
        {
            var featureConstraint = (Feature)values[1];
            featureConstraint.Values.Add((string)values[0]);
            return featureConstraint;
        };

    public static Func<List<object>, GenerateCommand> generateCommand = values =>
        new GenerateCommand()
        {
            QuantityExpression = (Expression)values[2],
            ModuleName = ((Identifier)values[0]).Value
        };

    public static Func<List<object>, GenerateCommand> generateCommandWithAlias = values =>
        new GenerateCommand()
        {
            QuantityExpression = (Expression)values[3],
            ModuleName = ((Identifier)values[1]).Value,
            Alias = ((Identifier)values[0]).Value
        };

    public static Func<List<object>, List<GenerateCommand>> createGenerateCommands = values =>
        new List<GenerateCommand>()
        {
            (GenerateCommand)values[0]
        };

    public static Func<List<object>, List<GenerateCommand>> fillGenerateCommands = values =>
        {
            var commands = (List<GenerateCommand>)values[2];
            commands.Add((GenerateCommand)values[0]);
            return commands;
        };

    public static Func<List<object>, List<Feature>> featureConstraintList = values =>
        new List<Feature>() {
            (Feature)values[0]
        };

    public static Func<List<object>, List<Feature>> fillFeatureConstraintList = values =>
        {
            var list = (List<Feature>)values[2];
            list.Add((Feature)values[0]);
            return list;
        };

    public static Func<List<object>, GenerateRule> emptyGenerate = values =>
        new GenerateRule()
        {
            Command = (List<GenerateCommand>)values[0]
        };

    public static Func<List<object>, GenerateRule> conditionedGenerate = values =>
        new GenerateRule()
        {
            Conditions = (List<Feature>)values[1],
            Command = (List<GenerateCommand>)values[0]
        };

    public static Func<List<object>, GenerateRule> restrictedGenerate = values =>
        new GenerateRule()
        {
            Restrictions = (List<Feature>)values[1],
            Command = (List<GenerateCommand>)values[0]
        };

    public static Func<List<object>, GenerateRule> fullGenerate = values =>
        new GenerateRule()
        {
            Conditions = (List<Feature>)values[2],
            Restrictions = (List<Feature>)values[1],
            Command = (List<GenerateCommand>)values[0]
        };

    public static Func<List<object>, FeatureRule> featureRule = values =>
        new FeatureRule()
        {
            Conditions = (List<Feature>)values[1],
            Consequences = (List<Feature>)values[0]
        };

    public static Func<List<object>, Feature> createFeature = values =>
        new Feature() {
            Name = ((Identifier)values[1]).Value,
            Values = new List<string>() { (string)values[0] },
            Type = int.TryParse((string)values[0], out var _) 
                ? FeatureType.INTEGER
                : FeatureType.ENUM
        };


    public static Func<List<object>, Feature> fillFeature = values =>
        {
            var feature = (Feature)values[1];
            feature.Values.Add((string)values[0]);
            if (int.TryParse((string)values[0], out var _) == false)
            {
                feature.Type = FeatureType.ENUM;
            }
            return feature;
        };

    public static Func<List<object>, Module> createModule = values =>
        new Module()
        {
            Name = ((Identifier)values[0]).Value
        };

    public static Func<List<object>, Module> addFeatureToModule = values =>
        {
            var module = (Module)values[1];
            module.Features.Add((Feature)values[0]);
            return module;
        };

    public static Func<List<object>, Module> addFeatureRuleToModule = values =>
        {
            var module = (Module)values[1];
            module.FeatureRules.Add((FeatureRule)values[0]);
            return module;
        };

    public static Func<List<object>, Module> addGenerateRuleToModule = values =>
        {
            var module = (Module)values[1];
            module.GenerateRules.Add((GenerateRule)values[0]);
            return module;
        };

    public static Func<List<object>, List<Module>> createModules = values =>
        new List<Module>() {
            (Module)values[0]
        };

    public static Func<List<object>, List<Module>> fillModules = values =>
        {
            var modules = (List<Module>)values[1];
            modules.Add((Module)values[0]);
            return modules;
        };

    public static Func<List<object>, M1Model> model = values =>
        new M1Model()
        {
            Modules = (List<Module>)values[1],
            CreateCommand = (CreateCommand)values[0]
        };

    public static Func<List<object>, M1Model> finish = values =>
        (M1Model)values[0];
}
