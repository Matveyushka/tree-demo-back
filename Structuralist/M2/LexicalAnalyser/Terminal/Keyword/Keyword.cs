using System;

namespace Structuralist.M2;

public class Keyword : Terminal
{
    public KeywordType Type { get; set; }

    public const string moduleBegin = "Structura";
    public const string moduleEnd = "EndStructura";
    public const string featureBegin = "Feature";
    public const string featureEnd = "EndFeature";
    public const string caseBegin = "Case";
    public const string ruleBegin = "On";
    public const string negation = "not";
    public const string link = "Link";
    public Keyword(
        string terminal,
        int stringNumber,
        int position
        ) : base(stringNumber, position)
    {
        switch (terminal)
        {
            case moduleBegin: this.Type = KeywordType.MODULE_BEGIN; break;
            case moduleEnd: this.Type = KeywordType.MODULE_END; break;
            case featureBegin: this.Type = KeywordType.FEATURE_BEGIN; break;
            case featureEnd: this.Type = KeywordType.FEATURE_END; break;
            case caseBegin: this.Type = KeywordType.CASE; break;
            case ruleBegin: this.Type = KeywordType.RULE; break;
            case link: this.Type = KeywordType.LINK; break;
            case negation: this.Type = KeywordType.NOT; break; 
            default: throw new ArgumentException($"Terminal {terminal} is not keyword");
        }
    }

    public static bool IsKeyword(string terminal) =>
        terminal == link ||
        terminal == ruleBegin ||
        terminal == caseBegin ||
        terminal == featureEnd ||
        terminal == featureBegin ||
        terminal == moduleEnd ||
        terminal == moduleBegin ||
        terminal == negation;
}