namespace Structuralist.M1;

class SemanticParser
{
    public List<string> SemanticCheck(M1Model model)
    {
        var errors = new List<string>();

        var moduleNames = new List<string>();

        foreach (var module in model.Modules)
        {
            if (moduleNames.Contains(module.Name))
            {
                errors.Add($"Module {module.Name} is defined more than 1 time.");
            }
            moduleNames.Add(module.Name);

            var features = new Dictionary<string, List<string>>();

            foreach (var feature in module.Features)
            {
                if (features.ContainsKey(feature.Name))
                {
                    errors.Add($"Feature {feature.Name} is defined more than 1 time in module {module.Name}");
                }
                features.Add(feature.Name, new List<string>());

                foreach (var featureValue in feature.Values)
                {
                    if (features[feature.Name].Contains(featureValue))
                    {
                        errors.Add($"Feature value {featureValue} is defined more than 1 time in feature {feature.Name} in module {module.Name}");
                    }
                    features[feature.Name].Add(featureValue);
                }
            }

            foreach (var featureRule in module.FeatureRules)
            {
                foreach (var condition in featureRule.Conditions)
                {
                    if (features.ContainsKey(condition.Name) == false)
                    {
                        errors.Add($"Feature constraint condition contains unknown feature name {condition.Name}' in module {module.Name}");
                    }
                    else
                    {
                        foreach (var conditionValue in condition.Values)
                        {
                            if (features[condition.Name].Contains(conditionValue) == false)
                            {
                                errors.Add($"Invalid value '{conditionValue}' of feature '{condition.Name}' in feature constraint condition in module '{module.Name}''");
                            }
                        }
                    }
                }

                foreach (var consequence in featureRule.Consequences)
                {
                    if (features.ContainsKey(consequence.Name) == false)
                    {
                        errors.Add($"Feature constraint consequence contains unknown feature name {consequence.Name}' in module {module.Name}");
                    }
                    else
                    {
                        foreach (var consequenceValue in consequence.Values)
                        {
                            if (features[consequence.Name].Contains(consequenceValue) == false)
                            {
                                errors.Add($"Invalid value '{consequenceValue}' of feature '{consequence.Name}' in feature constraint consequence in module '{module.Name}''");
                            }
                        }
                    }
                }
            }

            foreach (var generateRule in module.GenerateRules)
            {
                foreach (var condition in generateRule.Conditions)
                {
                    if (features.ContainsKey(condition.Name) == false)
                    {
                        errors.Add($"Generate rule condition contains unknown feature name {condition.Name}' in module {module.Name}");
                    }
                    else
                    {
                        foreach (var conditionValue in condition.Values)
                        {
                            if (features[condition.Name].Contains(conditionValue) == false)
                            {
                                errors.Add($"Invalid value '{conditionValue}' of feature '{condition.Name}' in generate rule condition in module '{module.Name}''");
                            }
                        }
                    }
                }

                var usedVariables = generateRule
                    .Command
                    .SelectMany(command => command.QuantityExpression.GetVariables())
                    .ToList();
                
                foreach (var variable in usedVariables)
                {
                    if (features.ContainsKey(variable) == false)
                    {
                        errors.Add($"Unknown feature {variable} used in generate rule command in module {module.Name}");
                    }
                }
            }
        }

        if (moduleNames.Contains(model.CreateCommand.ModuleName) == false)
        {
            errors.Add($"Unknown module {model.CreateCommand.ModuleName} in creation command.");
        }

        return errors;
    }
}