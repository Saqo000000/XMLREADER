using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using XMLReader.Models;
using XMLReader.Services.Interfaces;
using System.IO;
using System.Data;

namespace XMLReader.Services.Emplamentation
{
    class ProductService : IProductService
    {
        public int Count { get => cn; }
        #region
        /*
         * string queryDebit = $"UPDATE dbo.AccountDebit SET " +
                $"[Name]='{debit.Name}', Type={debit.Type},[Balance]={debit.Balance},UserId={debit.UserId} " +
                $"WHERE Id={debit.Id}";
            string queryCredit = $"UPDATE dbo.AccountCredit SET " +
                $"[Name]='{credit.Name}', Type={credit.Type},[Balance]={credit.Balance},UserId={credit.UserId} " +
                $"WHERE Id={credit.Id}";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlTransaction transaction = null;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();
                    using (SqlCommand cmd = new SqlCommand(queryDebit, conn, transaction)) { cmd.ExecuteNonQuery(); }

                    using (SqlCommand cmd = new SqlCommand(queryCredit, conn, transaction)) { cmd.ExecuteNonQuery(); }

                    transaction.Commit();
                }
                catch (

         */
        // const string connectionString =@"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=XML;Integrated Security=True";
        #endregion

        static readonly int cn;
        static ProductService()
        {
            cn = GetProductsCount();
        }
        static int GetProductsCount()
        {
            int count = 0;
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionStrings.XMLconnectionString))
            {
                sqlConnection.Open();
                string query = $"SELECT COUNT(id) FROM Products WHERE Count <> '{0}' AND IsPresent= '{1}'";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                count = (int)sqlCommand.ExecuteScalar();
            }
            return count;
        }

        public async Task<IEnumerable<ProductModel>> GetProducts(int offset, int fetch)
        {
            List<ProductModel> products = new List<ProductModel>();
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionStrings.XMLconnectionString))
            {
                sqlConnection.Open();
                string query = $"SELECT * FROM Products WHERE Count <> '{0}' AND IsPresent= '{1}' ORDER BY id OFFSET {offset} ROWS FETCH  FIRST {fetch} ROWS ONLY";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    while (await sqlDataReader.ReadAsync())
                    {
                        ProductModel product = new ProductModel()
                        {
                            Id = (int)sqlDataReader["id"],
                            Name = (string)sqlDataReader["Name"],
                            Count = (int)sqlDataReader["Count"],
                            Sku = (string)sqlDataReader["Sku"],
                            Price = (double)sqlDataReader["Price"],
                            OldPrice = (double)sqlDataReader["OldPrice"],
                        };
                        products.Add(product);
                    }
                }
            }
            return products;
        }

        public async Task ReadXMLWriteDB(IFormFile file)
        {
            XDocument document = new XDocument();
            string res = await ReadFormFileAsync(file);
            document = XDocument.Parse(res);
            List<XElement> xmlProducts = document.Root.Elements().ToList();
            List<string> Skus = new List<string>();



            foreach (XElement item in xmlProducts)
            {
                ProductModel product = new ProductModel();
                product.Name = item.Element("Name").Value;
                product.Count = Int32.Parse(item.Element("Count").Value);
                product.Price = double.Parse(item.Element("Price").Value);
                double oldPrice = default;
                double.TryParse(item.Element("OldPrice").Value, out oldPrice);
                product.OldPrice = oldPrice;
                product.Sku = item.Element("Sku").Value;
                string query_main = $"IF(SELECT COUNT(id) FROM [Products] WHERE  Sku=@Sku ) > 0 " +
                     $"Begin " +
                         $"UPDATE Products SET Name = @Name ,Count = @Count," +
                         $"Price = @Price, OldPrice =@OldPrice WHERE Sku =@Sku " +
                     $"END " +
                     $"ELSE BEGIN " +
                         $"INSERT  INTO Products (Name, Count, Price, OldPrice, Sku) " +
                         $"VALUES (@Name,@Count,@Price,@OldPrice,@Sku ) " +
                     $" END";
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionStrings.XMLconnectionString))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(query_main, sqlConnection);
                    sqlCommand.Parameters.AddWithValue("@Name", product.Name);
                    sqlCommand.Parameters.AddWithValue("@Count", product.Count);
                    sqlCommand.Parameters.AddWithValue("@Price", product.Price);
                    sqlCommand.Parameters.AddWithValue("@OldPrice", product.OldPrice);
                    sqlCommand.Parameters.AddWithValue("@Sku", product.Sku);

                    await sqlCommand.ExecuteNonQueryAsync();
                }
                Skus.Add(item.Element("Sku").Value);
            }
            string query_update_IsPresent = $"UPDATE Products SET IsPresent = '{0}'";
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionStrings.XMLconnectionString))
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand(query_update_IsPresent, sqlConnection);
                var ez = sqlCommand.ExecuteNonQuery();
            }
            foreach (string item in Skus)
            {
                string query_update = $"UPDATE Products SET IsPresent = '{1}' WHERE Sku ='{item}'";
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionStrings.XMLconnectionString))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(query_update, sqlConnection);
                    await sqlCommand.ExecuteNonQueryAsync();
                }
            }

        }
        private async Task<string> ReadFormFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return await Task.FromResult((string)null);
            }

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
