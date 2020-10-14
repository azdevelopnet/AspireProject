using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;

namespace Aspire.ApiServices.XTest
{
    public class ApiKey
    {
        public string Name { get; set; }
        public string KeyValue { get; set; }
    }
    public class RawString
    {
        public string Content { get; set; }
    }

    public class UnitTestHelper
    {
        public string BearerToken { get; set; }

        public async Task<T> GetAsync<T>(string url) where T : class, new()
        {
            using (var http = new HttpClient())
            {
                if (!string.IsNullOrEmpty(BearerToken))
                    http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", BearerToken);
                var result = await http.GetAsync(url);
                Assert.Equal("OK", result.StatusCode.ToString());
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var json = await result.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(json);
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<T> PostAsync<T>(string url, object obj, ApiKey apiKey = null) where T : class, new()
        {
            using (var http = new HttpClient())
            {
                if (!string.IsNullOrEmpty(BearerToken))
                    http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", BearerToken);

                if (apiKey != null)
                    http.DefaultRequestHeaders.Add(apiKey.Name, apiKey.KeyValue);

                var json = JsonConvert.SerializeObject(obj);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var result = await http.PostAsync(url, data);

                Assert.Equal("OK", result.StatusCode.ToString());
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var jsonResult = await result.Content.ReadAsStringAsync();

                    //ReWrap result is RawString return type
                    if (typeof(T).Name == "RawString")
                    {
                        jsonResult = JsonConvert.SerializeObject(new RawString()
                        {
                            Content = jsonResult
                        });
                    }

                    return JsonConvert.DeserializeObject<T>(jsonResult);

                }
                else
                {
                    return null;
                }
            }
        }

        public string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
