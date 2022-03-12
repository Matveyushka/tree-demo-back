using System;
using System.Collections.Generic;
using System.Linq;

namespace Structuralist.M1;

public enum ParserDepth
{
    OUT,
    ROOT,
    MODULE,
    ITEMSGROUP,
    CONSTRAINT_CONDITION,
    CONSTRAINT_FEATURE_CONSEQUENCE,
    CONSTRAINT_MODULE_CONSEQUENCE_INSTRUCTION,
    CONSTRAINT_MODULE_CONSEQUENCE_POINTER,
    GENERATE_SPECIFYING,
    REPEAT
}

public delegate void TokenAction();

public delegate ParserDepth NextDepthGetter(ParserDepth currentDepth);

public class Rule
{
    public List<ParserDepth> ValidDepth { get; set; }

    public NextDepthGetter NextDepth { get; set; }

    public TokenAction PerformAction { get; set; }
}

public class Parser
{
    List<ModuleM1> modules;
    int resultCode = 0;
    string errorMessage = "";


    void error(string message)
    {
        resultCode = 1;
        errorMessage = message;
    }

    void AddNewModuleM1()
    {
        modules.Add(new ModuleM1()
        {
            ClassificationFeatures = new List<ItemsGroup>(),
            Constraints = new List<Constraint>()
        });
    }

    void AddItemsGroup()
    {
        modules.Last().ClassificationFeatures.Add(new ItemsGroup()
        {
            Items = new List<string>()
        });
    }

    void AddConstraint()
    {
        modules.Last().Constraints.Add(new Constraint()
        {
            Conditions = new List<ConstraintCondition>() {
                                    new ConstraintCondition() {
                                        ConditionOptions = new List<string>()
                                    }
                                }
        });
    }

    void AddCondition()
    {
        modules.Last().Constraints.Last().Conditions.Add(new ConstraintCondition()
        {
            ConditionOptions = new List<string>()
        });
    }

    void AddFeatureConsequence()
    {
        modules.Last().Constraints.Last().Consequence = new ConstraintFeatureConsequence()
        {
            ValidOptions = new List<string>()
        };
    }

    void AddModuleConsequence()
    {
        modules.Last().Constraints.Last().Consequence = new ConstraintModuleConsequence();
    }

    void CheckModuleNameDuplicates()
    {
        var moduleNames = modules.Select(module => module.Name);

        if (moduleNames.Count() != moduleNames.Distinct().Count())
        {
            error($"There are modules with the same names.");
        }
    }

    bool CheckModuleNameExistence(string name)
    {
        var moduleNames = modules.Select(module => module.Name).ToList();
        bool result = true;

        if (moduleNames.FindIndex(moduleName => moduleName == name) == -1)
        {
            error($"Name of the module to generate specified wrong: no such module exists.");
            result = false;
        }

        return result;
    }

    void CheckItemsGroupName()
    {
        var itemsGroupNames = modules.Last().ClassificationFeatures.Select(itemsGroup => itemsGroup.Name);

        if (itemsGroupNames.Count() != itemsGroupNames.Distinct().Count())
        {
            error($"There are classification features with the same names in the module {modules.Last().Name}.");
        }
    }

    void CheckItemsGroupOptions()
    {
        var options = modules.Last().ClassificationFeatures.Last().Items.Select(item => item);

        if (options.Count() != options.Distinct().Count())
        {
            error($"There are the same options in the module '{modules.Last().Name}', classification feature '{modules.Last().ClassificationFeatures.Last().Name}'.");
        }
    }

    void CheckConstraintConditionName()
    {
        var conditionName = modules.Last().Constraints.Last().Conditions.Last().ClassificationFeatureName;

        var featureNames = modules.Last().ClassificationFeatures.Select(feature => feature.Name).ToList();

        if (featureNames.FindIndex(name => name == conditionName) == -1)
        {
            error($"Unknown classification feature in constraint condition. Module: '{modules.Last().Name}'.");
        }
    }

    void CheckConstraintConsequenceName()
    {
        var consequenceName = (modules.Last().Constraints.Last().Consequence as ConstraintFeatureConsequence)
            .FeatureName;

        var featureNames = modules.Last().ClassificationFeatures.Select(feature => feature.Name).ToList();

        if (featureNames.FindIndex(name => name == consequenceName) == -1)
        {
            error($"Unknown classification feature in constraint consequence. Module: '{modules.Last().Name}'.");
        }
    }

