using Mavidian.DataConveyer.Common;
using Mavidian.DataConveyer.Entities.KeyVal;
using Mavidian.DataConveyer.Logging;
using Mavidian.DataConveyer.Orchestrators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataConveyer_Dockerized270Parser.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class Parse270Controller : ControllerBase
   {
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
         using var reader = new StreamReader(Request.Body, Encoding.UTF8);
         (ProcessResult result, string output) = await ProcessX12Async(reader);
         if (result.CompletionStatus != CompletionStatus.IntakeDepleted) return BadRequest();
         return Ok(output);
      }

      private async Task<(ProcessResult result, string output)> ProcessX12Async(TextReader reader)
      {
         var retVal = new StringBuilder();

         var config = new OrchestratorConfig()
         {
            InputDataKind = KindOfTextData.X12,
            AsyncIntake = true,
            IntakeReader = () => reader,
            ClusterMarker = SegmentStartsCluster,
            MarkerStartsCluster = true,  //predicate (marker) matches the first record in cluster
            TransformerType = TransformerType.Universal,
            AllowTransformToAlterFields = true,
            UniversalTransformer = ExtractNeededElements,
            OutputDataKind = KindOfTextData.JSON,
            XmlJsonOutputSettings = "RecordNode|,IndentChars| ",
            OutputWriter = () => new StringWriter(retVal)
         };

         ProcessResult result;
         using (var orchtr = OrchestratorCreator.GetEtlOrchestrator(config))
         {
            result = await orchtr.ExecuteAsync();
         }

         return (result, retVal.ToString());
      }

      /// <summary>
      /// Cluster marker function to bundle all segments of each X12 transaction into a cluster
      /// </summary>
      /// <param name="rec"></param>
      /// <param name="prevRec"></param>
      /// <param name="recNo"></param>
      /// <returns></returns>
      private bool SegmentStartsCluster(IRecord rec, IRecord prevRec, int recNo)
      {
         return new string[] { "ISA", "GS", "ST", "GE", "IEA" }.Contains(rec["Segment"]);
      }  //each transaction is own cluster (plus separate clusters containing envelope segments)

      /// <summary>
      /// Extract fields from ST segments; ignore all other segments.
      /// </summary>
      /// <param name="cluster"></param>
      /// <returns></returns>
      private IEnumerable<ICluster> ExtractNeededElements(ICluster cluster)
      {
         var seg = (string)cluster[0]["Segment"];
         if (seg != "ST") return Enumerable.Empty<ICluster>(); //ignore envelope segments

         var outClstr = cluster.GetEmptyClone();
         var outRec = outClstr.ObtainEmptyRecord();

         if ((string)cluster[0][1] != "270")
         {
            outRec.AddItem("Error", "Submitted transaction is not an X12 270 Eligibility Inquiry");
         }
         else
         {
            outRec.AddItem("ReferenceID", cluster.Records.FirstOrDefault(r => (string)r["Segment"] == "TRN")?[2]);
            var subsriberRec = cluster.Records.FirstOrDefault(r => (string)r["Segment"] == "NM1" && (string)r[1] == "IL");
            outRec.AddItem("MemberID", subsriberRec[9]);
            var dependentRec = cluster.Records.FirstOrDefault(r => (string)r["Segment"] == "NM1" && (string)r[1] == "03");
            if (dependentRec == null)
            {
               outRec.AddItem("LastName", subsriberRec[3]);
               outRec.AddItem("FirstName", subsriberRec[4]);
            }
            else
            {
               outRec.AddItem("LastName", dependentRec[3]);
               outRec.AddItem("FirstName", dependentRec[4]);
            }
            outRec.AddItem("DOB", cluster.Records.FirstOrDefault(r => (string)r["Segment"] == "DMG")?[2]);
            outRec.AddItem("ServiceType", cluster.Records.FirstOrDefault(r => (string)r["Segment"] == "EQ")?[1]);
         }
         outClstr.AddRecord(outRec);
         return Enumerable.Repeat(outClstr, 1);
      }

   }
}