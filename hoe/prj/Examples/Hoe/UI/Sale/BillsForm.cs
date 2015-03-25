using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using MVCSharp.Core.Views;
using MVCSharp.Core;
using MVCSharp.Core.Configuration.Views;
using MVCSharp.Winforms;
using Hoe.Basic.Controller;
using Hoe.Basic.View;
using Hoe.Basic.AppLogic;
using Hoe.Basic.Model;
using System.Threading;
using Hoe.Reporter;
using Hoe.App.UI.Dialog;

namespace Hoe.UI.Sale
{
    [View(typeof(SaleTask), SaleTask.BillsView)]
    public partial class BillsForm : WinFormView, IBillsView
    {
        private HtmlReporterForm reporter = new HtmlReporterForm();

        public BillsForm()
        {
            InitializeComponent();
            CustomInitialize();
        }

        public void CustomInitialize()
        {
            this.statusComboBox.DataSource = new BindingSource(new Dictionary<string, bool?>
            {
              {"全部", null},
              {"已出货完成", true},
              {"未出货完成", false}
            }, null);
            this.statusComboBox.DisplayMember = "Key";
            this.statusComboBox.ValueMember = "Value";
        }

        public void ShowBillsList(List<Bill> bills)
        {
            BindingSource bs = new BindingSource(bills, null);
            billsGridView.DataSource = bs;
        }

        
        public void ShowBillInfo(Bill bill)
        {
            if (bill != null)
            {
                this.billRemarkTextBox.Text = bill.Remark;
                this.depositTextBox.Text = bill.Deposit.ToString();
                
                BindingSource bs = new BindingSource(bill, "Products");
                this.bill_productsGridView.DataSource = bs;

                CalculateAndShowTotalPrice();
            }

            (Controller as BillsController).SetCurrentBillProduct(CurrentBillProduct);
			RefreshCurrentProduct();
        }


        public void SelectBillInList(Bill bill)
        {
            int index = (this.billsGridView.DataSource as BindingSource).IndexOf(bill);
            int count = this.billsGridView.Rows.Count;
            if (index < count && index >= 0)
            {
                this.billsGridView.Rows[index].Selected = true;
                this.billsGridView.Refresh();
            }
        }

        public Bill CurrentBill
        {
            get
            {
                return billsGridView.CurrentRow == null ? null :
                       billsGridView.CurrentRow.DataBoundItem as Bill;
            }
            //set { billsGridView.CurrentCell = billsGridView.Rows[
               //(billsGridView.DataSource as IList).IndexOf(value)].Cells[0];}
        }

        public Product CurrentBillProduct
        {
            get
            {
                return bill_productsGridView.CurrentRow == null ? null :
                       bill_productsGridView.CurrentRow.DataBoundItem as Product;
            }
        }

        public void RemoveFromProductsList(Product product)
        {
            (bill_productsGridView.DataSource as BindingSource).Remove(product);
        }

		public void RefreshCurrentProduct()
		{
			(billsGridView.DataSource as BindingSource).ResetCurrentItem();
		}

        /*
         * util method
         */

        private void ColorBillsByCompleted()
        {
            foreach (DataGridViewRow row in this.billsGridView.Rows)
            {
                if ((row.DataBoundItem as Bill).Completed)
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.Red;
                }
            }
        }

        private void CalculateAndShowTotalPrice()
        {
            float totalPrice = (Controller as BillsController).CalculateTotalPrice();

            this.totalPriceLabel.Text = "合计(￥): " + totalPrice + " 元";
        }

        /*
         *  events
         */
        private void girdView_currentBillChanged(object sender, EventArgs e)
        {
            (Controller as BillsController).SetCurrentBill(CurrentBill);
        }

        private void gridView_currentBillProductChanged(object sender, EventArgs e)
        {
           (Controller as BillsController).SetCurrentBillProduct(CurrentBillProduct);
        }

