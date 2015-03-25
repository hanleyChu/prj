using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CommonForm.UI.Dialog
{
    public partial class BaseDialogForm : Form
    {
        public BaseDialogForm()
        {
            InitializeComponent();
        }

        private Object result = null;

        // to implement it
        protected virtual Object GetDialogResultInternal()
        {
            throw new NotImplementedException();
        }

        public Object GetDialogResult()
        {
            return result;
        }

        public void Show()
        {
            this.ShowDialog();
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            try
            {
                result = GetDialogResultInternal();
                this.Close();
            }
            catch (Exception ee)
            {
            
            }
            
            
        }
    }

    public class DialogResultException:Exception
    {

    }
}
