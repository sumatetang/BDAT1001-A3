using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ftp.Models.Utilities
{

    /// <summary>
    /// The HTTP class is a simple utilitiy object that allows interaction with common HTTP Requests
    /// </summary>
    public class HTTP
    {
        public const int Timeout = 60000; //60 seconds, a long time if needed
        public const string ContentType = "application/json;";
        public const string AuthHeader = "Authorization";
        public const string AuthMethod = "Bearer";

        //Code goes here

        /// <summary>
        /// Builds a standard HttpWebRequest object
        /// </summary>
        /// <param name="url">The Url to base the Request on</param>
        /// <param name="accessToken">If you have an Access Token you can use it here</param>
        /// <returns>HttpWebRequest</returns>
        public static HttpWebRequest BuildRequest(string url, string accessToken)
        {
            HttpWebRequest request = WebRequest.CreateHttp(url);

            //Set method to GET to retrieve data by default
            request.Method = HttpMethod.Get.Method;
            request.KeepAlive = false;
            request.ContentType = HTTP.ContentType;
            request.Timeout = HTTP.Timeout;
            //Use the application name to send as the UserAgent
            request.UserAgent = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

            //If we have an Access Token, add it to the Request
            if (!String.IsNullOrEmpty(accessToken))
            {
                request.Headers.Add($"{HTTP.AuthHeader}", $"{HTTP.AuthMethod} {accessToken}");
            }

            return (request);
        }
        /// <summary>
        /// Streams data to a HttpWebRequest
        /// </summary>
        /// <param name="request">HttpWebRequest</param>
        /// <param name="data">Any data</param>
        public async static Task StreamRequestData(HttpWebRequest request, object data)
        {
            //If we have data then Post it too
            if (data != null)
            {
                //Convert the data to Json
                string jsondata = JsonConvert.SerializeObject(data);
                //Convert the Json string to a byte[] array
                byte[] bytes = Encoding.UTF8.GetBytes(jsondata);
                //Let the request know how much data we are sending
                request.ContentLength = bytes.Length;
                //Write the data to the stream
                using (var stream = await request.GetRequestStreamAsync())
                {
                    await stream.WriteAsync(bytes, 0, bytes.Length);
                }
            }
        }

        /// <summary>
        /// Retrieves the result of a Web Request
        /// </summary>
        /// <param name="request">HttpWebRequest</param>
        /// <returns>The result of a web request</returns>
        public async static Task<string> PrintResponse(HttpWebRequest request)
        {
            string responseContent = null;

            // Get the Response
            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            {
                // Retrieve a handle to the Stream
                using (Stream stream = response.GetResponseStream())
                {
                    // Begin reading the Stream
                    using (StreamReader streamreader = new StreamReader(stream))
                    {
                        // Read the Response Stream to the end
                        responseContent = await streamreader.ReadToEndAsync();
                        streamreader.Close();
                    }

                    stream.Close();
                }

                response.Close();
            }

            return (responseContent);
        }
    }
}
