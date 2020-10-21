using System.Collections.Generic;
using System.Threading.Tasks;
using Aspire.ApiServices.Controllers;
using Aspire.ApiServices.Models;
using Microsoft.Extensions.Configuration;
using Xunit;
namespace Aspire.ApiServices.XTest
{
    public class AppointmentTest
    {
        AppointmentsController ctrl;

        public AppointmentTest()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            ctrl = new AppointmentsController(config);
        }


        [Fact]
        public async Task GetAppointments()
        {
           var results = await ctrl.GetAll();
            var x = results;

        }
    }
}
