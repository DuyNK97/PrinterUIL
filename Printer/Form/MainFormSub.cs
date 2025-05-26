using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sunny.UI;

namespace Printer
{
    public partial class MainFormSub : UIForm
    {
        public MainFormSub()
        {
            InitializeComponent();
        }

        private void MainFormSub_Load(object sender, EventArgs e)
        {
            LoadPrinters();
        }
        private void LoadPrinters()
        {
            foreach (string printerName in PrinterSettings.InstalledPrinters)
            {
                comboBoxPrinters.Items.Add(printerName);
            }
            if (comboBoxPrinters.Items.Count > 0)
                comboBoxPrinters.SelectedIndex = 0;
        }
    }
}
