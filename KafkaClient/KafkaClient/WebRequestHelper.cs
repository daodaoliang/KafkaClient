using System.IO;
using System.Net;
using System.Text;

namespace KafkaClient
{
    public class WebRequestHelper
    {
        public static string Get(string host, string address, string accept = null)
        {
            var httpWebRequest = (HttpWebRequest) WebRequest.Create($"http://{host}/{address}");

            if (accept != null)
                httpWebRequest.Accept = accept;

            try
            {
                var stream = httpWebRequest.GetResponse().GetResponseStream();
                var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
            catch (WebException e)
            {
                using (var response = e.Response)
                {
                    using (var data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        var text = reader.ReadToEnd();
                        return text;
                    }
                }
            }
        }

        public static string Post(string host, string address, string dataString, string contentType = null,
            string accept = null)
        {
            var httpWebRequest = (HttpWebRequest) WebRequest.Create($"http://{host}/{address}");
            httpWebRequest.Method = "POST";

            if (contentType != null)
                httpWebRequest.ContentType = contentType;
            if (accept != null)
                httpWebRequest.Accept = accept;

            using (var stream = httpWebRequest.GetRequestStream())
            {
                var data = Encoding.UTF8.GetBytes(dataString);
                stream.Write(data, 0, data.Length);
            }

            try
            {
                var response = httpWebRequest.GetResponse();
                var responseStream = response.GetResponseStream();
                var responseString = new StreamReader(responseStream).ReadToEnd();
                return responseString;
            }
            catch (WebException e)
            {
                using (var response = e.Response)
                {
                    using (var data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        var text = reader.ReadToEnd();
                        return text;
                    }
                }
            }
        }

        public static void Delete(string baseUri)
        {
            var httpWebRequest = (HttpWebRequest) WebRequest.Create(baseUri);
            httpWebRequest.Method = "DELETE";
            try
            {
                httpWebRequest.GetResponse();
            }
            catch (WebException e)
            {
            }
        }
    }
}
