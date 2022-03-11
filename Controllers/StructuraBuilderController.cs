using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Structuralist.M2;

namespace tree_demo_back.Controllers
{
    [ApiController]
    [Route("buildstructura")]
    public class StructuaBuilderController : ControllerBase
    {
        public StructuaBuilderController()
        {

        }

        private void LogTerminals(List<Terminal> terminals)
        {
            terminals.ForEach(term =>
            {
                if (term is Keyword keyword)
                {
                    Console.WriteLine(string.Format("{0,-30}{1,-30}{2, -5}{3, -5}",
                        "KEYWORD", keyword.Type, term.StringNumber, term.Position));
                }
                else if (term is Literal literal)
                {
                    Console.WriteLine(string.Format("{0,-15}{1,-15}{2,-30}{3, -5}{4, -5}",
                        "LITERAL", literal.Type, literal.Value, term.StringNumber, term.Position));
                }
                else if (term is Operator oper)
                {
                    Console.WriteLine(string.Format("{0,-30}{1,-30}{2, -5}{3, -5}",
                        "OPERATOR", oper.Type, term.StringNumber, term.Position));
                }
                else if (term is Identifier identifier)
                {
                    Console.WriteLine(string.Format("{0,-30}{1,-30}{2, -5}{3, -5}",
                        "IDENTIFIER", identifier.Value, term.StringNumber, term.Position));
                }
                else
                {
                    Console.WriteLine(string.Format("{0,-60}{1, -5}{2, -5}",
                        "UNKNOWN", term.StringNumber, term.Position));
                }

            });
        }

        [HttpPost]
        public IActionResult Post(InputCode inputCode)
        {
            try
            {
                var terminals = new LexicalAnalyser().GetTerminals(inputCode.code);
                LogTerminals(terminals);
                return Ok(inputCode.code);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}