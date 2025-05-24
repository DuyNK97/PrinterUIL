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
using Printer.Class.Model;
using Sunny.UI;

namespace Printer
{
    public partial class MainForm : UIForm
    {
        private readonly LabelPrinter printer;
        private readonly UnitBoxSN _boxsn;
        public MainForm()
        {
            InitializeComponent();
            printer = new LabelPrinter();
            _boxsn = new UnitBoxSN();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadPrinters();
        }

        private void LoadPrinters()
        {
            try
            {
                foreach (string printerName in PrinterSettings.InstalledPrinters)
                {
                    comboBoxPrinters.Items.Add(printerName);
                }
                if (comboBoxPrinters.Items.Count > 0)
                    comboBoxPrinters.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading printers: {ex.Message}");
            }
        }

        private void btprintunitbox_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtunitearncode.Text))
                {
                    MessageBox.Show("UPC/EAN CODE is not null");
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtunitskucode.Text))
                {
                    MessageBox.Show("SKU CODE is not null");
                    return;
                }
                string unitean_upc = txtunitearncode.Text.Trim();
                string unitsku = txtunitskucode.Text.Trim();
                if (unitsku.Length != 14)
                {
                    MessageBox.Show("SKU CODE is not correct format");
                    return;
                }
                string unititemmodel;
                if (string.IsNullOrWhiteSpace(txtunititemmodel.Text))
                {
                    MessageBox.Show("ITEM (MODEL) is not null");
                    return;
                }
                unititemmodel = txtunititemmodel.Text.Trim();

                string unitmanufacturedate;
                if (string.IsNullOrWhiteSpace(txtunitmanufacturedate.Text))
                {
                    unitmanufacturedate = DateTime.Now.ToString("yyyy.MM.dd");
                }
                else
                {
                    if (DateTime.TryParse(txtunitmanufacturedate.Text, out DateTime parsedDate))
                    {
                        unitmanufacturedate = parsedDate.ToString("yyyy.MM.dd");
                    }
                    else
                    {
                        unitmanufacturedate = DateTime.Now.ToString("yyyy.MM.dd");
                    }
                }

                string origin;
                if (string.IsNullOrWhiteSpace(txtunitorigin.Text))
                {
                    origin = "MADE IN VIETNAM / FABRIQUE AU VIETNAM";
                }
                else
                {
                    origin = txtunitorigin.Text.Trim();
                }

                string sn;
                if (string.IsNullOrWhiteSpace(txtunitsn1.Text))
                {
                    sn = _boxsn.GenerateSerialNumber();
                }

                sn = txtunitsn1.Text.Trim();

                UNITDATA unitdata = new UNITDATA
                {
                    EAN_UPC = unitean_upc,
                    SKU = unitsku,
                    ITEM_MODEL = unititemmodel,
                    MANUFACTURE_DATE = unitmanufacturedate,
                    ORIGIN = origin,
                    SN = sn
                };

                if (comboBoxPrinters.SelectedItem == null)
                {
                    MessageBox.Show("Please select a printer (e.g., Zebra ZT411).");
                    return;
                }
                string printername = comboBoxPrinters.SelectedItem.ToString();
                printer.PrintUnitBoxLabel(unitdata, printername);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing unit box label: {ex.Message}");
            }
        }

        private void btprintmiddlebox_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBoxPrinters.SelectedItem == null)
                {
                    MessageBox.Show("Please select a printer (e.g., Zebra ZT411).");
                    return;
                }

//                // Use input fields or defaults for dynamic values
//                string modelCode = txtMiddleModelCode?.Text.Trim() ?? "ET-SFR82";
//                string lotNo = txtMiddleLotNo?.Text.Trim() ?? "DUY5190003";
//                string sku = txtMiddleSKU?.Text.Trim() ?? "ET-SFR82RMBEGMX";
//                string quantity = txtMiddleQuantity?.Text.Trim() ?? "10";

//                string zmiddlestring = $@"^XA
//^LS0
//^FO0,0^BY2,3,130
//^FT500,30^BCR,,N,N^FD>:EB-A500USEGWWTYGC31Z001 003TY^FS
//^FT460,30^A0R,40,40^FH\^FDSPORT BAND^FS
//^FT415,30^A0R,40,40^FH\^FDMODEL : {modelCode}^FS
//^FT370,30^A0R,40,40^FH\^FDSKU : {sku}^FS
//^FT325,30^A0R,40,40^FH\^FDLOTNO : {lotNo}^FS
//^FT280,30^A0R,40,40^FH\^FDMADE IN VIETNAM^FS
//^FT170,30^A0R,40,40^FH\^FDQ'TY : {quantity} PCS^FS
//^FT160,350^BY2,3,100^BCR,,N,N^FD>:{modelCode}{lotNo}^FS
//^FT40,120^BY3,2,100^BER,Y,Y,N^FD8806090122965^FS
//^FO2,800^GB702,0,7^FS
//^BY2,3,130
//^FT100,820^BY300,300^BXR,10,200,0,0,1,~
//^FH\^FDIMPORTED AND MARKETED BY: SAMSUNG INDIA ELECTRONICS PVT.LTD.\0D\0A        SAMSUNG INDIA ELECTRONICS PVT. LTD\0D\0A^FS
//^PQ1,0,1,Y^XZ";

//                string printername = comboBoxPrinters.SelectedItem.ToString();
//                printer.PrintZPL(zmiddlestring, printername);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing middle box label: {ex.Message}");
            }
        }

        private void btprintmasterbox_Click(object sender, EventArgs e)
        {
            try
            {
//                if (comboBoxPrinters.SelectedItem == null)
//                {
//                    MessageBox.Show("Please select a printer (e.g., Zebra ZT411).");
//                    return;
//                }

//                // Use input fields or defaults for dynamic values
//                string modelCode = txtMasterModelCode?.Text.Trim() ?? "ET-SFR82";
//                string lotNo = txtMasterLotNo?.Text.Trim() ?? "LOTNO";
//                string sku = txtMasterSKU?.Text.Trim() ?? "MODESDADSADSADSASADL";
//                string quantity = txtMasterQuantity?.Text.Trim() ?? "50";
//                string masterNo = txtMasterNo?.Text.Trim() ?? "001";

//                string zmasterstring = $@"^XA^PON
//^FO680,60^BY2,3,24^BCR,130,N,Y,N^FD:{sku}SG^FS
//^FO600,60^A0R,60,60^FDSPORT BAND^FS
//^FO530,60^A0R,60,60^FDMODEL : {modelCode.Substring(0, Math.Min(8, modelCode.Length))}^FS
//^FO460,60^A0R,60,60^FDSKU : {sku}^FS
//^FO390,60^A0R,60,60^FDLOT NO : {lotNo}^FS
//^FO320,60^A0R,60,60^FDMADE IN VIETNAM^FS
//^FO220,60^A0R,60,60^FDQ'TY : {quantity} PCS^FS
//^FO210,440^BY2,1.0^BCR,100,N,N,N^FD{masterNo}^FS
//^FO50,200^BY4,3.0^BER,130,Y,N^FD8806090122965^FS
//^FO150,1020^BY4.3^BXR,8,200^FD{sku},{lotNo}^FS
//^FO0,1000^GB1400,0,5^FS^XZ";

//                string printername = comboBoxPrinters.SelectedItem.ToString();
//                printer.PrintZPL(zmasterstring, printername);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing master box label: {ex.Message}");
            }
        }
    }
}