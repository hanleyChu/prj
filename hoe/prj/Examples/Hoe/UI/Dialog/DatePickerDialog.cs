using CommonForm.UI.Dialog;
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
    public partial class DatePickerDialog : BaseDialogForm
    {
        public DatePickerDialog()
        {
            InitializeComponent();
        }

        protected override Object GetDialogResultInternal()
        {
            return this.datepicker.Value;
        }
    }

    
}
