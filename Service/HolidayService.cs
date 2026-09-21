using Data;
using Domain;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Service
{
    public class HolidayService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Controller));

        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public bool IsHoliday(DateTime date)
        {

            bool res = false;
            try
            {
                using (HttpClient client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true }))
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12;

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("AccessToken", "CSRM-2023-BC-5482");

                    int year = date.Year;
                    int month = date.Month;
                    int day = date.Day;

                    //string url = $"https://csrm.ndblk.int/api/holiday/IsHoliday?year={year}&month={month}&day={day}";
                    string url = $"https://10.96.1.63:9080/api/holiday/IsHoliday?year={year}&month={month}&day={day}";

                    HttpResponseMessage response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {

                        string responseBody = response.Content.ReadAsStringAsync().Result;
                        var result = JsonConvert.DeserializeObject<RequestResult>(responseBody);
                        // Access the values of Code and Message
                        if (result.Code == 1)
                            res = true;
                        //int code = result.Code;
                        //string message = result.Message;


                    }
                    else
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                Log.Error("Holiday API : Error " + date.ToString(), ex);
            }
            return res;
        }

      
        class RequestResult
        {
            public int Code { get; set; }
            public string Message { get; set; }
        }

        
    
    }
}
