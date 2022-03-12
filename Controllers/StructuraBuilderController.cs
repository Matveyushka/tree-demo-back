using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Structuralist.M2.Output;

namespace tree_demo_back.Controllers
{
    public class StructuraBuilderInput
    {
        public string Code { get; set; } = null!;
        public Structuralist.M1M2.ModuleIdentifier Identifier { get; set; } = null!;
    }

    [ApiController]
    [Route("buildstructura")]
    public class StructuaBuilderController : ControllerBase
    {
        public StructuaBuilderController()
        {

        }

        [HttpPost]
        public IActionResult Post(StructuraBuilderInput input)
        {
            try
            {
                var m2Compiler = new M2Compiler();

                var model = m2Compiler.Compile(input.Code);

                var output = model!.GenerateStructure(input.Identifier);

                return Ok(output);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}