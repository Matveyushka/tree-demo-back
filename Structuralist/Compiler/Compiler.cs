namespace tree_demo_back
{
    public class Compiler
    {
        public (int resultCode, TreeNode[] tree, string errorMessage) Compile(string code)
        {
            var parser = new Parser();
            var builder = new Builder();

            var parseResult = parser.Parse(code);

            if (parseResult.code != 0)
            {
                return (
                    parseResult.code,
                    null,
                    parseResult.message
                );
            }
            else
            {
                return(
                    0,
                    builder.Build(parseResult.ast, parseResult.moduleIndex),
                    "Compilated successful"
                );
            }
        }
    }
}

