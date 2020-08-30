using System;

namespace XMLReader.Models
{
    public class ProductModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
        public double? OldPrice { get; set; }
        public string Sku { get; set; } 
    }
}
