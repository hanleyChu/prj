using CommonForm.UI.Dialog;
namespace Hoe.App.UI.Dialog
{
    partial class ProductWithdrawDialog : BaseDialogForm
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
            this.warehousingDateCombox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.withdrawCountTextbox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // warehousingDateCombox
            // 
            this.warehousingDateCombox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.warehousingDateCombox.FormatString = "d";
            this.warehousingDateCombox.FormattingEnabled = true;
            this.warehousingDateCombox.Location = new System.Drawing.Point(107, 58);
            this.warehousingDateCombox.Name = "warehousingDateCombox";
            this.warehousingDateCombox.Size = new System.Drawing.Size(149, 20);
            this.warehousingDateCombox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "日期";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(37, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "撤回个数";
            // 
            // withdrawCountTextbox
            // 
            this.withdrawCountTextbox.Location = new System.Drawing.Point(107, 23);
            this.withdrawCountTextbox.Name = "withdrawCountTextbox";
            this.withdrawCountTextbox.Size = new System.Drawing.Size(149, 21);
            this.withdrawCountTextbox.TabIndex = 4;
            this.withdrawCountTextbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ProductWithdrawDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 137);
            this.Controls.Add(this.withdrawCountTextbox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.warehousingDateCombox);
            this.Name = "ProductWithdrawDialog";
            this.Text = "输入";
            this.Load += new System.EventHandler(this.ProductWithdrawDialog_Load);
            this.Controls.SetChildIndex(this.warehousingDateCombox, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.withdrawCountTextbox, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox warehousingDateCombox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox withdrawCountTextbox;


    }
}