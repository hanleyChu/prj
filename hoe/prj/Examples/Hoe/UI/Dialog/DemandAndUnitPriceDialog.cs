using CommonForm.UI.Dialog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Dynamic;



namespace Hoe.App.UI.Dialog
{
    public partial class DemandAndUnitPriceDialog : BaseDialogForm
    {
        public DemandAndUnitPriceDialog()
        {
            InitializeComponent();
        }
        protected override Object GetDialogResultInternal()
        {
            dynamic bean = new ExpandoObject();
            int num;
            if (!int.TryParse(this.demandTextBox.Text, out num))
            {
                MessageBox.Show("请输入数字！");
                throw new DialogResultException();
            }
            bean.Demand = num;

            if (!int.TryParse(this.unitpriceTextBox.Text, out num))
            {
                MessageBox.Show("请输入数字！");
                throw new DialogResultException();
            }
            bean.UnitPrice = num;

            return bean;
        }


    }
}
