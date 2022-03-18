using Microsoft.AspNetCore.Mvc;
using Structuralist.M1;
using Structuralist.M1M2;

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
            /*try
            {*/
                var tree = M1Compiler.Compile(inputCode.code);

                return Ok(tree);
            /*}
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }*/
        }
    }
}