using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MVCSharp.Core.Views;
using MVCSharp.Core;
using MVCSharp.Core.Configuration.Views;
using MVCSharp.Winforms;
using Hoe.Basic.Controller;
using Hoe.Basic.View;
using Hoe.Basic.AppLogic;
using Hoe.Basic.Model;
using System.Collections;
using Hoe.App.UI.Dialog;
using Hoe.Basic.DAO;
using MVCSharp.Examples.Basics.DAO;


namespace Hoe.UI.Repo
{
    [View(typeof(SaleTask), SaleTask.RepoView)]
    public partial class RepoForm : WinFormView, IRepoView
    {
        public RepoForm()
        {
            InitializeComponent();
        }

        public Product CurrentProduct
        {
            get
            {
                return productsDataGridView.CurrentRow == null ? null :
                       productsDataGridView.CurrentRow.DataBoundItem as Product;
            }
        }

        public SemiProduct CurrentSemiProduct
        {
            get
            {
                return semiproductsDataGridView.CurrentRow == null ? null :
                       semiproductsDataGridView.CurrentRow.DataBoundItem as SemiProduct;
            }
        }

        public int SalesCount
        {
            get
            {
                int salesCount;
                if (!int.TryParse(this.countOfProductTextBox.Text, out salesCount))
                {
                    return -1;
                }
                else
                {
                    return salesCount;
                }
            }
        }

        private void gridview_currentProductChanged(object sender, EventArgs e)
        {
            (Controller as RepoController).SetCurrentRepoProduct(CurrentProduct);
        }

        private void gridview_currentSemiProductChanged(object sender, EventArgs e)
        {
            (Controller as RepoController).SetCurrentRepoSemiProduct(CurrentSemiProduct);
        }

        private void addToBillButton_Click(object sender, EventArgs e)
        {
            (Controller as RepoController).AddCurrentProductToBill(SalesCount);
        }

        public void ShowProductsList(List<Product> products)
        {   
            this.productsDataGridView.DataSource = new BindingSource(products, null);
            this.productsDataGridView.Refresh();
        }

        public void ShowSemiProductsList(List<SemiProduct> semiproducts)
        {
            this.semiproductsDataGridView.DataSource = new BindingSource(semiproducts, null);
            this.semiproductsDataGridView.Refresh();
        }

        public void SelectProductInList(Product product)
        {
            int index = (this.productsDataGridView.DataSource as BindingSource).IndexOf(product);
            int count = this.productsDataGridView.Rows.Count;
            if(index<count && index >=0)
            {
                this.productsDataGridView.Rows[index].Selected = true;
                this.productsDataGridView.Refresh();
            }
        }

        public void SelectSemiProductInList(SemiProduct semiproduct)
        {
            int index = (this.semiproductsDataGridView.DataSource as BindingSource).IndexOf(semiproduct);
            int count = this.semiproductsDataGridView.Rows.Count;
            if (index < count && index >= 0)
            {
                this.semiproductsDataGridView.Rows[index].Selected = true;
                this.semiproductsDataGridView.Refresh();
            }
        }

        private void addSemiProductButton_Click(object sender, EventArgs e)
        {
            (Controller as RepoController).ShowSemiProductView();
        }

        private void productsDataGridView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // make right click row selected
                var hit = this.productsDataGridView.HitTest(e.X, e.Y);
                int count = this.productsDataGridView.Rows.Count;
                if (hit.RowIndex < count && hit.RowIndex >= 0)
                {
                    this.productsDataGridView.CurrentCell = this.productsDataGridView.Rows[hit.RowIndex].Cells[hit.ColumnIndex];
                    this.productsDataGridView.Rows[hit.RowIndex].Selected = true;
                    this.productsDataGridView.Focus();

                    // trigger the click event as left click to set the current product
                    this.gridview_currentProductChanged(null, null);
                }

