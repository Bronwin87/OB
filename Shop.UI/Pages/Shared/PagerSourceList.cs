using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.UI.Pages.Shared
{
    public class PagerSourceList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int PageCount { get; private set; }
        public bool HasPreviousPage
        {
            get { return PageIndex > 1;  }
        }
        public bool HasNextPage
        {
            get { return PageIndex < PageCount;  } 
        }

        public PagerSourceList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageCount = (int)Math.Ceiling(count / (double)pageSize);
            this.AddRange(items);
        }
        
        public static async Task<PagerSourceList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagerSourceList<T>(items, count, pageIndex, pageSize);
        }
    }
}