        private void showRepoButton_Click(object sender, EventArgs e)
        {
            (Controller as BillsController).ShowRepo();
        }

        private void createBillButton_Click(object sender, EventArgs e)
        {
            (Controller as BillsController).ShowBillView();
        }

        private void billsGridView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // make right click row selected
                var hit = this.billsGridView.HitTest(e.X,e.Y);
                int count = this.billsGridView.Rows.Count;
                if (hit.RowIndex < count && hit.RowIndex >= 0)
                {
                    this.billsGridView.CurrentCell = this.billsGridView.Rows[hit.RowIndex].Cells[hit.ColumnIndex];
                    this.billsGridView.Rows[hit.RowIndex].Selected = true;
                    this.billsGridView.Focus();

                    // trigger the click event as left click to set the current bill
                    this.girdView_currentBillChanged(null,null);
                }

                this.billMenu.Show(this.billsGridView, e.X, e.Y);
            }
        }

        private void billRemarkTextBox_Leave(object sender, EventArgs e)
        {
            Bill bill = CurrentBill;
            bill.Remark = this.billRemarkTextBox.Text.Trim();

            (Controller as BillsController).SetCurrentBill(bill);
            (Controller as BillsController).UpdateBill(bill);
        }

        private void deleteBillMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("你确定要删掉这个订单?", "确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
            {
                Bill currentBill = CurrentBill;
                (this.billsGridView.DataSource as BindingSource).Remove(currentBill);
                (Controller as BillsController).DeleteBill(currentBill);
            }
            
        }

        private void sortBillsMenuItem_Click(object sender, EventArgs e)
        {
            (Controller as BillsController).SortBills();
        }

        private void bill_productsDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            string columnName = this.bill_productsGridView.Columns[e.ColumnIndex].DataPropertyName;

            // Abort validation if cell is not in the UnitPrice column. 
            if (columnName.Equals("UnitPrice"))
            {
                String newValue = e.FormattedValue.ToString();
                float result;
                if (!float.TryParse(newValue as String, out result) || result < 0)
                {
                    MessageBox.Show("请输入合法的数字");
                    e.Cancel = true;
                    return;
                }
            }
            
        }

        //--------------------------------------------
        private void bill_productsGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //解决SetCurrentCellAddressCore函数的可重入调用.交给billProductsGridViewTimer来处理
            billProductsGridViewTimer.Enabled = true;   
        }
        private void billProductsGridView_timer_Tick(object sender, EventArgs e)
        {
            billProductsGridViewTimer.Enabled = false;

            (Controller as BillsController).UpdateBill(CurrentBill);
            CalculateAndShowTotalPrice();
        }
        //--------------------------------------------

        private void billsGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //only if we leave the edit mode, could we get the real updated value
            this.billsGridView.EndEdit();

            string columnName = this.billsGridView.Columns[e.ColumnIndex].DataPropertyName;

            // Abort validation if cell is not in the Quantity column. 
            if (columnName.Equals("Completed"))
            {
                if (CurrentBill.AssemblageOK == false && CurrentBill.Completed == true)
                {
                    MessageBox.Show("配货还没完成呢");
                    CurrentBill.Completed = false;
                    return;
                }

                if (CurrentBill.Completed == true)
                {
                    DatePickerDialog dpd = new DatePickerDialog();
                    dpd.Show();
                    CurrentBill.ShipmentDate = (DateTime)dpd.GetDialogResult();
                }
                else
                {
                    CurrentBill.ShipmentDate = null;
                }
                
            }

            (Controller as BillsController).UpdateBill(CurrentBill);

        }

        private void depositTextBox_Leave(object sender, EventArgs e)
        {
            float deposit = 0;

            if (!float.TryParse(this.depositTextBox.Text, out deposit))
            {
                MessageBox.Show("请输入数字");
                return;
            }

            CurrentBill.Deposit = deposit;
            (Controller as BillsController).UpdateBill(CurrentBill);
        }

        private void backupRestoreButton_Click(object sender, EventArgs e)
        {
            if (!(Controller as BillsController).TestDatabaseOK())
            {
                MessageBox.Show("亲，数据库服务没有启动~,请检查MongoDB服务是否已经正常启动了");
                return;
            }
            else
            {
                (Controller as BillsController).ShowBackupRestoreView();
            }
        }

        private void phoneSearchButton_Click(object sender, EventArgs e)
        {
            String phone = this.billSearchContentTextBox.Text.Trim();
            if (String.IsNullOrEmpty(phone))
            {
                MessageBox.Show("求输入电话号码，哥");
                return;
            }
            else
            {
                if((bool?)this.statusComboBox.SelectedValue == null) // find all
                    (Controller as BillsController).FilterBillsByPhone(phone);
                else
                    (Controller as BillsController).FilterBillsByPhone(phone, (bool)this.statusComboBox.SelectedValue);
            }
        }

        private void allBillButton_Click(object sender, EventArgs e)
        {
            if ((bool?)this.statusComboBox.SelectedValue == null) // find all
                (Controller as BillsController).AllBills();
            else
                (Controller as BillsController).AllBills((bool)this.statusComboBox.SelectedValue);
        }

        private void billProductSearchButton_Click(object sender, EventArgs e)
        {
            String content = this.billSearchContentTextBox.Text.Trim();
            if (String.IsNullOrEmpty(content))
            {
                MessageBox.Show("请输入搜索内容");
                return;
            }

            (Controller as BillsController).FilterBillsByProductName(content,(bool?)this.statusComboBox.SelectedValue);
        }

        private void statUnfinishedBillMenuItem_Click(object sender, EventArgs e)
        {
            reporter.Display("unfinishedBills.vm", (Controller as BillsController).GetUnfinishedBills());
        }

        // TODO: right click context menu to help incrementing and decrementing product on current product
        private void bills_productsGridView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // make right click row selected
                var hit = this.bill_productsGridView.HitTest(e.X, e.Y);
                int count = this.bill_productsGridView.Rows.Count;
                if (hit.RowIndex < count && hit.RowIndex >= 0)
                {
                    this.bill_productsGridView.CurrentCell = this.bill_productsGridView.Rows[hit.RowIndex].Cells[hit.ColumnIndex];
                    this.bill_productsGridView.Rows[hit.RowIndex].Selected = true;
                    this.bill_productsGridView.Focus();

                    // trigger selection event manually
                    this.gridView_currentBillProductChanged(null, null);
                }

                this.billProductMenu.Show(this.bill_productsGridView, e.X, e.Y);
            }
        }

        private void cancelBillProductMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("你确定你确定你确定????", "确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
            {
                if(!(Controller as BillsController).CheckIfExistInRepo(CurrentBillProduct))
                {
                    DialogResult dr = MessageBox.Show(@"发现不一致状态，你要撤销的货并不存在于仓库里，有可能你曾经删除了它，撤销之后你要添加到仓库吗：
  Yes. 撤销之后将这个货物将添加到仓库，货存=当前数量，需求=0
  No. 撤销之后直接丢弃这个货物", "警告", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button3);
                    if (dr == DialogResult.Yes)
                    {
                        (Controller as BillsController).CancelProductToRepo(CurrentBillProduct, false);
                    }
                    else if(dr == DialogResult.No)
                    {
                        (Controller as BillsController).CancelProductToRepo(CurrentBillProduct, true);
                    }
                }
                else
                {
                    (Controller as BillsController).CancelProductToRepo(CurrentBillProduct,true);
                }
                
            }
        }

        private void returnAllBillProductMenuItem_Click(object sender, EventArgs e)
        {
            (Controller as BillsController).ReturnProductToRepo(CurrentBillProduct, CurrentBillProduct.Quantity);
        }

        

    }
}