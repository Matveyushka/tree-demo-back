
using System.Collections.Generic;
using System.Linq;

namespace tree_demo_back
{
    public enum ParserDepth
    {
        OUT,
        ROOT,
        MODULE,
        ITEMSGROUP,
        CONSTRAINT_CONDITION,
        CONSTRAINT_CONSEQUENCE,
        GENERATE_SPECIFYING
    }

    public delegate void TokenAction();

    public class Rule
    {
        public List<ParserDepth> ValidDepth { get; set; }

        public ParserDepth NextDepth { get; set; }

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
                Сonditions = new List<ConstraintCondition>() {
                                    new ConstraintCondition() {
                                        СonditionOptions = new List<string>()
                                    }
                                },
                Сonsequence = new ConstraintConsequence()
                {
                    ValidOptions = new List<string>()
                }
            });
        }

        void AddCondition()
        {
            modules.Last().Constraints.Last().Сonditions.Add(new ConstraintCondition()
            {
                СonditionOptions = new List<string>()
            });
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
            var conditionName = modules.Last().Constraints.Last().Сonditions.Last().СlassificationFeatureName;

            var featureNames = modules.Last().ClassificationFeatures.Select(feature => feature.Name).ToList();

            if (featureNames.FindIndex(name => name == conditionName) == -1)
            {
                error($"Unknown classification feature in constraint condition. Module: '{modules.Last().Name}'.");
            }
        }

        void CheckConstraintConsequenceName()
        {
            var consequenceName = modules.Last().Constraints.Last().Сonsequence.СlassificationFeatureName;

            var featureNames = modules.Last().ClassificationFeatures.Select(feature => feature.Name).ToList();

            if (featureNames.FindIndex(name => name == consequenceName) == -1)
            {
                error($"Unknown classification feature in constraint consequence. Module: '{modules.Last().Name}'.");
            }
        }

        void CheckConstraintConditionOptions()
        {
            var options = modules.Last().Constraints.Last().Сonditions.Last().СonditionOptions;

            if (options.Count() != options.Distinct().Count())
            {
                error($"There are the same options in constarint condition: module '{modules.Last().Name}'.");
            }

            var conditionName = modules.Last().Constraints.Last().Сonditions.Last().СlassificationFeatureName;

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
            var options = modules.Last().Constraints.Last().Сonsequence.ValidOptions;

            if (options.Count() != options.Distinct().Count())
            {
                error($"There are the same options in constarint consequence: module '{modules.Last().Name}'.");
            }

            var consequenceName = modules.Last().Constraints.Last().Сonsequence.СlassificationFeatureName;

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

        void CheckItemsGroup()
        {
            if (modules.Last().ClassificationFeatures.Last().Name == null)
            {
                error($"Items group of the '{modules.Last().Name}' module must have a name.");
            }
            else if (modules.Last().ClassificationFeatures.Last().Items.Count == 0)
            {
                error($"Items group '{modules.Last().ClassificationFeatures.Last().Name}' of the '{modules.Last().Name}' module must not be empty.");
            }
        }

        bool CheckConstraintCondition()
        {
            bool result = true;
            if (modules.Last().Constraints.Last().Сonditions.Last().СlassificationFeatureName == null)
            {
                error($"No condition name. Module: {modules.Last().Name}.");
                result = false;
            }
            else if (modules.Last().Constraints.Last().Сonditions.Last().СonditionOptions.Count == 0)
            {
                error($"Empty condition. Module: {modules.Last().Name}. Condition: {modules.Last().Constraints.Last().Сonditions.Last().СlassificationFeatureName}");
                result = false;
            }
            return result;
        }

        bool CheckConstraintConsequence()
        {
            bool result = true;
            if (modules.Last().Constraints.Last().Сonsequence.СlassificationFeatureName == null)
            {
                error($"No consequence name. Module: {modules.Last().Name}.");
                result = false;
            }
            else if (modules.Last().Constraints.Last().Сonsequence.ValidOptions.Count == 0)
            {
                error($"Empty consequence. Module: {modules.Last().Name}. Consequence: {modules.Last().Constraints.Last().Сonsequence.СlassificationFeatureName}");
                result = false;
            }
            return result;
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
                    NextDepth = ParserDepth.ROOT,
                    PerformAction = () => {}
                } },
                { "EndStructuralist", new Rule() {
                    ValidDepth = new List<ParserDepth>() { ParserDepth.ROOT },
                    NextDepth = ParserDepth.OUT,
                    PerformAction = () => {}
                } },
                { "ModuleM1", new Rule() {
                    ValidDepth = new List<ParserDepth>() { ParserDepth.ROOT },
                    NextDepth = ParserDepth.MODULE,
                    PerformAction = () => {
                        AddNewModuleM1();
                    }
                } },
                { "EndModuleM1", new Rule() {
                    ValidDepth = new List<ParserDepth>() {
                        ParserDepth.MODULE,
                        ParserDepth.CONSTRAINT_CONSEQUENCE },
                    NextDepth = ParserDepth.ROOT,
                    PerformAction = () => {
                        if (modules.Last().Name == null) {
                            error("Module must have a name");
                        }
                        if (depth == ParserDepth.CONSTRAINT_CONSEQUENCE)
                        {
                            CheckConstraintConsequence();
                        }
                    }
                } },
                { "ItemsGroup", new Rule() {
                    ValidDepth = new List<ParserDepth>() { ParserDepth.MODULE },
                    NextDepth = ParserDepth.ITEMSGROUP,
                    PerformAction = () => {
                        AddItemsGroup();
                    }
                } },
                { "EndItemsGroup", new Rule() {
                    ValidDepth = new List<ParserDepth>() { ParserDepth.ITEMSGROUP },
                    NextDepth = ParserDepth.MODULE,
                    PerformAction = () => {
                        CheckItemsGroup();
                    }
                } },
                { "ForCase", new Rule() {
                    ValidDepth = new List<ParserDepth>() {
                        ParserDepth.MODULE,
                        ParserDepth.CONSTRAINT_CONDITION,
                        ParserDepth.CONSTRAINT_CONSEQUENCE },
                    NextDepth = ParserDepth.CONSTRAINT_CONDITION,
                    PerformAction = () => {
                        if (depth == ParserDepth.MODULE)
                        {
                            AddConstraint();
                        }
                        else if (depth == ParserDepth.CONSTRAINT_CONDITION)
                        {
                            if(CheckConstraintCondition())
                            {
                                AddCondition();
                            }
                        }
                        else if (depth == ParserDepth.CONSTRAINT_CONSEQUENCE)
                        {
                            if (CheckConstraintConsequence())
                            {
                                AddConstraint();
                            }
                        }
                    }
                } },
                { "LinkTo", new Rule() {
                    ValidDepth = new List<ParserDepth>() {
                        ParserDepth.CONSTRAINT_CONDITION },
                    NextDepth = ParserDepth.CONSTRAINT_CONSEQUENCE,
                    PerformAction = () => {
                        CheckConstraintCondition();
                    }
                } },
                {
                    "Generate",  new Rule() {
                        ValidDepth = new List<ParserDepth>() {
                            ParserDepth.OUT,
                        },
                        NextDepth = ParserDepth.GENERATE_SPECIFYING,
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
                            depth = rules[token].NextDepth;
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
                            if (modules.Last().Constraints.Last().Сonditions.Last().СlassificationFeatureName == null)
                            {
                                modules.Last().Constraints.Last().Сonditions.Last().СlassificationFeatureName = token;
                                CheckConstraintConditionName();
                            }
                            else
                            {
                                modules.Last().Constraints.Last().Сonditions.Last().СonditionOptions.Add(token);
                                CheckConstraintConditionOptions();
                            }
                        }
                        else if (depth == ParserDepth.CONSTRAINT_CONSEQUENCE)
                        {
                            if (modules.Last().Constraints.Last().Сonsequence.СlassificationFeatureName == null)
                            {
                                modules.Last().Constraints.Last().Сonsequence.СlassificationFeatureName = token;
                                CheckConstraintConsequenceName();
                            }
                            else
                            {
                                modules.Last().Constraints.Last().Сonsequence.ValidOptions.Add(token);
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
}
