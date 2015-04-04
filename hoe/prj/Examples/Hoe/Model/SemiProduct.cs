using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace Hoe.Basic.Model
{
    public class SemiProduct:ICloneable
    {

        public ObjectId Id { get; set; }
        public String Name{set; get;}
        public String Norm { set; get; }
        public String Material { set; get; }
        public DateTime WarehousingDate { set; get; }
        public int Quantity { set; get; }
        public int InitialQuantity { set; get; }
        public String Remark { set; get; }

        public void Set(SemiProduct p)
        {
            if (this.Name.Equals(p.Name) &&
                this.Material.Equals(p.Material) &&
                this.Norm.Equals(p.Norm) &&
                this.WarehousingDate.Date.Equals(p.WarehousingDate.Date))
            {
                this.Quantity = p.Quantity;
                this.InitialQuantity = p.InitialQuantity;
                this.Remark = p.Remark;
            }
            else
            {
                throw new Exception("不能对不同类别的SemiProduct调用Set方法");
            }
        }

        public object Clone()
        {
            Product clone = this.MemberwiseClone() as Product;
            clone.Id = new ObjectId();
            
            return clone; //浅复制
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
                return false;
            SemiProduct p = obj as SemiProduct;
            if (this.Name.Equals(p.Name) &&
                this.Material.Equals(p.Material) &&
                this.Norm.Equals(p.Norm) &&
                this.WarehousingDate.Date.Equals(p.WarehousingDate.Date))
                return true;
            else
                return false;
        }

        public bool EqualsType(Object obj)
        {
            if (obj == null)
                return false;
            SemiProduct p = obj as SemiProduct;
            if (this.Name.Equals(p.Name) &&
                this.Material.Equals(p.Material) &&
                this.Norm.Equals(p.Norm) )
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
 	         return this.Material.GetHashCode()+this.Name.GetHashCode()+this.Norm.GetHashCode();
        }

    }
}
