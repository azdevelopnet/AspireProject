using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Aspire.ApiServices.Models;
using Aspire.ApiServices.Services;
using Humanizer;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Aspire.ApiServices
{
    public class StartupHelper
    {
        IConfiguration config;

        public StartupHelper(IConfiguration config)
        {
            this.config = config;
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

        public async Task SeedApplication()
        {
            var service = new MongoDbService<User>(config);

            var random = new Random();
            var providerTypes = (ProviderType[])Enum.GetValues(typeof(ProviderType));
            var cityNames = new string[] { "Tempe", "Peoria", "Scottsdale", "Buckeye", "Sun City", "Mesa", "Phoenix", "Chandler", "Gilbert" };

            var usr = await service.GetFirstOrDefault(x => x.Email == "admin@aspire.com");
            if (usr == null)
            {
                var insertResult = await service.Insert(new User()
                {
                    Active = true,
                    UserType = UserType.Staff,
                    FirstName = "admin",
                    LastName = "user",
                    Email = "admin@aspire.com",
                    Password = CreateMD5("admin"),
                    ProfileImageUrl = "https://media-exp1.licdn.com/dms/image/C4D03AQGPE5iDbM1xsA/profile-displayphoto-shrink_200_200/0?e=1607558400&v=beta&t=IOpGgSQsUHlC8sLLhIROAQdmymu8vGVlEtHqExtmNWs"
                });


                using (var http = new HttpClient())
                {
                    var result = await http.GetAsync("https://randomuser.me/api/?nat=us&results=700");
                    var json = await result.Content.ReadAsStringAsync();
                    var root = JsonConvert.DeserializeObject<Root>(json);
                    var arizonaResults = root.results.Where(x => x.location.state == "Arizona");
                    foreach (var record in arizonaResults)
                    {
                        var idx = random.Next(0, (cityNames.Length - 1));
                        var providerIdx = random.Next(0, providerTypes.Length - 1);

                        insertResult = await service.Insert(new User()
                        {
                            UserType = UserType.Provider,
                            FirstName = record.name.first.Humanize(LetterCasing.Title),
                            LastName = record.name.last.Humanize(LetterCasing.Title),
                            Email = $"{record.name.first}.{record.name.last}@aspirehealth.com",
                            Password = CreateMD5($"{record.name.last}12345"),
                            ProfileImageUrl = record.picture.medium,
                            Address = new Address()
                            {
                                StreetAddress = $"{record.location.street.number} {record.location.street.name}",
                                City = cityNames[idx],
                                State = "AZ",
                                Zip = record.location.postcode,
                            },
                            UpdatedAt = DateTimeOffset.Now,
                            Active = true,
                            ProviderType = providerTypes[providerIdx]
                        });
                    }

                }

            }
        }
    }
}
