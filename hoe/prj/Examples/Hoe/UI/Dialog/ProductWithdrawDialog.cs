using CommonForm.UI.Dialog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Hoe.App.UI.Dialog
{
    public partial class ProductWithdrawDialog : BaseDialogForm
    {
        private List<DateTime> warehousingDates;
        private int maxCount;
        public ProductWithdrawDialog(int maxCount,List<DateTime> warehousingDates)
        {
            this.maxCount = maxCount;
            this.warehousingDates = warehousingDates;
            InitializeComponent();
        }

        protected override dynamic GetDialogResultInternal()
        {
            dynamic bean = new ExpandoObject();
            int num;
            if (!int.TryParse(this.withdrawCountTextbox.Text, out num))
            {
                MessageBox.Show("请输入数字！");
                throw new DialogResultException();
            }
            if(num >maxCount)
            {
                MessageBox.Show("撤回数量必须小于现有库存！");
                throw new DialogResultException();
            }
            bean.Count = num;
            bean.WarehousingDate = this.warehousingDateCombox.SelectedValue;

            return bean;
        }

        private void ProductWithdrawDialog_Load(object sender, EventArgs e)
        {
            
            this.warehousingDateCombox.DataSource = warehousingDates;
        }
    }

    
}
