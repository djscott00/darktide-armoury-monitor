using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;



namespace Darktide_Armoury_Monitor
{
    public static class Notifier
    {

        public static bool SendNotification(string apiToken, string project, 
            string channel, string eventSubject, string message, out string errorMsg)
        {
            errorMsg = "";
            try {

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://api.logsnag.com/v1/log");
                
                client.DefaultRequestHeaders.Accept.Clear();

                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer",apiToken);


                var body = @"{" + 
                    $@"""project"":""{project}""," + 
                    $@"""channel"":""{channel}""," + 
                    $@"""event"":""{eventSubject}""," + 
                    $@"""description"":""{message}""" + 
                    @",""icon"":""🚨"",""notify"":true}";                

                StringContent contentBody = new StringContent(body, Encoding.UTF8, "application/json");

                var task = client.PostAsync("", contentBody);
                var result = task.Result;

                return result.IsSuccessStatusCode;

            }
            catch (Exception err) {
                errorMsg = err.Message;
                return false;
            }

        }



    }


}
