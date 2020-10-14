using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Aspire.ApiServices.Models;
using Aspire.ApiServices.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Aspire.ApiServices.Controllers
{
    public class GenericControllerBase<T>: ControllerBase where T: MongoDbbase
    {
        protected readonly MongoDbService<T> service;

        public GenericControllerBase(IConfiguration config)
        {
            service = new MongoDbService<T>(config);
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<List<T>> GetAll()
        {
            return await service.GetAll();
        }

        [HttpGet]
        [Route("GetById")]
        public async Task<T> GetById(string id)
        {
            return await service.GetById(id);
        }

        [HttpPost]
        [Route("Insert")]
        public async Task<T> Insert(T obj)
        {
            return await service.Insert(obj);
        }

        [HttpPut]
        [Route("Update")]
        public async Task<bool> Update(T obj)
        {
            return await service.Update(obj);
        }

        [HttpDelete]
        [Route("RemoveById")]
        public async Task<bool> RemoveById(string id)
        {
            return await service.RemoveById(id);
        }


        [HttpPost]
        [Route("GetByQuery")]
        public PaginatedList<T> GetByQuery(APIQuery query)
        {
            return service.GetByQuery(query);
        }

        protected async Task<List<T>> GetBy(Expression<Func<T, bool>> exp)
        {
            return await service.GetBy(exp);
        }
        protected async Task<T> GetFirstOrDefault(Expression<Func<T, bool>> exp)
        {
            return await service.GetFirstOrDefault(exp);
        }
    }


}
