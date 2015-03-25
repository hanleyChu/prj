using CommonForm.UI.Dialog;
namespace Hoe.App.UI.Dialog
{
    partial class DatePickerDialog : BaseDialogForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.datepicker = new System.Windows.Forms.DateTimePicker();
            this.SuspendLayout();
            // 
            // datepicker
            // 
            this.datepicker.Location = new System.Drawing.Point(53, 47);
            this.datepicker.Name = "datepicker";
            this.datepicker.Size = new System.Drawing.Size(200, 21);
            this.datepicker.TabIndex = 1;
            // 
            // DatePickerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 137);
            this.Controls.Add(this.datepicker);
            this.Name = "DatePickerForm";
            this.Text = "输入出货日期";
            this.Controls.SetChildIndex(this.datepicker, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DateTimePicker datepicker;
    }
}