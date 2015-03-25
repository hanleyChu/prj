namespace Hoe.App.UI.Dialog
{
    partial class DemandAndUnitPriceDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.demandTextBox = new System.Windows.Forms.TextBox();
            this.unitpriceTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(41, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "需求数量";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(43, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "单价(元)";
            // 
            // demandTextBox
            // 
            this.demandTextBox.Location = new System.Drawing.Point(107, 23);
            this.demandTextBox.Name = "demandTextBox";
            this.demandTextBox.Size = new System.Drawing.Size(116, 21);
            this.demandTextBox.TabIndex = 3;
            // 
            // unitpriceTextBox
            // 
            this.unitpriceTextBox.Location = new System.Drawing.Point(107, 54);
            this.unitpriceTextBox.Name = "unitpriceTextBox";
            this.unitpriceTextBox.Size = new System.Drawing.Size(116, 21);
            this.unitpriceTextBox.TabIndex = 4;
            // 
            // DemandAndUnitPriceDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 137);
            this.Controls.Add(this.unitpriceTextBox);
            this.Controls.Add(this.demandTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "DemandAndUnitPriceDialog";
            this.Text = "输入";
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.demandTextBox, 0);
            this.Controls.SetChildIndex(this.unitpriceTextBox, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox demandTextBox;
        private System.Windows.Forms.TextBox unitpriceTextBox;
    }
}