using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Hoe.App.UI.Admin
{
    public partial class LoginForm : Form
    {
        // whether successfully login
        public static string loginType = "exit";

        public LoginForm()
        {
            InitializeComponent();
        }

        private void textBoxCmd_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                String cmd = textBoxCmd.Text.Trim();
                if (cmd.Equals("112926")) // true branch
                {
                    loginType = "software";
                    Close();
                }
                else if (cmd.Equals("123456")) // spider game branch
                {
                    loginType = "spider";
                    Close();
                }
                else
                {
                    textBoxCmd.Text = "";
                    MessageBox.Show("口令不正确");
                }
            }
        }
    }
}
