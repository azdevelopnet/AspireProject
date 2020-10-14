using System;
using Aspire.ApiServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Aspire.ApiServices.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController: GenericControllerBase<User>
    {
        public UsersController(IConfiguration config) : base(config)
        {
        }
    }
}
