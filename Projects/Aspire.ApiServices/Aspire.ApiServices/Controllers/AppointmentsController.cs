using System;
using System.Collections.Generic;
using Aspire.ApiServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Aspire.ApiServices.Extensions;
using System.Threading.Tasks;

namespace Aspire.ApiServices.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AppointmentsController: GenericControllerBase<Appointment>
    {
        public AppointmentsController(IConfiguration config):base(config)
        {
        }

        [HttpGet]
        [Route("GetProviderSchedule")]
        public async Task<List<WeekSchedule>> GetProviderSchedule(string providerId, int weekOfYear)
        {
            var lst = new List<WeekSchedule>();
            Blocks.AllPossibleBlocks.ForEach(x => lst.Add(x.ToWeekSchedule()));
            lst.ForEach(x => x.WeekOfYear = weekOfYear);
            var appointments = await this.GetBy(x => x.ProviderId== providerId && x.Block.WeekOfYear == weekOfYear);
            foreach(var appt in appointments)
            {
                var obj = lst.FirstOrDefault(x => x.DayOfWeek == appt.Block.DayOfWeek
                    && x.HourBlock == appt.Block.HourBlock
                    && x.HourSegment == appt.Block.HourSegment);
                if (obj != null)
                    obj.IsBooked = true;
            }

            return lst;
        }
    }
}