                this.productMenu.Show(this.productsDataGridView, e.X, e.Y);
            }
        }

        private void semiproductsDataGridView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // make right click row selected
                var hit = this.semiproductsDataGridView.HitTest(e.X, e.Y);
                int count = this.semiproductsDataGridView.Rows.Count;
                if (hit.RowIndex < count && hit.RowIndex >= 0)
                {
                    this.semiproductsDataGridView.CurrentCell = this.semiproductsDataGridView.Rows[hit.RowIndex].Cells[hit.ColumnIndex];
                    this.semiproductsDataGridView.Rows[hit.RowIndex].Selected = true;
                    this.semiproductsDataGridView.Focus();

                    // trigger the click event as left click to set the current product
                    this.gridview_currentSemiProductChanged(null, null);
                }

                this.semiproductMenu.Show(this.semiproductsDataGridView, e.X, e.Y);
            }
        }

        private void backToSemiProductMenuItem_Click(object sender, EventArgs e)
        {

                Product product = CurrentProduct;
                int maxCount = product.Quantity;
                int demand = product.Demand;

                List<SemiProduct> semiproducts = (Controller as RepoController).GetRelatedSemiProducts(product);
                if (semiproducts.Count == 0)
                {
                    MessageBox.Show("不存在这个货物的半成品库或者该货物的所有半成品库存都是满的，撤回失败，如果需要撤回，请先在半成品库中新建批货");
                    return;
                }

                List<DateTime> warehousingDates = semiproducts.Select(x => x.WarehousingDate).ToList();
                ProductWithdrawDialog pwd = new ProductWithdrawDialog(maxCount,warehousingDates);
                pwd.Show();
                dynamic result = pwd.GetDialogResult();

                if (result != null)
                {
                    SemiProduct semiproduct = semiproducts.Single(x => x.WarehousingDate.Date == result.WarehousingDate.Date);
                    if (semiproduct.InitialQuantity - semiproduct.Quantity < result.Count)
                    {
                        MessageBox.Show("撤回的数量太多，超出了初始库存数");
                        return;
                    }

                    product.Quantity -= result.Count;
                    if (product.Demand == 0 && product.Quantity == 0)
                    {
                        // update product
                        (this.productsDataGridView.DataSource as BindingSource).Remove(CurrentProduct);
                        (Controller as RepoController).DeleteProduct(product);
                    }
                    else
                    {
                        (Controller as RepoController).UpdateProduct(product);
                    }

                    // update semiproduct
                    (Controller as RepoController).WithdrawProductsToSemiProducts(product, result.WarehousingDate, result.Count);
                }
                


        }

        private void deleteSemiProductMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("你确定要删掉这个批半成品货?", "确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
            {
                SemiProduct semiproduct = CurrentSemiProduct;

                (this.semiproductsDataGridView.DataSource as BindingSource).Remove(CurrentSemiProduct);
                (Controller as RepoController).DeleteSemiProduct(semiproduct);
            }

        }

        private void productsDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            string columnName = this.productsDataGridView.Columns[e.ColumnIndex].DataPropertyName;

            // Abort validation if cell is not in the Quantity column. 
            if (columnName.Equals("Quantity") || columnName.Equals("Count"))
            {
                String newValue = e.FormattedValue.ToString();
                int result;
                if (!int.TryParse(newValue as String, out result) || result<0)
                {
                    MessageBox.Show("请输入合法的数字");
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void semiproductsDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            string columnName = this.semiproductsDataGridView.Columns[e.ColumnIndex].DataPropertyName;
            
            // Abort validation if cell is not in the Quantity column. 
            if (columnName.Equals("Quantity") || columnName.Equals("InitialQuantity"))
            {
                int oldValue = (int)this.semiproductsDataGridView[e.ColumnIndex, e.RowIndex].Value;
                String newValueString = e.FormattedValue.ToString();
                
                int newValue;
                if (!int.TryParse(newValueString as String, out newValue) || newValue < 0)
                {
                    MessageBox.Show("请输入合法的数字");
                    e.Cancel = true;
                    return;
                }

                int deltaValue = (newValue - oldValue);
                if (columnName.Equals("Quantity"))
                {
                    CurrentSemiProduct.InitialQuantity += deltaValue;
                }
                else
                {
                    if(deltaValue<0 && Math.Abs(deltaValue)>CurrentSemiProduct.Quantity)
                    {
                        MessageBox.Show("初始库存数量不能使得现有库存数量小于0");
                        e.Cancel = true;
                        return; 
                    }
                    else
                    {
                        CurrentSemiProduct.Quantity += deltaValue;
                    }
                }
            }
        }

        private void productsDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //only end edit mode, we can get the real updated value
            this.productsDataGridView.EndEdit();

            (Controller as RepoController).UpdateProduct(CurrentProduct);
        }

        private SemiProduct lastModifiedSemiProduct = null;
        private void semiproductsDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            semiproductsDataGridViewTimer.Enabled = true;
            lastModifiedSemiProduct = CurrentSemiProduct;
        }

        private void nameOrNormOfProductTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                queryProducts();
            }
        }

        private void materialOfProductTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                queryProducts();
            }
        }

        private void productFindBtn_Click(object sender, EventArgs e)
        {
            queryProducts();
        }

        private void queryProducts()
        {
            (Controller as RepoController).FilterProducts(this.nameOrNormOfProductTextBox.Text.Trim(), this.materialOfProductTextBox.Text.Trim());
        }

        private void addToProductBtn_Click(object sender, EventArgs e)
        {
            int count;
            if (!int.TryParse(this.countOfSemiProductTextbox.Text, out count) || count < 0)
            {
                MessageBox.Show("分配数不能为负数");
                return;
            }
            (Controller as RepoController).AddCurrentSemiProductToProduct(CurrentSemiProduct,count);
        }

        private void findAllProductButton_Click(object sender, EventArgs e)
        {
            (Controller as RepoController).FilterProducts("", "");
        }

        public dynamic ShowAndExtractProductDemandUnitprice()
        {
            DemandAndUnitPriceDialog dupd = new DemandAndUnitPriceDialog();
            dupd.Show();
            return dupd.GetDialogResult() as dynamic;
        }

        private void productsDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            (Controller as RepoController).SetCurrentRepoProduct(CurrentProduct);
        }

        private void semiproductsDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            RefreshSemiProductStatus();
            (Controller as RepoController).SetCurrentRepoSemiProduct(CurrentSemiProduct);
        }

        private void showBillsViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (Controller as RepoController).ShowBills();
        }

        private void addSemiProductToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (Controller as RepoController).ShowSemiProductView();
        }

        public void RefreshSemiProductStatus()
        {
            this.currentSemiProductStatusLabel.Text = (Controller as RepoController).GetSemiProductStatusString(
                (this.semiproductsDataGridView.DataSource as BindingSource).Current as SemiProduct, this.fromSemiproductDatePicker.Value, this.toSemiproductDatePicker.Value);

        }

        private void fromSemiproductDatePicker_ValueChanged(object sender, EventArgs e)
        {
            RefreshSemiProductStatus();
        }

        private void toSemiproductDatePicker_ValueChanged(object sender, EventArgs e)
        {
            RefreshSemiProductStatus();
        }

        private void semiproductFindBtn_Click(object sender, EventArgs e)
        {
            querySemiProducts();
        }

        private void querySemiProducts()
        {
            (Controller as RepoController).FilterSemiProducts(this.nameOrNormOfSemiProductTextBox.Text.Trim(), this.materialOfSemiProductTextBox.Text.Trim());
        }

        private void findAllSemiProductBtn_Click(object sender, EventArgs e)
        {
            (Controller as RepoController).FilterSemiProducts("", "");
        }

        private void semiproductsDataGridViewTimer_Tick(object sender, EventArgs e)
        {
            this.semiproductsDataGridViewTimer.Enabled = false;
            (Controller as RepoController).UpdateSemiProduct(lastModifiedSemiProduct);
        }

        private void 检查数据错误修复ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Product product = CurrentProduct;
            List<Bill> all_bills = BillDao.GetAll();

            int actual_demand = 0;
            int actual_quantity = 0;

            foreach(Bill b in all_bills)
            {
                Product p = b.Products.SingleOrDefault(x=>x.Equals(product));
                if(p!=null)
                {
                    actual_demand+=p.Demand;
                    actual_quantity+=p.Quantity;
                }
            }

            product.Demand = actual_demand-actual_quantity;

            IEnumerable<SemiProduct> all_semiproducts = SemiProductDao.GetAll().FindAll(x=>(x.Name.Equals(product.Name) && x.Material.Equals(product.Material) && x.Norm.Equals(product.Norm)));
            int offered = 0;
            foreach (SemiProduct sp in all_semiproducts)
            {
               offered += sp.InitialQuantity-sp.Quantity;
            }

            product.Quantity = offered - actual_quantity;

            (Controller as RepoController).UpdateProduct(product);

        }    

    }
}
