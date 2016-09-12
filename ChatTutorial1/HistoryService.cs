using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ChatDAL.CommonModels;
using Newtonsoft.Json;

namespace ChatTutorial1
{
    public class HistoryService
    {
        private string _dbApiUrl = "http://localhost:5749/api/";
        public IEnumerable<ChatLog> GetChatLogs()
        {
            var response = GetString(_dbApiUrl+ "chats").GetAwaiter().GetResult();
            
            var dserlz = new List<DBApiDTO>();
            try
            {
                dserlz = JsonConvert.DeserializeObject<List<DBApiDTO>>(response);
            }
            catch (Exception ex)
            {
                //failed to deserialize
            }

            var chatLogs = dserlz.SelectMany(e => e.chats);
            return chatLogs;
        }

        public void SaveChatLogs(List<ChatLog> logs)
        {
            var logString = JsonConvert.SerializeObject(logs);
            Save(logString).GetAwaiter().GetResult();
        }

        private async Task<string> GetString(string url)
        {
            using (var client = new HttpClient())
            {
                return await client.GetStringAsync(url).ConfigureAwait(false);
            }
        }

        private async Task<string> Save(string contentString)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    Content = new StringContent(contentString, Encoding.UTF8, "application/json"),
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(_dbApiUrl + "chats")
                };
                var response = await client.SendAsync(request).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }
            return "fail";
        }

        private async Task Delete(string id)
        {
            using (var client = new HttpClient())
            {
                
                var response = await client.DeleteAsync(_dbApiUrl + "chats"+id);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();

                }
            }
        }

        public class DBApiDTO
        {
            public string name { get; set; }
            public List<ChatLog> chats { get; set; } 
        }

    }
}