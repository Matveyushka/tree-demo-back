using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Structuralist.M1;

namespace tree_demo_back.Controllers
{
    public class InputCode
    {
        public string code { get; set; } = null!;
    }

    [ApiController]
    [Route("compiletree")]
    public class MorphologicalSetCompilerController : ControllerBase
    {
        private readonly ILogger<MorphologicalSetCompilerController> _logger;

        public MorphologicalSetCompilerController(ILogger<MorphologicalSetCompilerController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello!");
        }

        [HttpPost]
        public IActionResult Post(InputCode inputCode)
        {
            try
            {
                var m1lexic = new M1LexicalParser();

                var tokens = m1lexic.Parser.GetTokens(inputCode.code);

                var model = new SyntaxParser().GetModel(tokens);

                var builder = new Builder();

                var buildedModule = model.Modules.First(m => m.Name == model.Create.ModuleName);

                var tree = builder.Build(model.Modules, model.Modules.IndexOf(buildedModule));

                return Ok(tree);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}