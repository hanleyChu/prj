using Hoe.Basic.DB;
using Hoe.Basic.Model;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCSharp.Examples.Basics.DAO
{
    public class SemiProductDao
    {
        public static List<SemiProduct> GetAll()
        {
            List<SemiProduct> semiproducts = new List<SemiProduct>();
            MongoCursor mc = DatabaseHelper.SemiProductCollection.FindAllAs<SemiProduct>();

            foreach (SemiProduct semiproduct in mc)
            {
                semiproducts.Add(semiproduct);
            }

            if (semiproducts.Count > 0)
                return semiproducts;
            else
                return null;
        }

        public static List<SemiProduct> FindLike(String nameOrNorm, String material)
        {
            List<SemiProduct> semiproducts = new List<SemiProduct>();
            MongoCursor mc = DatabaseHelper.SemiProductCollection.FindAs<SemiProduct>(Query.And(Query.Or(new IMongoQuery[] { Query.Matches("Name", nameOrNorm), Query.Matches("Norm", nameOrNorm) }), Query.Matches("Material", material)));

            foreach (SemiProduct semiproduct in mc)
            {
                semiproducts.Add(semiproduct);
            }

            return semiproducts;

        }

        public static void Insert(SemiProduct product)
        {
            product.Id = new MongoDB.Bson.ObjectId();
            DatabaseHelper.SemiProductCollection.Insert<SemiProduct>(product);
        }

        public static void Update(SemiProduct product)
        {
            DatabaseHelper.SemiProductCollection.Save(product);
        }

        public static void Delete(SemiProduct product)
        {
            DatabaseHelper.SemiProductCollection.Remove(Query<SemiProduct>.EQ(e => e.Id, product.Id));
        }
    }
}
