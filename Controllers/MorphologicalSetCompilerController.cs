using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Structuralist.M1;

namespace tree_demo_back.Controllers
{
    public class InputCode {
        public string code { get; set; }
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
            //return BadRequest("Bad req");
            var compiler = new Compiler();
            var result = compiler.Compile(inputCode.code);
            if (result.resultCode == 0) {
                return Ok(result.tree);
            } else {
                return BadRequest(result.errorMessage);
            }
        }
    }
}