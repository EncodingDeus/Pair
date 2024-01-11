using System;
using System.IO;
using System.Net;
using System.Text;
using Cysharp.Threading.Tasks;
using Dobrozaur.Core;
using Dobrozaur.Network.Packet;
using Newtonsoft.Json;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Dobrozaur.Manager
{
    public class NetworkManager
    {
        private Uri _uri = new Uri("http://localhost:8080/api/v1");
        
        public NetworkManager()
        {
            // Init().Forget();
        }

        public async UniTaskVoid Auth()
        {
            Debug.Log("auth");
            var res = await Send<GameEntry.UserRegDto>("auth",
                new GameEntry.UserRegDto()
                {
                    username = "Misha",
                    clientId = Guid.NewGuid().ToString(),
                    platformId = 1,
                },
                HTTPMethod.PUT);
            
            Debug.Log(res);
        }

        public async UniTask<OutgoingPacket<object>> Send<T>(string path, T data, HTTPMethod method = HTTPMethod.GET)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"{_uri}/{path}");
            request.Method = method.ToString();
            request.ContentType = "application/json";
            
            var json = JsonConvert.SerializeObject(data);
            Debug.Log($"Send: {json}");
            
            if (data != null)
            {
                byte[] sentData = Encoding.UTF8.GetBytes(json);
                request.ContentLength = sentData.Length;
                
                using (Stream sendStream = await request.GetRequestStreamAsync())
                {
                    await sendStream.WriteAsync(sentData, 0, sentData.Length);
                    sendStream.Close();
                }
            }            
            
            string result = String.Empty;

            HttpWebResponse res = (HttpWebResponse)(await request.GetResponseAsync());

            if ((int)res.StatusCode < 500)
            {
                Stream responseStream = res.GetResponseStream();

                using (StreamReader sr = new
                           StreamReader(responseStream, Encoding.UTF8))
                {
                    char[] read = new char[256];
                    int count = sr.Read(read, 0, 256);

                    while (count > 0)
                    {
                        string str = new string(read, 0, count);
                        result += str;
                        count = await sr.ReadAsync(read, 0, 256);
                    }
                }

                Debug.Log("Response: " + result);
            }

            return new OutgoingPacket<object>((int)res.StatusCode >= 500, result);
        }

        public enum HTTPMethod
        {
            GET = 0,
            POST = 1,
            PUT = 2,
            DELETE = 3,
            PATCH = 4,
        }
    }
}