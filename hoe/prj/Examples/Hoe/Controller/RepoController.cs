using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MVCSharp.Core;
using MVCSharp.Core.Tasks;
using MVCSharp.Core.Views;
using Hoe.Basic.AppLogic;
using Hoe.Basic.View;
using Hoe.Basic.Model;
using Hoe.Basic.AppLogic.Event;
using Hoe.Basic.DAO;
using System.Windows.Forms;
using MVCSharp.Examples.Basics.DAO;


namespace Hoe.Basic.Controller
{
    public class RepoController : ControllerBase
    {
        public override ITask Task
        {
            get { return base.Task; }
            set
            {
                base.Task = value;
                (Task as SaleTask).CurrentRepoProductChanged += CurrentRepoProductChanged;
                (Task as SaleTask).CurrentRepoSemiProductChanged += CurrentRepoSemiProductChanged;
            }
        }

        public override IView View
        {
            get {return base.View; }
            set
            {
                base.View = value;
                (Task as SaleTask).RepoProductsChanged += RepoProductsChanged;
                (Task as SaleTask).RepoSemiProductsChanged += RepoSemiProductsChanged;

                // load from database
                (Task as SaleTask).Products = ProductDao.GetAll();
                if ((Task as SaleTask).Products.Count > 0)
                    (Task as SaleTask).CurrentRepoProduct = (Task as SaleTask).Products[0];

                (Task as SaleTask).SemiProducts = SemiProductDao.GetAll();
                if ((Task as SaleTask).SemiProducts.Count > 0)
                    (Task as SaleTask).CurrentRepoSemiProduct = (Task as SaleTask).SemiProducts[0];
            }
        }

        public void FilterProducts(String nameOrName, String material)
        {
            if (String.IsNullOrEmpty(nameOrName) && String.IsNullOrEmpty(material))
                (Task as SaleTask).Products = ProductDao.GetAll();
            else
                (Task as SaleTask).Products = ProductDao.FindLike(nameOrName, material);
        }

        public void FilterSemiProducts(String nameOrName, String material)
        {
            if (String.IsNullOrEmpty(nameOrName) && String.IsNullOrEmpty(material))
                (Task as SaleTask).SemiProducts = SemiProductDao.GetAll();
            else
                (Task as SaleTask).SemiProducts = SemiProductDao.FindLike(nameOrName, material);
        }  

        public void ShowRepo()
        {
            Task.Navigator.Navigate(SaleTask.RepoView);
        }

        public void SetCurrentRepoProduct(Product product)
        {
            (Task as SaleTask).CurrentRepoProduct = product;
        }

        public void SetCurrentRepoSemiProduct(SemiProduct semiproduct)
        {
            (Task as SaleTask).CurrentRepoSemiProduct = semiproduct;
        }

        public void AddCurrentProductToBill(int salesCount)
        {
            Product currentProduct = (Task as SaleTask).CurrentRepoProduct;
            if (currentProduct == null)
            {
                MessageBox.Show("请选择货物");
                return;
            }

            if ((Task as SaleTask).CurrentBill == null)
            {
                MessageBox.Show("请选择当前订单");
                return;
            }

            if (salesCount < 0)
            {
                MessageBox.Show("请输入数量");
                return;
            }

            if (!(Task as SaleTask).CurrentBill.Products.Contains(currentProduct))
            {
                dynamic result = (View as IRepoView).ShowAndExtractProductDemandUnitprice();
                if (result!=null)
                {
                    if (result.Demand < salesCount)
                    {
                        MessageBox.Show("分配超额");
                        return;
                    }
                    else if (salesCount > currentProduct.Quantity)
                    {
                        MessageBox.Show("货存不足");
                        return;
                    }
                    else
                    {
                        (Task as SaleTask).PreAddCurrentProductToCurrentBill(result.Demand, salesCount, result.UnitPrice);
                    }
                }
                return;
            }
            else
            {
                Product p = (Task as SaleTask).CurrentBill.Products.Find(x => x.Equals(currentProduct));
                if (p.Demand < salesCount + p.Quantity)
                {
                    MessageBox.Show("分配超额");
                    return;
                }
                if (salesCount > currentProduct.Quantity)
                {
                    MessageBox.Show("货存不足");
                    return;
                }
                else
                {
                    (Task as SaleTask).AddCurrentProductToCurrentBill(salesCount);
                    return;
                }
                
            }
        }