    void CheckConstraintConditionOptions()
    {
        var options = modules.Last().Constraints.Last().Conditions.Last().ConditionOptions;

        if (options.Count() != options.Distinct().Count())
        {
            error($"There are the same options in constarint condition: module '{modules.Last().Name}'.");
        }

        var conditionName = modules.Last().Constraints.Last().Conditions.Last().ClassificationFeatureName;

        var targetItemsGroup = modules.Last().ClassificationFeatures.First(feature => feature.Name == conditionName);

        foreach (var option in options)
        {
            if (targetItemsGroup.Items.FindIndex(item => item == option) == -1)
            {
                error($"There is not an option '{option}' in classification feature '{targetItemsGroup.Name}', module: '{modules.Last().Name}'");
                break;
            }
        }
    }

    void CheckConstraintConsequeceOptions()
    {
        var consequence = modules.Last().Constraints.Last().Consequence as ConstraintFeatureConsequence;
        var options = consequence.ValidOptions;

        if (options.Count() != options.Distinct().Count())
        {
            error($"There are the same options in constarint consequence: module '{modules.Last().Name}'.");
        }

        var consequenceName = consequence.FeatureName;

        var targetItemsGroup = modules.Last().ClassificationFeatures.First(feature => feature.Name == consequenceName);

        foreach (var option in options)
        {
            if (targetItemsGroup.Items.FindIndex(item => item == option) == -1)
            {
                error($"There is not an option '{option}' in classification feature '{targetItemsGroup.Name}', module: '{modules.Last().Name}'");
                break;
            }
        }
    }

    void HandleItemsGroup()
    {
        var ItemsGroup = modules.Last().ClassificationFeatures.Last();
        if (ItemsGroup.Name == null)
        {
            error($"Items group of the '{modules.Last().Name}' module must have a name.");
        }
        else if (ItemsGroup.Items.Count == 0)
        {
            error($"Items group '{modules.Last().ClassificationFeatures.Last().Name}' of the '{modules.Last().Name}' module must not be empty.");
        }
        if (ItemsGroup.Items.All(item => int.TryParse(item, out var _)))
        {
            ItemsGroup.Type = ItemsGroupType.INTEGER;
        }
        else
        {
            ItemsGroup.Type = ItemsGroupType.ENUM;
        }
    }

    bool CheckConstraintCondition()
    {
        bool result = true;
        if (modules.Last().Constraints.Last().Conditions.Last().ClassificationFeatureName == null)
        {
            error($"No condition name. Module: {modules.Last().Name}.");
            result = false;
        }
        else if (modules.Last().Constraints.Last().Conditions.Last().ConditionOptions.Count == 0)
        {
            error($"Empty condition. Module: {modules.Last().Name}. Condition: {modules.Last().Constraints.Last().Conditions.Last().ClassificationFeatureName}");
            result = false;
        }
        return result;
    }

    bool CheckFeatureConstraintConsequence()
    {
        bool result = true;
        var consequence = modules.Last().Constraints.Last().Consequence as ConstraintFeatureConsequence;
        if (consequence.FeatureName == null)
        {
            error($"No consequence name. Module: {modules.Last().Name}.");
            result = false;
        }
        else if (consequence.ValidOptions.Count == 0)
        {
            error($"Empty consequence. Module: {modules.Last().Name}. Consequence: {consequence.FeatureName}");
            result = false;
        }
        return result;
    }

    bool CheckModuleConsequenceInstruction()
    {
        bool result = true;
        var consequence = modules.Last().Constraints.Last().Consequence as ConstraintModuleConsequence;
        var features = modules.Last().ClassificationFeatures;
        if (consequence.ExpressionString.Length == 0)
        {
            error($"Repeat expression cannot be empty");
            result = false;
        }

        var integerFeature = features.Where(feature => feature.Type == ItemsGroupType.INTEGER);

        var integerFeatureNames = integerFeature.Select(feature => feature.Name).ToList();

        consequence.Expression = new tempmath.MathExpression(consequence.ExpressionString.ToString(), integerFeatureNames);
        var exporessionError = consequence.Expression.GetError();

        if (exporessionError is not null)
        {
            error(exporessionError);
            result = false;
        }

        return result;
    }

    bool CheckModuleConsequencePointer()
    {
        bool result = true;
        var consequence = modules.Last().Constraints.Last().Consequence as ConstraintModuleConsequence;
        if (consequence.ModuleName == null)
        {
            error($"Module name must be provider in module constraint: module {modules.Last().Name}");
            result = false;
        }
        else if (!CheckModuleNameExistence(consequence.ModuleName))
        {
            error($"There is not a module with name {consequence.ModuleName}");
            result = false;
        }
        return result;
    }

    bool CheckFeatureExists(string name, ModuleM1 module)
    {
        return module.ClassificationFeatures.Where(feature => feature.Name == name).Count() != 0;
    }

    bool CheckFeatureIsInt(string name, ModuleM1 module)
    {
        return module.ClassificationFeatures.Find(feature => feature.Name == name).Type == ItemsGroupType.INTEGER;
    }

