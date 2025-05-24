using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

namespace Printer
{
    public partial class Form1 : Form
    {
        private readonly LabelPrinter printer;

        public Form1()
        {
            InitializeComponent();
            printer = new LabelPrinter();
        }

        private void Form1_Load(object sender, EventArgs e)
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

        
        private void btPrintUnitBox_Click(object sender, EventArgs e)
        {
            if (comboBoxPrinters.SelectedItem == null)
            {
                MessageBox.Show("Please select a printer (e.g., Zebra ZD421CN).");
                return;
            }

            try
            {
                string serial = printer.PrintUnitBoxLabel(comboBoxPrinters.SelectedItem.ToString());
               
                MessageBox.Show($"Unit Box label printed with S/N: {serial}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing Unit Box label: {ex.Message}");
            }
        }

        private void btPrintMiddleBox_Click(object sender, EventArgs e)
        {
            if (comboBoxPrinters.SelectedItem == null)
            {
                MessageBox.Show("Please select a printer.");
                return;
            }

            try
            {
                string lotNumber = printer.PrintMiddleBoxLabel(comboBoxPrinters.SelectedItem.ToString());
                
                MessageBox.Show($"Middle Box label printed with Lot No: {lotNumber}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing Middle Box label: {ex.Message}");
            }
        }

        private void btPrintMasterBox_Click(object sender, EventArgs e)
        {
            if (comboBoxPrinters.SelectedItem == null)
            {
                MessageBox.Show("Please select a printer.");
                return;
            }

            try
            {
                string lotNumber = printer.PrintMasterBoxLabel(comboBoxPrinters.SelectedItem.ToString());
                
                MessageBox.Show($"Master Box label printed with Lot No: {lotNumber}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing Master Box label: {ex.Message}");
            }
        }

        private void btPrintTest_Click(object sender, EventArgs e)
        {
            if (comboBoxPrinters.SelectedItem == null)
            {
                MessageBox.Show("Please select a printer.");
                return;
            }

            try
            {
                printer.PrintTestLabel(comboBoxPrinters.SelectedItem.ToString());
                MessageBox.Show("Test label printed successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing test label: {ex.Message}");
            }
        }

        private void btPrint_Click(object sender, EventArgs e)
        {
            if (comboBoxPrinters.SelectedItem == null)
            {
                MessageBox.Show("Please select a printer (e.g., Zebra ZD421CN).");
                return;
            }

            try
            {
                string serial = printer.PrintUnitBoxLabel(comboBoxPrinters.SelectedItem.ToString());

                MessageBox.Show($"Unit Box label printed with S/N: {serial}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing Unit Box label: {ex.Message}");
            }
        }
    }
}
