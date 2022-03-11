using Structuralist.M2;

public class SyntaxAnalyserState
{
    public Module CurrentModule { get; set; } = null;
    public FeatureRule CurrentFeatureRule { get; set; } = null;
    public FeatureRuleCase CurrentFeatureRuleCase { get; set; } = null;
    public LinkRule CurrentLinkRule { get; set; } = null;

    public SyntaxAnalyserLevel Level
    {
        get
        {
            if (this.CurrentModule is null &&
                CurrentFeatureRule is null &&
                CurrentFeatureRuleCase is null &&
                CurrentLinkRule is null)
            {
                return SyntaxAnalyserLevel.ROOT;
            }
            else if (this.CurrentModule is Module &&
                CurrentFeatureRule is null &&
                CurrentFeatureRuleCase is null &&
                CurrentLinkRule is null)
            {
                return SyntaxAnalyserLevel.MODULE;
            }
            else if (this.CurrentModule is Module &&
                CurrentFeatureRule is FeatureRule &&
                CurrentFeatureRuleCase is null &&
                CurrentLinkRule is null)
            {
                return SyntaxAnalyserLevel.FEATURE;
            }
            else if (this.CurrentModule is Module &&
                CurrentFeatureRule is FeatureRule &&
                CurrentFeatureRuleCase is FeatureRuleCase &&
                CurrentLinkRule is null)
            {
                return SyntaxAnalyserLevel.CASE;
            }
            else if (this.CurrentModule is Module &&
                CurrentFeatureRule is FeatureRule &&
                CurrentFeatureRuleCase is FeatureRuleCase &&
                CurrentLinkRule is LinkRule)
            {
                return SyntaxAnalyserLevel.LINK;
            }
            else
            {
                return SyntaxAnalyserLevel.INVALID;
            }
        }
    }
}