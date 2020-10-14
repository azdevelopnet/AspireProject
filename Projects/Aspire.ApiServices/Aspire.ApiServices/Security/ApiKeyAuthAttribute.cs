using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System.Linq;

namespace Aspire.ApiServices.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
    {
        private const string ApiKeyHeaderName = "ApiKey";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var potentialApiKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var apiKey = configuration.GetValue<string>("ApiKey");

            if (!apiKey.Equals(potentialApiKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            await next();
        }
    }

    public class ApiKeyAuthParameter : IOperationProcessor
    {
        public bool Process(OperationProcessorContext context)
        {
            if (context.ControllerType.GetCustomAttributes(typeof(ApiKeyAuthAttribute), true).FirstOrDefault() != null ||
                context.MethodInfo.GetCustomAttributes(typeof(ApiKeyAuthAttribute), true).FirstOrDefault() != null)
            {
                context.OperationDescription.Operation.Parameters.Add(
                    new OpenApiParameter
                    {
                        Name = "ApiKey",
                        Kind = OpenApiParameterKind.Header,
                        Type = NJsonSchema.JsonObjectType.String,
                        IsRequired = false,
                        Description = "Type your api key",
                    });
            }

            return true;
        }

    }

}
