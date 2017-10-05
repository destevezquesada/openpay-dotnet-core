using Newtonsoft.Json;
using Openpay.Entities;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Openpay
{
    public class OpenpayHttpClient
    {
        private static readonly string api_endpoint = "https://api.openpay.mx/v1/";
        private static readonly string api_endpoint_sandbox = "https://sandbox-api.openpay.mx/v1/";
        private static readonly string user_agent = "Openpay .NET v1";
        private static readonly Encoding encoding = Encoding.UTF8;
        private Boolean _isProduction = false;

        public int TimeoutSeconds { get; set; }

        public String MerchantId { get; internal set; }

        public String APIEndpoint { get; set; }

        public String APIKey { get; set; }

        public OpenpayHttpClient(string api_key, string merchant_id, bool production = false)
        {
            if (String.IsNullOrEmpty(api_endpoint_sandbox))
                throw new ArgumentNullException("api_key");
            if (String.IsNullOrEmpty(merchant_id))
                throw new ArgumentNullException("merchant_id");
            MerchantId = merchant_id;
            APIKey = api_key;
            TimeoutSeconds = 120;
            Production = production;
        }

        public bool Production { 
            get { 
                return _isProduction; 
            } 
            set { 
                APIEndpoint = value ? api_endpoint : api_endpoint_sandbox;
                _isProduction = value;
            } 
        }

        protected virtual HttpClient SetupRequest()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", user_agent);
            var byteArray = Encoding.ASCII.GetBytes(APIKey + ":");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            return client;
        }

        protected string GetResponseAsString(WebResponse response)
        {
            using (StreamReader sr = new StreamReader(response.GetResponseStream(), encoding))
            {
                return sr.ReadToEnd();
            }
        }

        public T Post<T>(string endpoint, JsonObject obj)
        {
            var json = DoRequest(endpoint, HttpMethod.POST, obj.ToJson());
            return JsonConvert.DeserializeObject<T>(json);
        }

		public void Post<T>(string endpoint)
		{
			DoRequest(endpoint, HttpMethod.POST, null);
		}

        public T Get<T>(string endpoint)
        {
            var json = DoRequest(endpoint, HttpMethod.GET, null);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public T Put<T>(string endpoint, JsonObject obj)
        {
            var json = DoRequest(endpoint, HttpMethod.PUT, obj.ToJson());
            return JsonConvert.DeserializeObject<T>(json);
        }

        public void Delete(string endpoint)
        {
            DoRequest(endpoint, HttpMethod.DELETE, null);
        }

        protected virtual string DoRequest(string path, HttpMethod method, string body)
        {
            var endpoint = APIEndpoint + MerchantId + path;
            var client = SetupRequest();
            try
            {
                var requestMessage = new HttpRequestMessage
                {
                    RequestUri = new Uri(endpoint),
                    Method = new System.Net.Http.HttpMethod(method.ToString()),
                };
                if (body != null)
                    requestMessage.Content = new StringContent(body,Encoding.UTF8,"application/json");
                var response = client.SendAsync(requestMessage).Result;

                if (response.IsSuccessStatusCode)
                    return response.Content.ReadAsStringAsync().Result;
                throw OpenpayException.GetFromJSON(HttpStatusCode.BadRequest, response.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public enum HttpMethod
        {
            GET, POST, DELETE, PUT,
        }
    }
}
