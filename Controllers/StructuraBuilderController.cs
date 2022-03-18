using Microsoft.AspNetCore.Mvc;
using Structuralist.M2;

namespace tree_demo_back.Controllers
{
    public class StructuraBuilderInput
    {
        public string Code { get; set; } = null!;
        public Structuralist.M1M2.ModuleIdentifier Identifier { get; set; } = null!;
    }

    [ApiController]
    [Route("buildstructura")]
    public class StructuraBuilderController : ControllerBase
    {
        public StructuraBuilderController()
        {

        }

        [HttpPost]
        public IActionResult Post(StructuraBuilderInput input)
        {
            // try
            // {
                var output = M2Compiler
                    .Compile(input.Code)
                    .GenerateStructure(input.Identifier);

                return Ok(output);
            // }
            // catch (Exception e)
            // {
                // return BadRequest(e.Message);
            // }
        }
    }
}