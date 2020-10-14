using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Aspire.ApiServices.Models;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using MDL = MongoDB.Driver.Linq;

namespace Aspire.ApiServices.Extensions
{
    public static class AppExtensions
    {

        public static WeekSchedule ToWeekSchedule(this AppointmentBlock block)
        {
            return new WeekSchedule()
            {
                WeekOfYear = block.WeekOfYear,
                DayOfWeek = block.DayOfWeek,
                HourBlock = block.HourBlock,
                HourSegment = block.HourSegment
            };
        }

        public static PaginatedList<T> CreatePaginatedList<T>(this MDL.IMongoQueryable<T> query, APIQuery apiQuery) where T : class
        {
            int pageIndex = apiQuery.PageIndex.HasValue ? apiQuery.PageIndex.Value : 0;
            int pageSize = apiQuery.PageSize.HasValue ? apiQuery.PageSize.Value : 0;
            int count = 0;
            List<T> items = null;

            var q = query.AsQueryable();

            if (string.IsNullOrEmpty(apiQuery.Sort) && typeof(T).GetProperty("Id") != null)
            {
                q = q.OrderBy("Id");
            }

            if (typeof(T).GetProperty("IsActive") != null)
            {
                if (!apiQuery.IncludeAll)
                    q = q.Where($"IsActive = true");
            }

            if (apiQuery.PageSize.HasValue)
            {

                count = q.Count();
                items = q.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            }
            else
            {
                items = q.ToList();
                count = items.Count;
            }
            return new PaginatedList<T>(items, count, pageIndex, pageSize);

        }


        public static MDL.IMongoQueryable<T> CreateOrderBy<T>(this MDL.IMongoQueryable<T> query, string sort) where T : class
        {
            if (!string.IsNullOrEmpty(sort))
            {
                var col = sort.Split(" ");
                if (sort.IndexOf("Asc") != -1 || sort.IndexOf("asc") != -1)
                {
                    query = (MDL.IMongoQueryable<T>)query.OrderBy(col[0]);
                }
                else
                {
                    query = (MDL.IMongoQueryable<T>)query.OrderBy(col[0] + " descending");
                }
                return query;
            }
            else
            {
                return (MDL.IMongoQueryable<T>)query;
            }

        }


        public static MDL.IMongoQueryable<T> CreateFilterQuery<T>(this MDL.IMongoQueryable<T> query, string filter) where T : class
        {
            if (!string.IsNullOrEmpty(filter))
            {
                return (MDL.IMongoQueryable<T>)query.Where(filter);
            }
            else
            {
                return (MDL.IMongoQueryable<T>)query;
            }

        }

        public static MDL.IMongoQueryable<T> CreateSearchQuery<T>(this MDL.IMongoQueryable<T> query, string search) where T : class
        {
            if (!string.IsNullOrEmpty(search))
            {
                var properties = typeof(T)
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(o => o.CanRead && o.CanWrite)
                    .Where(o => o.PropertyType == typeof(string));
                var builder = new StringBuilder();
                foreach (var prop in properties)
                {
                    builder.Append($"{prop.Name}.Contains(\"{search}\") || ");
                }
                var temp = builder.ToString();
                var index = temp.LastIndexOf("||");
                return (MDL.IMongoQueryable<T>)query.Where(temp.Substring(0, index));
            }
            else
            {
                return (MDL.IMongoQueryable<T>)query;
            }

        }
    }


}
