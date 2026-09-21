using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Web.Api
{
    public class TicketController : ApiController
    {

        [HttpPost]
        //[APIAuthorization]
        public IHttpActionResult Post([FromBody] RequestModel model)
        {
            int code = 0;
            string message = "";

            try
            {
                //message += " | Reading File";
                //message += " | Get CUstomer";

                //string dirname = "UploadedFiles/";

                code =1;
                message = "IN00012" + model.LanId;

            }
            catch (Exception ex)
            {
                // If an error occurs during processing, return an appropriate error response.
                code = 0;
                message = ex.Message.ToString();
            }

            return Ok(new
            {
                Code = code,
                Message = message
            });

        }

        public class RequestModel
        {
            public string LanId { get; set; }
            //public int CustomerId { get; set; }
            //public int RequestId { get; set; }
            //public int BranchId { get; set; }
            //public byte[] PdfFile { get; set; }
            public string Description { get; set; }
            //public String AccessToken { get; set; }

        }

    }
}
