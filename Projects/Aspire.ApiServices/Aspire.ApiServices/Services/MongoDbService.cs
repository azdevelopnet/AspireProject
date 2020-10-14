using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Aspire.ApiServices.Extensions;
using Aspire.ApiServices.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Aspire.ApiServices.Services
{
    public class MondbInstance
    {
        private static IMongoDatabase _db;
        public static string ConnectionString { get; set; }
        public static IMongoDatabase Instance
        {
            get
            {
                if (_db == null)
                {
                    var client = new MongoClient("mongodb+srv://aspireadmin:aspire2020@aspirecluster.908tz.azure.mongodb.net/Aspire?retryWrites=true&w=majority");
                    _db = client.GetDatabase("Aspire");
                }
                return _db;
            }
        }
    }
    public class MongoDbService<T> where T: MongoDbbase
    {
      
        private IMongoCollection<T> collection;

        public MongoDbService(IConfiguration config)
        { 
            collection = MondbInstance.Instance.GetCollection<T>($"{typeof(T).Name}s");
        }

        public async Task<List<T>> GetAll()
        {
            return await collection.Find(_=>true).ToListAsync();
        }

        public async Task<T> GetById(string id)
        {
            return await collection.Find(obj => obj.Id == id).FirstOrDefaultAsync();
        }

        public async Task<T> Insert(T obj)
        {

            if (obj.Id == null)
            {
                obj.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
                await collection.InsertOneAsync(obj);
                return obj;
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> Update(T obj)
        {
            var result = await collection.ReplaceOneAsync(x => x.Id.Equals(obj.Id), obj, new ReplaceOptions { IsUpsert = true });
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> RemoveById(string id)
        {
            var result = await collection.DeleteOneAsync(x => x.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public PaginatedList<T> GetByQuery(APIQuery query)
        {
            var lst = collection.AsQueryable<T>()
                .CreateSearchQuery(query.Search)
                .CreateFilterQuery(query.Filter)
                .CreateOrderBy(query.Sort)
                .CreatePaginatedList<T>(query);

            return lst;
        }

        public async Task<List<T>> GetBy(Expression<Func<T, bool>> exp)
        {
            return await collection.Find(exp).ToListAsync();
        }
        public async Task<T> GetFirstOrDefault(Expression<Func<T, bool>> exp)
        {
            return await collection.Find(exp).FirstOrDefaultAsync();

        }
    }
}
