using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Hoe.App.UI.Dialog
{
    public abstract partial class BaseDialogForm : Form
    {
        public BaseDialogForm()
        {
            InitializeComponent();
        }

        private Object result;

        public void Show()
        {
            this.ShowDialog();
        }

        public Object GetDialogResult()
        {
            return result;
        }

        protected abstract Object GetInternalDialogResult();

        private void OkBtn_Click(object sender, EventArgs e)
        {
            result = GetInternalDialogResult();
            this.Close();
        }


    }

}
