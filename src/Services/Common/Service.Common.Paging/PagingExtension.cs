using Microsoft.EntityFrameworkCore;
using Service.Common.Collection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Common.Paging
{
    public static class PagingExtension
    {
        public static async Task<DataCollection<T>> GetPagedAsync<T>(
            this IQueryable<T> query,
            int page,
            int take)
        {
            var originalPages = page;

            if (page == 0)
            {
                var result1 = new DataCollection<T>
                {
                    Items = await query.ToListAsync(),
                    Total = await query.CountAsync(),
                    Page = originalPages
                };

                if (result1.Total > 0)
                {
                    result1.Pages = 1;
                }

                return result1;
            }
            else
            {

                page--;

                if (take == 0)
                    take = 10;

                if (page > 0)
                    page = page * take;

                var result = new DataCollection<T>
                {
                    Items = await query.Skip(page).Take(take).ToListAsync(),
                    Total = await query.CountAsync(),
                    Page = originalPages
                };

                if (result.Total > 0)
                {
                    result.Pages = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(result.Total) / take));
                }
                return result;
            }

        }

        public static DataCollection<T> GetPaged<T>(
            this IQueryable<T> query,
            int page,
            int take)
        {
            var originalPages = page;

            if (page == 0)
            {
                var _items = query.ToList();
                var result1 = new DataCollection<T>
                {
                    Items = _items,
                    Total = _items.Count,
                    Page = originalPages
                };

                if (result1.Total > 0)
                {
                    result1.Pages = 1;
                }

                return result1;
            }
            else
            {

                page--;

                if (take == 0)
                    take = 10;

                if (page > 0)
                    page = page * take;

                var _items = query.ToList();
                var _count = _items.Count;
                var _items2 = _items.Skip(page).Take(take).ToList();
                var result = new DataCollection<T>
                {
                    Items = _items2,
                    Total = _count,
                    Page = originalPages
                };

                if (result.Total > 0)
                {
                    result.Pages = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(result.Total) / take));
                }
                return result;
            }

        }

        public static DataCollection<T> GetPagedList<T>(
            this List<T> query,
            int page,
            int take,
            int total)
        {
            var result = new DataCollection<T>
            {
                Items = query,
                Total = total,
                Page = page
            };

            if (result.Total > 0)
            {
                result.Pages = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(result.Total) / take));
            }

            return result;
        }

    }
}
