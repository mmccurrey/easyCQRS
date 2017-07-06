using System;
using System.Collections.Generic;
using System.Text;

namespace EasyCQRS
{
    public class PagedResult<T> where T : class, new()
    {
        public PagedResult()
        {
            this.Results = new List<T>();
        }

        public IList<T> Results { get; set; }
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
    }
}
