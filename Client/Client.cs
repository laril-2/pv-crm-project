using System;
using System.IO;
using System.Net;
using System.Text;
using System.Security.Cryptography;
using System.Runtime.Serialization.Json;

namespace LianaMailerClient
{
    class Client
    {

        private const string URL = "https://rest.lianamailer.com/api/";

        private string realm;
        private string apiUser;
        private string apiKey;

        public Client(string realm, string apiUser, string apiKey)
        {
            this.realm = realm;
            this.apiUser = apiUser;
            this.apiKey = apiKey;
        }

        public EchoResponse EchoMessage(string echoMessage)
        {
            return this.Call<EchoResponse>("echoMessage", "[\"" + echoMessage + "\"]");
        }

        public T Call<T>(string method, string args, int version = 1)
        {
            Stream stream = null;
            WebResponse response = null;
            T jsonResponse = default(T);

            byte[] data = Encoding.UTF8.GetBytes(args);

            string md5;
            using (System.Security.Cryptography.MD5 md5Engine = System.Security.Cryptography.MD5.Create())
            {
                md5 = BitConverter.ToString(md5Engine.ComputeHash(data)).Replace("-", String.Empty).ToLower();
            }

            string now = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss+02:00");

            string message = string.Format(
                "POST\n{0}\napplication/json\n{1}\n{2}\n/api/v{3}/{4}",
                md5,
                now,
                args,
                version,
                method
            );

            HMACSHA256 hmc = new HMACSHA256(Encoding.ASCII.GetBytes(this.apiKey));
            byte[] hmres = hmc.ComputeHash(Encoding.ASCII.GetBytes(message));
            string signature = BitConverter.ToString(hmres).Replace("-", String.Empty).ToLower();

            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(string.Format(URL + "v{0}/{1}", version, method));

            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = data.Length;

            request.Headers.Set("X-Date", now);
            request.Headers.Set("Content-MD5", md5);
            request.Headers.Set("Authorization", string.Format("{0} {1}:{2}", this.realm, this.apiUser, signature));

            try
            {
                stream = request.GetRequestStream();
                stream.Write(data, 0, data.Length);
            }
            catch (Exception e)
            {
                // TODO proper error handling
                return default(T);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }

            try
            {
                response = request.GetResponse();
                stream = response.GetResponseStream();

                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(T));
                object objResponse = jsonSerializer.ReadObject(stream);
                jsonResponse = (T)objResponse;
            }
            catch (Exception e)
            {
                // TODO proper error handling
                return default(T);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
            }

            return jsonResponse;
        }

    }
}
