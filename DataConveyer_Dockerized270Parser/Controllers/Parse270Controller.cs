using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DataConveyer_Dockerized270Parser.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class Parse270Controller : ControllerBase
   {
      public Parse270Controller(ILogger<Parse270Controller> logger)
      {
      }

      [HttpGet]
      [ProducesResponseType(StatusCodes.Status400BadRequest)]
      public IActionResult Get()
      {
         return BadRequest("Nothing to GET here. Use http POST method passing EDI 270 transaction in the request body.");
      }

      [HttpPost]
      [ProducesResponseType(StatusCodes.Status200OK)]
      [ProducesResponseType(StatusCodes.Status400BadRequest)]
      public async Task<ActionResult<string>> Post()
      {
         using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
         {
            var data = await reader.ReadToEndAsync();
            if (data == "xyz") return BadRequest();
            return Ok("Data Sent: " + data);
         }
      }
   }
}