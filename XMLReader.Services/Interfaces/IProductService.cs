using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLReader.Models;


namespace XMLReader.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductModel>> GetProducts(int offset,int fetch );
        Task ReadXMLWriteDB(IFormFile file);
        int Count { get;}
    }
}
