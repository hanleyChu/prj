using NVelocity;
using NVelocity.App;
using NVelocity.Runtime;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hoe.Reporter
{
    public partial class HtmlReporterForm : Form
    {
        private static VelocityEngine vltEngine = new VelocityEngine();



        static HtmlReporterForm()
        {
            vltEngine.SetProperty(RuntimeConstants.RESOURCE_LOADER, "file");
            vltEngine.SetProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH, @"./vm");
            vltEngine.SetProperty(RuntimeConstants.INPUT_ENCODING, "utf-8");
            vltEngine.SetProperty(RuntimeConstants.OUTPUT_ENCODING, "utf-8");
            vltEngine.Init();
        }

        public void Display(String template, Object model)
        {
            String tname = template+".html";

            Template vltTemplate = vltEngine.GetTemplate(template);
            VelocityContext vltContext = new VelocityContext();
            vltContext.Put("model", model);
            
            // write to html file
            using (StreamWriter sw = new StreamWriter(new FileStream(@"./vm/" + tname, FileMode.Create)))
            {
                vltTemplate.Merge(vltContext, sw);
            }

            // display
            webBrowser.Navigate("file:///" + System.AppDomain.CurrentDomain.BaseDirectory + "vm/" + tname);
            
            this.ShowDialog();
            
        }

        public HtmlReporterForm()
        {
            InitializeComponent();
        }

        private void HtmlReporterForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true; // this cancels the close event.
        }
    }
}