        public void AddCurrentSemiProductToProduct(SemiProduct semiproduct, int count)
        {
            if(semiproduct.Quantity < count)
            {
                MessageBox.Show("分配超额");
                return;
            }

            semiproduct.Quantity -= count;
            this.UpdateSemiProduct(semiproduct);

            List<Product> products = (from s in (Task as SaleTask).Products
                                       where
                                           s.Norm.Equals(semiproduct.Norm)
                                           && s.Name.Equals(semiproduct.Name)
                                           && s.Material.Equals(semiproduct.Material)
                                       select s).ToList<Product>();
            Product theProduct;
            if (products.Count > 0)
            {
                theProduct = products[0];
                theProduct.Quantity += count;

                (Task as SaleTask).TriggerRepoProductsChanged(null, new ProductChangeEventArg(theProduct, ModelChangeEventArg.UPDATE));
            }
            else
            {
                theProduct = new Product() 
                { 
                    Quantity = count,
                    Name = semiproduct.Name,
                    Norm = semiproduct.Norm,
                    Material = semiproduct.Material,
                    Demand = 0,
                };

                (Task as SaleTask).Products.Add(theProduct);
                (Task as SaleTask).TriggerRepoProductsChanged(null, new ProductChangeEventArg(theProduct, ModelChangeEventArg.INSERT));
            }


        }


        public void WithdrawProductsToSemiProducts(Product p, DateTime warehousingDate,int count)
        {
            SemiProduct semiproduct = (from s in SemiProductDao.GetAll()
                        where s.WarehousingDate.Date == warehousingDate.Date
                            && s.Norm.Equals(p.Norm)
                            && s.Name.Equals(p.Name)
                            && s.Material.Equals(p.Material)
                        select s).ToList<SemiProduct>()[0];

            semiproduct.Quantity += count;
            this.UpdateSemiProduct(semiproduct);

        }

        public String GetSemiProductStatusString(SemiProduct p, DateTime fr, DateTime to)
        {
            var list = (from s in (Task as SaleTask).SemiProducts where s.WarehousingDate.Date >= fr.Date 
                            && s.WarehousingDate.Date <= to.Date 
                            && s.Norm.Equals(p.Norm)
                            && s.Name.Equals(p.Name)
                            && s.Material.Equals(p.Material)
                        select s);
            int sumQuantity = list.Select(c => c.Quantity).Sum();
            int sumInitQuantity = list.Select(c => c.InitialQuantity).Sum();

            return "总现有库存:" + sumQuantity + "; " + "总初始库存:" + sumInitQuantity;
              
        }

        public List<SemiProduct> GetRelatedSemiProducts(Product p)
        {
            List<SemiProduct> list = (from s in SemiProductDao.GetAll()
                        where 
                            s.Norm.Equals(p.Norm)
                            && s.Name.Equals(p.Name)
                            && s.Material.Equals(p.Material)
                            && s.InitialQuantity>s.Quantity
                        select s).ToList<SemiProduct>();

            return list;
        }

        public void ShowBills()
        {
            Task.Navigator.Navigate(SaleTask.BillsView);
        }

        public void ShowSemiProductView()
        {
            Task.TasksManager.StartTask(typeof(ProductTask), Task);
        }


        /*
         * observor events
         */
        private void CurrentRepoProductChanged(object sender, EventArgs e)
        {

        }

        private void CurrentRepoSemiProductChanged(object sender, EventArgs e)
        {
            (View as IRepoView).RefreshSemiProductStatus();
        }

