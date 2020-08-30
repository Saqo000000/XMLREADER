using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XMLREADER.Models
{
    public class MyClass
    {
        public string MyProperty { get; set; }
    }
    public class PageViewModel
    {
        public int PageNumber { get; private set; }
        public int TotalPages { get; private set; }
        public int Pagesize { get; private set; } = 1;
        public PageViewModel(int count, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            Pagesize = pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)Pagesize);
        }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