    public (int code, List<ModuleM1> ast, string message, int moduleIndex) Parse(string code)
    {
        modules = new List<ModuleM1>();
        var moduleIndex = -1;

        var tokens = code.Split((char[])null, System.StringSplitOptions.RemoveEmptyEntries);
        var depth = ParserDepth.OUT;
        resultCode = 0;
        errorMessage = "";

        var rules = new Dictionary<string, Rule>() {
                { "Structuralist", new Rule() {
                    ValidDepth = new List<ParserDepth>() { ParserDepth.OUT },
                    NextDepth = depth => ParserDepth.ROOT,
                    PerformAction = () => {}
                } },
                { "EndStructuralist", new Rule() {
                    ValidDepth = new List<ParserDepth>() { ParserDepth.ROOT },
                    NextDepth = depth => ParserDepth.OUT,
                    PerformAction = () => {}
                } },
                { "ModuleM1", new Rule() {
                    ValidDepth = new List<ParserDepth>() { ParserDepth.ROOT },
                    NextDepth = depth => ParserDepth.MODULE,
                    PerformAction = () => {
                        AddNewModuleM1();
                    }
                } },
                { "EndModuleM1", new Rule() {
                    ValidDepth = new List<ParserDepth>() {
                        ParserDepth.MODULE,
                        ParserDepth.CONSTRAINT_FEATURE_CONSEQUENCE,
                        ParserDepth.CONSTRAINT_MODULE_CONSEQUENCE_POINTER },
                    NextDepth = depth => ParserDepth.ROOT,
                    PerformAction = () => {
                        if (modules.Last().Name == null) {
                            error("Module must have a name");
                        }
                        else if (depth == ParserDepth.CONSTRAINT_FEATURE_CONSEQUENCE)
                        {
                            CheckFeatureConstraintConsequence();
                        }
                        else if (depth == ParserDepth.CONSTRAINT_MODULE_CONSEQUENCE_POINTER)
                        {
                            CheckModuleConsequencePointer();
                        }
                    }
                } },
                { "ItemsGroup", new Rule() {
                    ValidDepth = new List<ParserDepth>() { ParserDepth.MODULE },
                    NextDepth = depth => ParserDepth.ITEMSGROUP,
                    PerformAction = () => {
                        AddItemsGroup();
                    }
                } },
                { "EndItemsGroup", new Rule() {
                    ValidDepth = new List<ParserDepth>() { ParserDepth.ITEMSGROUP },
                    NextDepth = depth => ParserDepth.MODULE,
                    PerformAction = () => {
                        HandleItemsGroup();
                    }
                } },
                { "ForCase", new Rule() {
                    ValidDepth = new List<ParserDepth>() {
                        ParserDepth.MODULE,
                        ParserDepth.CONSTRAINT_CONDITION,
                        ParserDepth.CONSTRAINT_FEATURE_CONSEQUENCE,
                        ParserDepth.CONSTRAINT_MODULE_CONSEQUENCE_POINTER },
                    NextDepth = depth => ParserDepth.CONSTRAINT_CONDITION,
                    PerformAction = () => {
                        if (depth == ParserDepth.MODULE)
                        {
                            AddConstraint();
                        }
                        else if (depth == ParserDepth.CONSTRAINT_CONDITION)
                        {
                            if (CheckConstraintCondition())
                            {
                                AddCondition();
                            }
                        }
                        else if (depth == ParserDepth.CONSTRAINT_FEATURE_CONSEQUENCE)
                        {
                            if (CheckFeatureConstraintConsequence())
                            {
                                AddConstraint();
                            }
                        }
                        else if (depth == ParserDepth.CONSTRAINT_MODULE_CONSEQUENCE_POINTER)
                        {
                            if (CheckModuleConsequencePointer())
                            {
                                AddConstraint();
                            }
                        }
                    }
                } },
                { "LinkTo", new Rule() {
                    ValidDepth = new List<ParserDepth>() {
                        ParserDepth.CONSTRAINT_CONDITION,
                        ParserDepth.CONSTRAINT_MODULE_CONSEQUENCE_INSTRUCTION },
                    NextDepth = depth => {
                        if (depth == ParserDepth.CONSTRAINT_CONDITION) {
                            return ParserDepth.CONSTRAINT_FEATURE_CONSEQUENCE;
                        }
                        return ParserDepth.CONSTRAINT_MODULE_CONSEQUENCE_POINTER;
                    },
                    PerformAction = () => {
                        if (depth == ParserDepth.CONSTRAINT_CONDITION)
                        {
                            CheckConstraintCondition();
                            AddFeatureConsequence();
                        }
                        else if (depth == ParserDepth.CONSTRAINT_MODULE_CONSEQUENCE_INSTRUCTION)
                        {
                            CheckModuleConsequenceInstruction();
                        }
                    }
                } },
                { "Repeat", new Rule() {
                    ValidDepth = new List<ParserDepth>() {
                        ParserDepth.CONSTRAINT_CONDITION },
                    NextDepth = depth => ParserDepth.CONSTRAINT_MODULE_CONSEQUENCE_INSTRUCTION,
                    PerformAction = () => {
                        if(CheckConstraintCondition())
                        {
                            AddModuleConsequence();
                        }
                    }
                } },
                { "Generate",  new Rule() {
                        ValidDepth = new List<ParserDepth>() {
                            ParserDepth.OUT,
                        },
                        NextDepth = depth => ParserDepth.GENERATE_SPECIFYING,
                        PerformAction = () => {
                        }
                    }
                }
            };

        if (tokens.Length == 0)
        {
            error("Description is empty.");
        }

        if (resultCode == 0)
        {
            foreach (var token in tokens)
            {
                if (rules.ContainsKey(token))
                {
                    if (rules[token].ValidDepth.FindIndex(d => d == depth) == -1)
                    {
                        error($"Invalid {token} token depth.");
                    }
                    else
                    {
                        rules[token].PerformAction();
                        depth = rules[token].NextDepth(depth);
                    }
                }
                else
                {
                    if (depth == ParserDepth.MODULE)
                    {
                        if (modules.Last().Name == null)
                        {
                            modules.Last().Name = token;
                            CheckModuleNameDuplicates();
                        }
                        else
                        {
                            error($"Bad literal: '{token}'");
                        }
                    }
                    else if (depth == ParserDepth.ITEMSGROUP)
                    {
                        if (modules.Last().ClassificationFeatures.Last().Name == null)
                        {
                            modules.Last().ClassificationFeatures.Last().Name = token;
                            CheckItemsGroupName();
                        }
                        else
                        {
                            modules.Last().ClassificationFeatures.Last().Items.Add(token);
                            CheckItemsGroupOptions();
                        }
                    }
                    else if (depth == ParserDepth.CONSTRAINT_CONDITION)
                    {
                        if (modules.Last().Constraints.Last().Conditions.Last().ClassificationFeatureName == null)
                        {
                            modules.Last().Constraints.Last().Conditions.Last().ClassificationFeatureName = token;
                            CheckConstraintConditionName();
                        }
                        else
                        {
                            modules.Last().Constraints.Last().Conditions.Last().ConditionOptions.Add(token);
                            CheckConstraintConditionOptions();
                        }
                    }
                    else if (depth == ParserDepth.CONSTRAINT_FEATURE_CONSEQUENCE)
                    {
                        var consequence = modules.Last().Constraints.Last().Consequence as ConstraintFeatureConsequence;
                        if (consequence.FeatureName == null)
                        {
                            consequence.FeatureName = token;
                            CheckConstraintConsequenceName();
                        }
                        else
                        {
                            consequence.ValidOptions.Add(token);
                            CheckConstraintConsequeceOptions();
                        }
                    }
                    else if (depth == ParserDepth.GENERATE_SPECIFYING)
                    {
                        if (CheckModuleNameExistence(token))
                        {
                            moduleIndex = modules.FindIndex(module => module.Name == token);
                        }
                    }
                    else if (depth == ParserDepth.CONSTRAINT_MODULE_CONSEQUENCE_INSTRUCTION)
                    {
                        var consequence = modules.Last().Constraints.Last().Consequence as ConstraintModuleConsequence;

                        consequence.ExpressionString.Append(token);
                    }
                    else if (depth == ParserDepth.CONSTRAINT_MODULE_CONSEQUENCE_POINTER)
                    {
                        var consequence = modules.Last().Constraints.Last().Consequence as ConstraintModuleConsequence;

                        if (consequence.ModuleName == null && CheckModuleNameExistence(token))
                        {
                            consequence.ModuleName = token;
                        }
                        else if (consequence.ModuleAlias == null)
                        {
                            consequence.ModuleAlias = token;
                            CheckModuleConsequencePointer();
                        }
                        else
                        {
                            error($"Aggregate can have only one alias: module {modules.Last().Name}");
                        }
                    }
                    else
                    {
                        error("Unknown token");
                    }
                }
                if (resultCode != 0)
                {
                    break;
                }
            }
        }

        if (resultCode == 0 && depth != ParserDepth.OUT && depth != ParserDepth.GENERATE_SPECIFYING)
        {
            error("Description is not ended.");
        }

        if (resultCode == 0 && moduleIndex == -1)
        {
            error("Module to generate is not specified.");
        }

        return (
            code: resultCode,
            ast: modules,
            errorMessage,
            moduleIndex
        );
    }
}