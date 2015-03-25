using System;
using System.Collections.Generic;
using Hoe.Basic.Model;
using Hoe.Basic.DB;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;

namespace Hoe.Basic.DAO
{
    public class ProductDao
    {
        public static List<Product> GetAll()
        {
            List<Product> products = new List<Product>();
            MongoCursor mc = DatabaseHelper.ProductCollection.FindAllAs<Product>();

            foreach (Product product in mc)
            {
                products.Add(product);
            }

            if (products.Count > 0)
                return products;
            else
                return null;
        }

        public static List<Product> FindLike(String nameOrNorm, String material)
        {
            List<Product> products = new List<Product>();
            MongoCursor mc = DatabaseHelper.ProductCollection.FindAs<Product>(Query.And(Query.Or(new IMongoQuery[]{Query.Matches("Name", nameOrNorm),Query.Matches("Norm", nameOrNorm)}),Query.Matches("Material", material)));

            foreach (Product product in mc)
            {
                products.Add(product);
            }

            return products;

        }

        public static void Insert(Product product)
        {
            product.Id = new MongoDB.Bson.ObjectId();
            DatabaseHelper.ProductCollection.Insert<Product>(product);
        }

        public static void Update(Product product)
        {
            DatabaseHelper.ProductCollection.Save(product);
        }

        public static void Delete(Product product)
        {
            DatabaseHelper.ProductCollection.Remove(Query<Product>.EQ(e => e.Id, product.Id));
        }

        
    }
}