        private void RepoProductsChanged(object sender, EventArgs e)
        {
            if (e is ProductChangeEventArg)
            {
                ProductChangeEventArg eventArg = (e as ProductChangeEventArg);
                Product product = eventArg.ChangedProduct;

                //if (eventArg.ChangingType != ModelChangeEventArg.UPDATE)
                //{
                    (View as IRepoView).ShowProductsList((Task as SaleTask).Products);
                //}
                (View as IRepoView).SelectProductInList(product);
                SetCurrentRepoProduct(product);

                //database operation
                DatabaseProductPersist(product, eventArg.ChangingType);
                
            }
            else
            {
                (View as IRepoView).ShowProductsList((Task as SaleTask).Products);
            }
        }

        private void RepoSemiProductsChanged(object sender, EventArgs e)
        {
            if (e is SemiProductChangeEventArg)
            {
                SemiProductChangeEventArg eventArg = (e as SemiProductChangeEventArg);
                SemiProduct semiproduct = eventArg.ChangedSemiProduct;

                if (eventArg.ChangingType != ModelChangeEventArg.UPDATE)
                {
                    (View as IRepoView).ShowSemiProductsList((Task as SaleTask).SemiProducts);
                }
                (View as IRepoView).SelectSemiProductInList(semiproduct);
                SetCurrentRepoSemiProduct(semiproduct);

                //database operation
                DatabaseSemiProductPersist(semiproduct, eventArg.ChangingType);

            }
            else
            {
                (View as IRepoView).ShowSemiProductsList((Task as SaleTask).SemiProducts);
            }
        }

        /*
         * product modify method
         */
        public void DeleteProduct(Product product)
        {
            // since in the form view layer, we called bindingsource.remove method, it will also delete the object of model layer
            // here we just call the event trigger method to notify the observors to take actions
            (Task as SaleTask).TriggerRepoProductsChanged(null, new ProductChangeEventArg(product, ModelChangeEventArg.REMOVE));
        }

        public void DeleteSemiProduct(SemiProduct semiproduct)
        {
            // since in the form view layer, we called bindingsource.remove method, it will also delete the object of model layer
            // here we just call the event trigger method to notify the observors to take actions
            (Task as SaleTask).TriggerRepoSemiProductsChanged(null, new SemiProductChangeEventArg(semiproduct, ModelChangeEventArg.REMOVE));
        }

        public bool CheckIfExist(Product pro)
        {
            List<Product> specificProduct = (from p in ProductDao.GetAll() where p.Equals(pro) select p).ToList();

            return specificProduct.Count >= 1 ? true : false;

        }

        public void UpdateProduct(Product product)
        {
            (Task as SaleTask).TriggerRepoProductsChanged(null, new ProductChangeEventArg(product, ModelChangeEventArg.UPDATE));
        }

        public void UpdateSemiProduct(SemiProduct semiproduct)
        {
            (Task as SaleTask).TriggerRepoSemiProductsChanged(null, new SemiProductChangeEventArg(semiproduct, ModelChangeEventArg.UPDATE));
        }

        private void DatabaseProductPersist(Product product, int changingType)
        {
            switch (changingType)
            {
                case ModelChangeEventArg.INSERT:
                    ProductDao.Insert(product);
                    break;
                case ModelChangeEventArg.UPDATE:
                    ProductDao.Update(product);
                    break;
                case ModelChangeEventArg.REMOVE:
                    ProductDao.Delete(product);
                    break;
            }
        }
        private void DatabaseSemiProductPersist(SemiProduct semiproduct, int changingType)
        {
            switch (changingType)
            {
                case ModelChangeEventArg.INSERT:
                    SemiProductDao.Insert(semiproduct);
                    break;
                case ModelChangeEventArg.UPDATE:
                    SemiProductDao.Update(semiproduct);
                    break;
                case ModelChangeEventArg.REMOVE:
                    SemiProductDao.Delete(semiproduct);
                    break;
            }
        }
    }
}
