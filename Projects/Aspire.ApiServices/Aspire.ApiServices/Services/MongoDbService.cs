using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Aspire.ApiServices.Extensions;
using Aspire.ApiServices.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace Aspire.ApiServices.Services
{
    public class MondbInstance
    {
        private static IMongoDatabase _db;
        public static string ConnectionString { get; set; }
        public static string DatabaseName { get; set; }
        public static IMongoDatabase Instance
        {
            get
            {
                if (_db == null)
                {
                   //var c = Encrypt(ConnectionString, "Aspire");
                    var client = new MongoClient(Decrypt(ConnectionString, "Aspire"));
                    _db = client.GetDatabase(DatabaseName);

                }
                return _db;
            }
        }

        private static string Encrypt(string source, string key)
        {
            using (TripleDESCryptoServiceProvider tripleDESCryptoService = new TripleDESCryptoServiceProvider())
            {
                using (MD5CryptoServiceProvider hashMD5Provider = new MD5CryptoServiceProvider())
                {
                    byte[] byteHash = hashMD5Provider.ComputeHash(Encoding.UTF8.GetBytes(key));
                    tripleDESCryptoService.Key = byteHash;
                    tripleDESCryptoService.Mode = CipherMode.ECB;
                    byte[] data = Encoding.UTF8.GetBytes(source);
                    return Convert.ToBase64String(tripleDESCryptoService.CreateEncryptor().TransformFinalBlock(data, 0, data.Length));
                }
            }
        }

        private static string Decrypt(string encrypt, string key)
        {
            using (TripleDESCryptoServiceProvider tripleDESCryptoService = new TripleDESCryptoServiceProvider())
            {
                using (MD5CryptoServiceProvider hashMD5Provider = new MD5CryptoServiceProvider())
                {
                    byte[] byteHash = hashMD5Provider.ComputeHash(Encoding.UTF8.GetBytes(key));
                    tripleDESCryptoService.Key = byteHash;
                    tripleDESCryptoService.Mode = CipherMode.ECB;
                    byte[] data = Convert.FromBase64String(encrypt);
                    return Encoding.UTF8.GetString(tripleDESCryptoService.CreateDecryptor().TransformFinalBlock(data, 0, data.Length));
                }
            }
        }
    }
    public class MongoDbService<T> where T: MongoDbbase
    {
      
        private IMongoCollection<T> collection;

        public MongoDbService(IConfiguration config)
        {
            if (string.IsNullOrEmpty(MondbInstance.ConnectionString))
            {
                MondbInstance.ConnectionString = config["MongoConnection:ConnectionString"];
                MondbInstance.DatabaseName = config["MongoConnection:Database"];
            }
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
