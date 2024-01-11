using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;

namespace Dobrozaur.Core
{
    public class GameEntry : MonoBehaviour
    {
        [Tooltip("Managers will be initialize by array order")]
        [SerializeField] private Core.Manager[] managers;

        private int _initManagerIndex;

        public class UserRegDto {

            public string username;
            public string clientId;
            public long platformId;
        }    
        
        public class CompletedLevelDto {
            public long userId;
            public short levelNumber;
            public int score;
            public short userAttempts;
        }
        
        public void Start123()
        {
            // var data = new UserRegDto()
            // {
            //     username = "Vasya",
            //     clientId = Guid.NewGuid().ToString(),
            //     platformId = 1,
            // };
            
            var data = new CompletedLevelDto()
            {
                score = 10,
                levelNumber = 1,
                userAttempts = 2,
                userId = 1,
            };

            
            Debug.Log("StartRequest");
            var request = WebRequest.Create($"http://localhost:8080/api/v1/completeLevel");
            request.Method = "PATCH";
            request.ContentType = "application/json";
            
            var json = JsonUtility.ToJson(data);
            Debug.Log($"Request: {json}");
            
            if (json !=null)
            {
                byte[] sentData = Encoding.UTF8.GetBytes(json);
                request.ContentLength = sentData.Length;
                
                using (Stream sendStream = request.GetRequestStream())
                {
                    sendStream.Write(sentData, 0, sentData.Length);
                    sendStream.Close();
                }
            
            }            
            
            string Out = String.Empty;

            System.Net.WebResponse res = request.GetResponse();
            System.IO.Stream ReceiveStream = res.GetResponseStream();
            using (System.IO.StreamReader sr = new 
                       System.IO.StreamReader(ReceiveStream, Encoding.UTF8))
            {

                Char[] read = new Char[256];
                int count = sr.Read(read, 0, 256);

                while (count > 0)
                {
                    String str = new String(read, 0, count);
                    Out += str;
                    count = sr.Read(read, 0, 256);
                }
            }
            
            
            Debug.Log("Response: " + Out);
            // Debug.Log (response.StatusDescription);

            
            // Init();
        }

        // private void Init()
        // {
        //     _initManagerIndex = 0;
        //     
        //     InitManager(managers[_initManagerIndex]);
        // }
        //
        // private void InitManager(IManager manager)
        // {
        //     Debug.Log($"Init {manager}");
        //     manager.Init(OnManagerInitComplete);
        // }
        //
        // private void OnManagerInitComplete(IManager manager)
        // {
        //     _initManagerIndex++;
        //     InitManager(managers[_initManagerIndex]);
        // }
    }
}