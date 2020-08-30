using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using XMLReader.Models;
using XMLReader.Services.Emplamentation;
using XMLReader.Services.Interfaces;

namespace XMLReader.Services.ServiceExtentions
{
    public static class ServiceExtention
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<IProductService, ProductService>();
        }
    }
}
