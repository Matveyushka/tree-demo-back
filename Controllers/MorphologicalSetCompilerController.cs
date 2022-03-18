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
                var tokens = new M1LexicalParser().Parser.GetTokens(inputCode.code);

                var model = new SyntaxParser().GetModel(tokens);

                var tree = new Builder().Build(model);

                return Ok(tree);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}