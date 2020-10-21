using System;
using System.Collections.Generic;
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

            var providerTypes = new List<ProviderDetail>();
            providerTypes.Add(new ProviderDetail()
            {
                 ProviderType = ProviderType.Cardiology,
                 Certifications = new string[] {
                     "State Board of Cardiology",
                     "Family Medicine",
                     "ABMS Cerfified"
                 },
                 IsTakingPatients=true,
                 PracticeDate = DateTime.Now.AddMonths(-60)
            });
            providerTypes.Add(new ProviderDetail()
            {
                ProviderType = ProviderType.Endocrinology,
                Certifications = new string[] {
                     "State Board of Endocrinology",
                     "Family Medicine",
                     "ABMS Cerfified"
                 },
                IsTakingPatients = true,
                PracticeDate = DateTime.Now.AddMonths(-70)
            });
            providerTypes.Add(new ProviderDetail()
            {
                ProviderType = ProviderType.FamilyMedicine,
                Certifications = new string[] {
                     "Family Medicine",
                     "ABPS Cerfified"
                 },
                IsTakingPatients = true,
                PracticeDate = DateTime.Now.AddMonths(-77)
            });
            providerTypes.Add(new ProviderDetail()
            {
                ProviderType = ProviderType.Internist,
                Certifications = new string[] {
                     "Family Medicine",
                     "ABMS Cerfified",
                     "ABPS Cerfified"
                 },
                IsTakingPatients = true,
                PracticeDate = DateTime.Now.AddMonths(-30)
            });
            providerTypes.Add(new ProviderDetail()
            {
                ProviderType = ProviderType.Psychology,
                Certifications = new string[] {
                     "State Board of Psychology",
                     "ABMP Cerfified",
                     "ABPS Cerfified"
                 },
                IsTakingPatients = true,
                PracticeDate = DateTime.Now.AddMonths(-80)
            });
            providerTypes.Add(new ProviderDetail()
            {
                ProviderType = ProviderType.Pediatrics,
                Certifications = new string[] {
                     "State Board of Pediatrics",
                     "AAPS Cerfified",
                     "ABPS Cerfified"
                 },
                IsTakingPatients = true,
                PracticeDate = DateTime.Now.AddMonths(-80)
            });
            providerTypes.Add(new ProviderDetail()
            {
                ProviderType = ProviderType.Neurology,
                Certifications = new string[] {
                     "State Board of Neurology",
                     "ABN Cerfified",
                     "ABPS Cerfified"
                 },
                IsTakingPatients = true,
                PracticeDate = DateTime.Now.AddMonths(-100)
            });
            providerTypes.Add(new ProviderDetail()
            {
                ProviderType = ProviderType.Oncology,
                Certifications = new string[] {
                     "State Board of Oncology",
                     "ABN Cerfified",
                     "ABPS Cerfified"
                 },
                IsTakingPatients = true,
                PracticeDate = DateTime.Now.AddMonths(-150)
            });



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
                    var result = await http.GetAsync("https://randomuser.me/api/?nat=us&results=400");
                    var json = await result.Content.ReadAsStringAsync();
                    var root = JsonConvert.DeserializeObject<Root>(json);

                    foreach (var record in root.results)
                    {
                        var idx = random.Next(0, (cityNames.Length - 1));
                        var providerIdx = random.Next(0, providerTypes.Count - 1);

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
                                Zip = 85204 //record.location.postcode,
                            },
                            UpdatedAt = DateTimeOffset.Now,
                            Active = true,
                            ProviderDetail = providerTypes[providerIdx]
                        });
                    }

                }

            }
        }
    }
}
