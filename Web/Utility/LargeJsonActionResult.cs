using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Web.Utility
{
    public class LargeJsonActionResult : ActionResult
    {
        public object Data { get; set; }
        public string ContentType { get; set; }
        public Encoding ContentEncoding { get; set; }
        public JsonSerializerSettings Settings { get; set; }

        public LargeJsonActionResult(object data)
        {
            Data = data;
            ContentType = "application/json";
            ContentEncoding = Encoding.UTF8;
            Settings = new JsonSerializerSettings
            {
                MaxDepth = 100,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var response = context.HttpContext.Response;
            response.ContentType = ContentType;

            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }

            if (Data != null)
            {
                var serializedData = JsonConvert.SerializeObject(Data, Settings);
                response.Write(serializedData);
            }
        }
    }
}