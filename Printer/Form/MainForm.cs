using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OfficeOpenXml;
using Printer.Class;
using Printer.Class.Model;
using Sunny.UI;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace Printer
{
    public partial class MainForm : UIForm
    {
        private readonly LabelPrinter printer;
        private readonly UnitBoxSN _boxsn;
        private static string customer = "";
        private static string ordertype = "";
        private static string productGroup = "";
        private static string vendorcode = "";
        private string mastervendorcode = "";
        private string middlevendorcode = "";
        private int middleboxqty = 0;
        private bool auto = false;

        public MainForm()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            InitializeComponent();
            printer = new LabelPrinter();
            _boxsn = new UnitBoxSN();
            InitializeRadioButtonEvents();


            Global.configmodel = Global.ReadConfigs(Path.Combine(Global.Configfile, "CONFIGMODEL.xlsx"));



        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            dateMasterBox.Value = datepick.Value = DateTime.Today;
            LoadPrinters();
            LoadModel();
            string settingPath = Global.GetFilePathSetting();

            if (!File.Exists(settingPath))
            {
                var defaultValues = new Dictionary<string, string>
                {
                    ["CurrentUnitSerial"] = "0",
                    ["CurrentMiddleSerial"] = "0",
                    ["CurrentMasterSerial"] = "0",
                    ["UnitExcelfoler"] = @"D:\Printer\Unit",
                    ["MiddleExcelfoler"] = @"D:\Printer\Middle",
                    ["MasterExcelfoler"] = @"D:\Printer\Master",
                    ["CartonExcelfoler"] = @"D:\Printer\Carton",
                    ["MiddlePrinter"] = @"ZDesigner ZT411-300dpi ZPL (1)",
                    ["MasterPrinter"] = @"ZDesigner ZT410-300dpi ZPL",
                    ["MasterPrinterCarton"] = @"ZDesigner ZT411-300dpi ZPL (IN)",
                    ["SNlen"] = "14",
                    ["LastMonthCode"] = "5",
                    ["LastDateCode"] = "29",

                };

                Global.WriteFileToTxt(settingPath, defaultValues);
            }
            Dictionary<string, string> currentData = Global.ReadValueFileTxt(Global.GetFilePathSetting(), new List<string> { "CurrentUnitSerial", "CurrentMiddleSerial", "CurrentMasterSerial", "UnitExcelfoler", "MiddleExcelfoler", "MasterExcelfoler", "LastMonthCode", "LastDateCode", "SNlen", "MiddlePrinter", "MasterPrinter", "MasterPrinterCarton", "CartonExcelfoler" });

            Global.CurrentUnitSerial = currentData["CurrentUnitSerial"];
            Global.CurrentMiddleSerial = currentData["CurrentMiddleSerial"];
            Global.CurrentMasterSerial = currentData["CurrentMasterSerial"];
            Global.UnitExcelfoler = currentData["UnitExcelfoler"];
            Global.MiddleExcelfoler = currentData["MiddleExcelfoler"];
            Global.MasterExcelfoler = currentData["MasterExcelfoler"];
            Global.LastMonthCode = currentData["LastMonthCode"];
            Global.LastDateCode = currentData["LastDateCode"];
            Global.SNlen = int.Parse(currentData["SNlen"]);
            Global.MiddlePrinter = currentData["MiddlePrinter"];
            Global.MasterPrinter = currentData["MasterPrinter"];
            Global.MasterPrinterCarton = currentData["MasterPrinterCarton"];
            Global.CartonExcelfoler = currentData["CartonExcelfoler"];

            rdomanual.Checked = true;
            rdoSEV.Checked = true;
            rdohhp.Checked = true;


        }




        private void InitializeRadioButtonEvents()
        {
            rdocustomerdirectorder.CheckedChanged += RadioButton_CheckedChanged;
            rdoSEDAM.CheckedChanged += RadioButton_CheckedChanged;
            rdoSEDAC.CheckedChanged += RadioButton_CheckedChanged;
            rdoSEC.CheckedChanged += RadioButton_CheckedChanged;
            rdoSIELN.CheckedChanged += RadioButton_CheckedChanged;
            rdoSEIN.CheckedChanged += RadioButton_CheckedChanged;
            rdoTSTC.CheckedChanged += RadioButton_CheckedChanged;
            rdoSEVT.CheckedChanged += RadioButton_CheckedChanged;
            rdoSEV.CheckedChanged += RadioButton_CheckedChanged;
            rdoinbox.CheckedChanged += RadioButton_CheckedChangedOrdertype;
            rdodirect.CheckedChanged += RadioButton_CheckedChangedOrdertype;

            rdomp3.CheckedChanged += RadioButton_CheckedChangedProducttype;
            rdohhp.CheckedChanged += RadioButton_CheckedChangedProducttype;
            rdomedicaldevices.CheckedChanged += RadioButton_CheckedChangedProducttype;

            rdomanual.CheckedChanged += RadioButton_CheckedChangedRunmode;
            rdoauto.CheckedChanged += RadioButton_CheckedChangedRunmode;
        }
        private void RadioButton_CheckedChangedRunmode(object sender, EventArgs e)
        {

            if (rdomanual.Checked)
            {
                rdomanual.Checked = true;
                rdoauto.Checked = false;
                auto = false;
            }
            else if (rdoauto.Checked)
            {

                rdomanual.Checked = false;
                rdoauto.Checked = true;
                auto = true;
            }

        }

        private void RadioButton_CheckedChangedProducttype(object sender, EventArgs e)
        {

            if (rdomp3.Checked)
            {
                rdomp3.Checked = true;
                rdohhp.Checked = false;
                rdomedicaldevices.Checked = false;
                productGroup = "1";
            }
            else if (rdohhp.Checked)
            {
                rdomp3.Checked = false;
                rdohhp.Checked = true;
                rdomedicaldevices.Checked = false;
                productGroup = "R";
            }
            else if (rdomedicaldevices.Checked)
            {
                rdomp3.Checked = false;
                rdohhp.Checked = false;
                rdomedicaldevices.Checked = true;
                productGroup = "1";
            }
        }
        private void RadioButton_CheckedChangedOrdertype(object sender, EventArgs e)
        {

            if (rdoinbox.Checked)
            {
                rdoinbox.Checked = true;
                rdodirect.Checked = false;
                ordertype = "A";
            }
            else if (rdodirect.Checked)
            {
                rdoinbox.Checked = false;
                rdodirect.Checked = true;
                ordertype = "B";
            }
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is UIRadioButton radioButton && radioButton.Checked)
            {
                // Gán giá trị customer dựa trên RadioButton được chọn
                switch (radioButton)
                {
                    case var rb when rb == rdocustomerdirectorder:
                        customer = "A";
                        break;
                    case var rb when rb == rdoSEDAM:
                        customer = "X";
                        break;
                    case var rb when rb == rdoSEDAC:
                        customer = "Q";
                        break;
                    case var rb when rb == rdoSEC:
                        customer = "3";
                        break;
                    case var rb when rb == rdoSIELN:
                        customer = "Z";
                        break;
                    case var rb when rb == rdoSEIN:
                        customer = "R";
                        break;
                    case var rb when rb == rdoTSTC:
                        customer = "V";
                        break;
                    case var rb when rb == rdoSEVT:
                        customer = "5";
                        break;
                    case var rb when rb == rdoSEV:
                        customer = "F";
                        break;
                }
            }
            //if (rdoSEV.Checked)
            //{
            //    rdoSEV.Checked = true;
            //    rdoSEVT.Checked = false;
            //    customer = "F";
            //}
            //else if (rdoSEVT.Checked)
            //{
            //    rdoSEV.Checked = false;
            //    rdoSEVT.Checked = true;
            //    customer = "5";
            //}
        }
        private void LoadModel()
        {
            try
            {
                foreach (var model in Global.configmodel)
                {
                    cmbModel.Items.Add(model.Model);
                }
                if (cmbModel.Items.Count > 0)
                    cmbModel.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading printers: {ex.Message}");
            }
        }
        private void LoadPrinters()
        {
            try
            {
                foreach (string printerName in PrinterSettings.InstalledPrinters)
                {
                    comboBoxPrinters.Items.Add(printerName);
                    cbxMasterPrinterCartonName.Items.Add(printerName);
                }
                if (comboBoxPrinters.Items.Count > 0)
                    comboBoxPrinters.SelectedIndex = 0;
                if (comboBoxPrinters.Items.Count > 1)
                    cbxMasterPrinterCartonName.SelectedIndex = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading printers: {ex.Message}");
            }
        }
        int a = 0;
        private void btprintunitbox_Click(object sender, EventArgs e)
        {
            try
            {
                string sn = "";
                txtunitsn1.Text = "";
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
                if (numqty.Value <= 0)
                {
                    MessageBox.Show("Qty <= 0");
                    return;
                }

                unititemmodel = txtunititemmodel.Text.Trim();

                string unitmanufacturedate;
                //if (string.IsNullOrWhiteSpace(txtunitmanufacturedate.Text))
                //{
                //    unitmanufacturedate = DateTime.Now.ToString("yyyy.MM.dd");
                //}
                //else
                //{
                //    if (DateTime.TryParse(txtunitmanufacturedate.Text, out DateTime parsedDate))
                //    {
                //        unitmanufacturedate = parsedDate.ToString("yyyy.MM.dd");
                //    }
                //    else
                //    {
                //        unitmanufacturedate = DateTime.Now.ToString("yyyy.MM.dd");
                //    }
                //}
                if (string.IsNullOrWhiteSpace(datepick.Value.ToString()))
                {
                    unitmanufacturedate = DateTime.Now.ToString("yyyy.MM.dd");
                }
                else
                {
                    if (DateTime.TryParse(datepick.Value.ToString(), out DateTime parsedDate))
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

                if (!string.IsNullOrWhiteSpace(txvendor.Text))
                {
                    vendorcode = txvendor.Text.ToUpper();
                }


                if (string.IsNullOrWhiteSpace(txtunitsn1.Text))
                {
                    if (string.IsNullOrWhiteSpace(customer))
                    {
                        MessageBox.Show("Please select Customer!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(productGroup))
                    {
                        MessageBox.Show("Please select Product Type!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(ordertype))
                    {
                        MessageBox.Show("Please select Order Type!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(vendorcode))
                    {
                        MessageBox.Show("Please input Vendor Code!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else if (vendorcode.Length != 2)
                    {
                        MessageBox.Show("Vendor code must be 2 characters!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }



                    char monthCode = "123456789ABC"[DateTime.Now.Month - 1];
                    string printername = comboBoxPrinters.SelectedItem?.ToString();

                    if (string.IsNullOrWhiteSpace(printername))
                    {
                        MessageBox.Show("Please select a printer (e.g., Zebra ZT411).");
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(txvendor.Text))
                    {
                        MessageBox.Show("Please input Vendor code.");
                        return;
                    }



                    string color = txunitcolor.Text;

                    //switch (unitsku)
                    //{
                    //    case "ET-SLL50LWEGUJ":
                    //    case "ET-SNL33LWEGUJ":
                    //        color = "WHITE";
                    //        break;
                    //    case "ET-SLL50LNEGUJ":
                    //    case "ET-SNL33LNEGUJ":
                    //        color = "NAVY";
                    //        break;
                    //    case "ET-SLL50LJEGUJ":
                    //        color = "TAUPE";
                    //        break;
                    //    case "ET-SLL50LBEGUJ":
                    //        color = "BLACK";
                    //        break;
                    //    case "ET-SLL50LAEGUJ":
                    //        color = "CARMEL";
                    //        break;
                    //    case "ET-SNL33LPEGUJ":
                    //        color = "PINK";
                    //        break;
                    //    case "ET-SNL33LMEGUJ":
                    //        color = "MINT";
                    //        break;
                    //    case "ET-SNL33LBEGUJ":
                    //        color = "DARK GRAY";
                    //        break;
                    //    default:
                    //        color = "";
                    //        break;
                    //}

                    int successCount = 0;


                    for (int i = 1; i <= numqty.Value; i++)
                    {
                        sn = Global.GenerateSerialNumber(
                            productGroup[0],
                            customer[0],
                            '7',
                            monthCode,
                            vendorcode,
                            ordertype[0]
                        );

                        UNITDATA unitdata = new UNITDATA
                        {
                            EAN_UPC = unitean_upc,
                            SKU = unitsku,
                            ITEM_MODEL = unititemmodel,
                            MANUFACTURE_DATE = unitmanufacturedate,
                            ORIGIN = origin,
                            SN = sn,
                            COLOR = color,
                        };

                        bool result = PrintAndSaveLabel(unitdata, printername);
                        if (result)
                        {
                            successCount++;
                        }

                        Task.Delay(100);
                    }
                    if (successCount == numqty.Value)
                    {
                        numqty.Value = 0;
                        MessageBox.Show($"{successCount} label(s) printed successfully.");
                    }
                    else
                    {
                        MessageBox.Show("Failed to print any labels.");
                    }



                }
                //Action UpdateSN = () =>
                //{

                //    txtunitsn1.Text = sn;
                //};

                //if (this.InvokeRequired)
                //    this.Invoke(UpdateSN);
                //else
                //    UpdateSN();

                //else
                //{
                //    sn = txtunitsn1.Text.Trim();
                //}              


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing unit box label: {ex.Message}");
            }
        }
        private bool PrintAndSaveLabel(UNITDATA unitdata, string printerName)
        {
            bool printSuccess = printer.PrintUnitBoxLabelbool(unitdata, printerName);

            if (printSuccess)
            {
                long serial = Global.FromBase33(unitdata.SN.Substring(5, 5));
                Global.SaveLastSerialToSetting("CurrentUnitSerial", serial);
                Global.CreateExcelFile(Global.UnitExcelfoler, unitdata);
                return true;
            }

            return false;
        }

        private bool RePrintAndSaveLabel(UNITDATA unitdata, string printerName)
        {
            bool printSuccess = printer.PrintUnitBoxLabelbool(unitdata, printerName);

            if (printSuccess)
            {
                Global.CreateExcelFile(Global.UnitExcelfoler, unitdata, "Reprint");
                return true;
            }

            return false;
        }

        private void btprintmiddlebox_Click(object sender, EventArgs e)
        {
            try
            {



                if (string.IsNullOrWhiteSpace(txmidleitem.Text))
                {
                    MessageBox.Show("Item is not null");
                    return;
                }
                if (string.IsNullOrWhiteSpace(txmidlesku.Text))
                {
                    MessageBox.Show("SKU CODE is not null");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txmidlebarcodeean.Text))
                {
                    MessageBox.Show("UPC/EAN CODE is not null");
                    return;
                }
                string origin;
                if (string.IsNullOrWhiteSpace(txmidleorigin.Text))
                {
                    origin = "MADE IN VIETNAM";
                }
                else
                {
                    origin = txmidleorigin.Text.Trim();
                }
                middlevendorcode = txmiddlevendorcode.Text.ToUpper();



                if (string.IsNullOrWhiteSpace(middlevendorcode))
                {
                    MessageBox.Show("Please input Vendor Code!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (middlevendorcode.Length != 2)
                {
                    MessageBox.Show("Vendor code must be 2 characters!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txmidleqty.Text))
                {
                    MessageBox.Show("Please input Q'ty!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!int.TryParse(txmidleqty.Text, out int qty))
                {
                    MessageBox.Show("Quantity must be a valid integer!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txmidlebarcodeean.Text))
                {
                    MessageBox.Show("Please input EAN/UPC code!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                string model = txmidlesku.Text.Substring(0, 9);




                string lotno = txmidlelotno.Text;
                if (lotno.Length != 10)
                {
                    MessageBox.Show("Lot no is 10 character!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;

                }



                string middleqty = txmidleqty.Text + " PCS";
                string barcodelotno = txmidlesku.Text + lotno + " " + qty.ToString("D3") + middlevendorcode;

                Action Updatbarcodelotno = () =>
                {

                    txmidlelotnobarcode.Text = barcodelotno;
                };

                if (this.InvokeRequired)
                    this.Invoke(Updatbarcodelotno);
                else
                    Updatbarcodelotno();

                long newmodel = long.Parse(Global.CurrentMiddleSerial) + 1;


                string barcodemodel = txmidlesku.Text + newmodel.ToString("D3");
                Action Updatmodel = () =>
                {

                    txmidlemodel.Text = model;
                    txmidlebarcodemodel.Text = barcodemodel;
                };

                if (this.InvokeRequired)
                    this.Invoke(Updatmodel);
                else
                    Updatmodel();


                var serialNumbers = dgvsn.Rows
              .Cast<DataGridViewRow>()
              .Where(row => !row.IsNewRow)
              .Select(row => row.Cells[0].Value?.ToString())
              .Where(value => !string.IsNullOrEmpty(value))
              .ToList();
                if (serialNumbers.Count() <= 0)
                {
                    MessageBox.Show("Please input SN!");
                    return;
                }

                var selectedSerials = serialNumbers.Take(qty)
                    .ToList();


                if (comboBoxPrinters.SelectedItem == null)
                {
                    MessageBox.Show("Please select a printer (e.g., Zebra ZT411).");
                    return;
                }


                string serialNumbersString = string.Join(",", selectedSerials);
                string matrixdata = txmidlesku.Text + "," + lotno + "," + qty.ToString("D3") + "," + serialNumbersString;

                MIDDLECODE middledata = new MIDDLECODE
                {
                    EAN_UPC = txmidlebarcodeean.Text,
                    SKU = txmidlesku.Text,
                    Item = txmidleitem.Text,
                    BarcodeLotno = barcodelotno,
                    MODEL = model,
                    LOTNO = lotno,
                    BarcodeMODEL = barcodemodel,
                    QTY = qty.ToString(),
                    ORIGIN = origin,
                    Matrixdata = matrixdata,
                };
                string printername = comboBoxPrinters.SelectedItem.ToString();


                DialogResult confirmResult = MessageBox.Show(
                   "Are you sure you want to print the middle box label?",
                   "Confirm Print",
                   MessageBoxButtons.YesNo,
                   MessageBoxIcon.Question
               );

                if (confirmResult == DialogResult.Yes)
                {
                    printer.PrintMiddleBoxLabel(printername, middledata);
                    if (int.Parse(lblqty.Text) == 50)
                    {
                        dgvsn.Rows.Clear();
                        lblqty.Text = "0";
                    }

                    middleboxqty = 0;
                    txmidlelotno.Text = "";
                    txmidlelotnobarcode.Text = "";
                    txmidleqty.Text = "0";
                    txmidlebarcodemodel.Text = "";
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing middle box label: {ex.Message}");
            }
        }
        public void printmiddlelabel(MIDDLECODE middledata)
        {

            string printername = comboBoxPrinters.SelectedItem.ToString();




            printer.PrintMiddleBoxLabel(printername, middledata);
            if (int.Parse(lblqty.Text) == 50)
            {
                dgvsn.Rows.Clear();
                lblqty.Text = "0";
            }

            middleboxqty = 0;
            txmidlelotno.Text = "";
            txmidlelotnobarcode.Text = "";
            txmidleqty.Text = "0";
            txmidlebarcodemodel.Text = "";


        }


        private void txmidleitem_KeyPress(object sender, KeyPressEventArgs e)
        {

            try
            {
                if (e.KeyChar == (Char)Keys.Enter)
                {
                    if (!string.IsNullOrWhiteSpace(txmidleitem.Text.Trim()))
                    {
                        txmiddlesn.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Global.WriteLog("txmidleitem_KeyPress fail:" + ex.Message);
            }
        }
        private void txtunitearncode_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (Char)Keys.Enter)
                {
                    if (!string.IsNullOrWhiteSpace(txtunitearncode.Text.Trim()))
                    {
                        txtunitskucode.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Global.WriteLog("txtunitearncode_KeyPress fail:" + ex.Message);
            }
        }
        private void txvendor_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (Char)Keys.Enter)
                {
                    if (!string.IsNullOrWhiteSpace(txvendor.Text.Trim()))
                    {

                        if (!string.IsNullOrWhiteSpace(txvendor.Text))
                        {
                            vendorcode = txvendor.Text.ToUpper();
                        }
                        if (string.IsNullOrWhiteSpace(vendorcode))
                        {
                            MessageBox.Show("Please input Vendor Code!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        else if (vendorcode.Length != 2)
                        {
                            MessageBox.Show("Vendor code must be 2 characters!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        txtunitearncode.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Global.WriteLog("txvendor_KeyPress fail:" + ex.Message);
            }
        }

        private void txtunitskucode_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (Char)Keys.Enter)
                {
                    if (!string.IsNullOrWhiteSpace(txtunitskucode.Text.Trim()))
                    {
                        txmiddlesn.Focus();
                        string color;

                        switch (txtunitskucode.Text.Trim())
                        {
                            case "ET-SLL50LWEGUJ":
                            case "ET-SNL33LWEGUJ":
                                color = "WHITE";
                                break;
                            case "ET-SLL50LNEGUJ":
                            case "ET-SNL33LNEGUJ":
                                color = "NAVY";
                                break;
                            case "ET-SLL50LJEGUJ":
                                color = "TAUPE";
                                break;
                            case "ET-SLL50LBEGUJ":
                                color = "BLACK";
                                break;
                            case "ET-SLL50LAEGUJ":
                                color = "CARMEL";
                                break;
                            case "ET-SNL33LPEGUJ":
                                color = "PINK";
                                break;
                            case "ET-SNL33LMEGUJ":
                                color = "MINT";
                                break;
                            case "ET-SNL33LBEGUJ":
                                color = "DARK GRAY";
                                break;
                            default:
                                color = "";
                                break;
                        }


                        txunitcolor.Text = color;

                    }
                }
            }
            catch (Exception ex)
            {
                Global.WriteLog("txtunitskucode_KeyPress fail:" + ex.Message);
            }
        }

        private void txtunititemmodel_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (Char)Keys.Enter)
                {
                    if (!string.IsNullOrWhiteSpace(txtunititemmodel.Text.Trim()))
                    {
                        txvendor.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Global.WriteLog("txtunititemmodel_KeyPress fail:" + ex.Message);
            }

        }

        private bool IsMiddleDuplicate(string sn)
        {
            foreach (DataGridViewRow row in dgvsn.Rows)
            {
                if (!row.IsNewRow && row.Cells[0].Value != null && row.Cells[0].Value.ToString() == sn)
                {
                    return true;
                }
            }
            return false;
        }
        private void txmiddlesn_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    string sn = txmiddlesn.Text.Trim();
                    if (!string.IsNullOrWhiteSpace(sn))
                    {
                        if (sn.Length != Global.SNlen)
                        {
                            MessageBox.Show($"Serial number not correct :!{sn}", "SN Format", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txmiddlesn.Clear();
                            return;
                        }
                        if (IsMiddleDuplicate(sn))
                        {
                            MessageBox.Show("Serial number already exists!", "Duplicate SN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txmiddlesn.Clear();
                            return;
                        }
                        else
                        {
                            dgvsn.Rows.Insert(0, new object[] { sn });
                            middleboxqty++;
                            txmiddlesn.Clear();

                            var serialNumbers = dgvsn.Rows
                                .Cast<DataGridViewRow>()
                                .Where(row => !row.IsNewRow)
                                .Select(row => row.Cells[0].Value?.ToString())
                                .Where(value => !string.IsNullOrEmpty(value))
                                .ToList();

                            txmidleqty.Text = middleboxqty.ToString();

                            lblqty.Text = serialNumbers.Count.ToString();
                            if (auto)
                            {

                                if (int.Parse(lblqty.Text) % 10 == 0 && int.Parse(lblqty.Text) <= 50)
                                {
                                    // Validate input fields
                                    if (string.IsNullOrWhiteSpace(txmidleitem.Text))
                                    {
                                        MessageBox.Show("Item is not null");
                                        return;
                                    }
                                    if (string.IsNullOrWhiteSpace(txmidlesku.Text))
                                    {
                                        MessageBox.Show("SKU CODE is not null");
                                        return;
                                    }
                                    if (string.IsNullOrWhiteSpace(txmidlebarcodeean.Text))
                                    {
                                        MessageBox.Show("UPC/EAN CODE is not null");
                                        return;
                                    }
                                    if (string.IsNullOrWhiteSpace(middlevendorcode))
                                    {
                                        MessageBox.Show("Please input Vendor Code!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        return;
                                    }
                                    if (middlevendorcode.Length != 2)
                                    {
                                        MessageBox.Show("Vendor code must be 2 characters!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        return;
                                    }
                                    if (string.IsNullOrWhiteSpace(txmidleqty.Text) || !int.TryParse(txmidleqty.Text, out int qty))
                                    {
                                        MessageBox.Show("Please input valid Q'ty!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        return;
                                    }
                                    if (string.IsNullOrWhiteSpace(txmidlelotno.Text) || txmidlelotno.Text.Length != 10)
                                    {
                                        MessageBox.Show("Lot no is 10 characters!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        return;
                                    }
                                    if (comboBoxPrinters.SelectedItem == null)
                                    {
                                        MessageBox.Show("Please select a printer (e.g., Zebra ZT411).");
                                        return;
                                    }
                                    if (cbxMasterPrinterCartonName.SelectedItem == null)
                                    {
                                        MessageBox.Show("Please select a printer (e.g., Zebra ZT411).");
                                        return;
                                    }

                                    // Tính toán chỉ số bắt đầu và kết thúc để lấy 10 SN từ dưới lên
                                    int totalRows = serialNumbers.Count;
                                    int batchIndex = totalRows / 10; // Xác định lô thứ mấy (1, 2, 3, 4, 5)
                                    int startIndex = Math.Max(0, totalRows - (batchIndex * 10)); // Chỉ số bắt đầu
                                    int endIndex = Math.Min(totalRows, startIndex + 10); // Chỉ số kết thúc
                                    int takeCount = endIndex - startIndex;

                                    if (takeCount < 10)
                                    {
                                        MessageBox.Show("Not enough serial numbers to print (need 10)!");
                                        return;
                                    }

                                    // Lấy 10 SN từ dưới lên

                                    var selectedSerials = serialNumbers
                                        .Skip(startIndex)
                                        .Take(10)
                                        .ToList();

                                    // Cập nhật thông tin cho nhãn
                                    string origin = string.IsNullOrWhiteSpace(txmidleorigin.Text)
                                        ? "MADE IN VIETNAM"
                                        : txmidleorigin.Text.Trim();

                                    string model = txmidlesku.Text.Substring(0, 9);
                                    string lotno = txmidlelotno.Text;
                                    string middleqty = qty.ToString() + " PCS";
                                    string barcodelotno = txmidlesku.Text + lotno + " " + qty.ToString("D3") + middlevendorcode;

                                    long newmodel = long.Parse(Global.CurrentMiddleSerial) + 1;
                                    string barcodemodel = txmidlesku.Text + newmodel.ToString("D3");

                                    // Cập nhật UI
                                    Action updateUI = () =>
                                    {
                                        txmidlelotnobarcode.Text = barcodelotno;
                                        txmidlemodel.Text = model;
                                        txmidlebarcodemodel.Text = barcodemodel;
                                    };

                                    if (this.InvokeRequired)
                                        this.Invoke(updateUI);
                                    else
                                        updateUI();

                                    string serialNumbersString = string.Join(",", selectedSerials);
                                    string matrixdata = txmidlesku.Text + "," + lotno + "," + qty.ToString("D3") + "," + serialNumbersString;

                                    MIDDLECODE middledata = new MIDDLECODE
                                    {
                                        EAN_UPC = txmidlebarcodeean.Text,
                                        SKU = txmidlesku.Text,
                                        Item = txmidleitem.Text,
                                        BarcodeLotno = barcodelotno,
                                        MODEL = model,
                                        LOTNO = lotno,
                                        BarcodeMODEL = barcodemodel,
                                        QTY = qty.ToString(),
                                        ORIGIN = origin,
                                        Matrixdata = matrixdata,
                                    };

                                    string printername = Global.MiddlePrinter;


                                    printer.PrintMiddleBoxLabel(printername, middledata);


                                    middleboxqty = 0;
                                    txmidlelotno.Text = "";
                                    txmidlelotnobarcode.Text = "";
                                    txmidleqty.Text = "0";
                                    txmidlebarcodemodel.Text = "";

                                    // Tăng CurrentMiddleSerial
                                    Global.SaveLastSerialToSetting("CurrentMiddleSerial", newmodel);

                                }
                                if (int.Parse(lblqty.Text) == 50)
                                {
                                    Task.Delay(100);
                                    string mtlotno = Global.GenerateMiddleLotno(middlevendorcode);
                                    string mtsku = txmidlesku.Text.Trim().ToUpper();
                                    string mtmodel = txmidlesku.Text.Substring(0, 9);
                                    long newmodel = long.Parse(Global.CurrentMiddleSerial) + 1;
                                    var modelconfig = Global.configmodel.Where(r => r.Model == txmidlesku.Text).FirstOrDefault();
                                    string mtitem = modelconfig.Item;
                                    string mtbarcodemodel = txmidlesku.Text + newmodel.ToString("D3");
                                    string mtbarcodeean = modelconfig.UpcCode;
                                    string mtorigin = txmidleorigin.Text;

                                    var mtserialNumbers = dgvsn.Rows
                                  .Cast<DataGridViewRow>()
                                  .Where(row => !row.IsNewRow)
                                  .Select(row => row.Cells[0].Value?.ToString())
                                  .Where(value => !string.IsNullOrEmpty(value))
                                  .ToList();
                                    if (serialNumbers.Count() <= 0)
                                    {
                                        MessageBox.Show("Please input SN!");
                                        return;
                                    }
                                    int qty = int.Parse(lblqty.Text);

                                    string mtserialNumbersString = string.Join(",", mtserialNumbers);
                                    string mtmatrixdata = mtsku + "," + mtlotno + "," + qty.ToString("D3") + "," + mtserialNumbersString;
                                    string mtbarcodelotno = mtsku + mtlotno + " " + qty.ToString("D3") + middlevendorcode;
                                    MASTERDATA masterdata = new MASTERDATA
                                    {
                                        EAN_UPC = mtbarcodeean,
                                        SKU = mtsku,
                                        Item = mtitem,
                                        BarcodeLotno = mtbarcodelotno,
                                        MODEL = mtmodel,
                                        LOTNO = mtlotno,
                                        BarcodeMODEL = mtbarcodemodel,
                                        QTY = qty.ToString() + " PCS",
                                        ORIGIN = mtorigin,
                                        Matrixdata = mtmatrixdata,
                                    };
                                    string mtprintername = Global.MasterPrinter;
                                    string mtprintercartonname = Global.MasterPrinterCarton;

                                    DialogResult confirmResult = MessageBox.Show(
                                       "Are you sure you want to print the Master box label?",
                                       "Confirm Print",
                                       MessageBoxButtons.YesNo,
                                       MessageBoxIcon.Question
                                    );

                                    if (confirmResult == DialogResult.Yes)
                                    {
                                        //if (!chbUseCartonID.Checked)
                                        //{
                                        //    printer.PrintMasterBoxLabel(mtprintername, masterdata);
                                        //}
                                        //else
                                        //{
                                        //    
                                        //}
                                        printer.PrintMasterBoxLabel(mtprintername, masterdata);
                                        printer.PrintMasterBoxLabel2(txCartonID.Text, mtprintercartonname, dateMasterBox.Value.ToString("MM/dd/yy"));
                                        dgvsn.Rows.Clear();
                                        lblqty.Text = "0";
                                        middleboxqty = 0;
                                        txmidlelotno.Text = "";
                                        txmidlelotnobarcode.Text = "";
                                        txmidleqty.Text = "0";
                                        txmidlebarcodemodel.Text = "";

                                    }



                                }
                            }





                        }
                        if (dgvsn.Rows.Count > 1 && !string.IsNullOrWhiteSpace(sn))
                        {

                            string lotno = Global.GenerateMiddleLotno(middlevendorcode);
                            Action updateLot = () =>
                            {
                                txmidlelotno.Text = lotno;
                            };

                            if (this.InvokeRequired)
                                this.Invoke(updateLot);
                            else
                                updateLot();

                            if (!string.IsNullOrWhiteSpace(txmidlesku.Text) && txmidlesku.Text.Length >= 9)
                            {
                                long newmodel = long.Parse(Global.CurrentMiddleSerial) + 1;
                                string model = txmidlesku.Text.Substring(0, 9);
                                string barcodemodel = txmidlesku.Text + newmodel.ToString("D3");

                                Action updateModel = () =>
                                {
                                    txmidlemodel.Text = model;
                                    txmidlebarcodemodel.Text = barcodemodel;
                                };

                                if (this.InvokeRequired)
                                    this.Invoke(updateModel);
                                else
                                    updateModel();
                            }


                        }
                        e.Handled = true;
                    }

                }
            }
            catch (Exception ex)
            {
                Global.WriteLog($"txmiddlesn_KeyPress failed: {ex.Message}");
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txmidlebarcodeean_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (Char)Keys.Enter)
                {
                    if (!string.IsNullOrWhiteSpace(txmidlebarcodeean.Text.Trim()))
                    {
                        txmidlesku.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Global.WriteLog("txmidlebarcodeean_KeyPress fail:" + ex.Message);
            }
        }

        private void txmiddlevendorcode_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (Char)Keys.Enter)
                {
                    if (!string.IsNullOrWhiteSpace(txmiddlevendorcode.Text))
                    {
                        middlevendorcode = txmiddlevendorcode.Text.ToUpper();
                        txmiddlevendorcode.Text = txmiddlevendorcode.Text.ToUpper();

                        if (string.IsNullOrWhiteSpace(middlevendorcode))
                        {
                            MessageBox.Show("Please input Vendor Code!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        if (middlevendorcode.Length != 2)
                        {
                            MessageBox.Show("Vendor code must be 2 characters!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        string lotno = Global.GenerateMiddleLotno(middlevendorcode);
                        Action UpdatLot = () =>
                        {
                            txmidlelotno.Text = lotno;
                        };

                        if (this.InvokeRequired)
                            this.Invoke(UpdatLot);
                        else
                            UpdatLot();
                        txmidlesku.Focus();

                    }



                }
            }
            catch (Exception ex)
            {
                Global.WriteLog("txmiddlevendorcode_KeyPress fail:" + ex.Message);
            }



        }

        private void btdelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvsn.SelectedRows.Count > 0)
                {
                    foreach (DataGridViewRow row in dgvsn.SelectedRows)
                    {
                        if (!row.IsNewRow)
                        {
                            dgvsn.Rows.Remove(row);
                            middleboxqty--;
                        }
                    }
                    var serialNumbers = dgvsn.Rows
                        .Cast<DataGridViewRow>()
                        .Where(row => !row.IsNewRow)
                        .Select(row => row.Cells[0].Value?.ToString())
                        .Where(value => !string.IsNullOrEmpty(value))
                        .ToList();
                    txmidleqty.Text = serialNumbers.Count().ToString();
                    lblqty.Text = middleboxqty.ToString();
                    txmiddlesn.Focus();
                }
                else
                {
                    MessageBox.Show("Please select row!", "Alarm", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Global.WriteLog($"delete eror : {ex.Message}");
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btndeleteall_Click(object sender, EventArgs e)
        {
            try
            {
                dgvsn.Rows.Clear();
                txmidleqty.Text = "0";
                lblqty.Text = "0";
                middleboxqty = 0;
                txmiddlesn.Focus();
            }
            catch (Exception ex)
            {
                Global.WriteLog($"delete eror : {ex.Message}");
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txmidlesku_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    if (!string.IsNullOrWhiteSpace(txmidlesku.Text.Trim()))
                    {
                        string model = txmidlesku.Text.Substring(0, 9);

                        long newmodel = long.Parse(Global.CurrentMiddleSerial) + 1;


                        string barcodemodel = txmidlesku.Text + newmodel.ToString("D3");

                        var modelconfig = Global.configmodel.Where(r => r.Model == txmidlesku.Text).FirstOrDefault();

                        Action Updatmodel = () =>
                        {
                            txmidleitem.Text = modelconfig.Item;
                            txmidlebarcodeean.Text = modelconfig.UpcCode;
                            txmidlemodel.Text = model;
                            txmidlebarcodemodel.Text = barcodemodel;
                        };

                        if (this.InvokeRequired)
                            this.Invoke(Updatmodel);
                        else
                            Updatmodel();
                        txmiddlesn.Focus();
                    }


                }
            }
            catch (Exception ex)
            {
                Global.WriteLog($"txmiddlesn_KeyPress failed: {ex.Message}");
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void btprintmasterbox_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txmasteritem.Text))
                {
                    MessageBox.Show("Item is not null");
                    return;
                }
                if (string.IsNullOrWhiteSpace(txmastersku.Text))
                {
                    MessageBox.Show("SKU CODE is not null");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txmasterbarcodeean.Text))
                {
                    MessageBox.Show("UPC/EAN CODE is not null");
                    return;
                }
                string origin;
                if (string.IsNullOrWhiteSpace(txmasterorigin.Text))
                {
                    origin = "MADE IN VIETNAM";
                }
                else
                {
                    origin = txmasterorigin.Text.Trim();
                }

                mastervendorcode = txmtvendercode.Text;

                if (string.IsNullOrWhiteSpace(mastervendorcode))
                {
                    MessageBox.Show("Please input Vendor Code!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (mastervendorcode.Length != 2)
                {
                    MessageBox.Show("Vendor code must be 2 characters!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txmasterlotno.Text))
                {
                    MessageBox.Show("Please input Lotno!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!int.TryParse(txmasterqty.Text, out int qty))
                {
                    MessageBox.Show("Quantity must be a valid integer!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txmastermodel.Text))
                {
                    MessageBox.Show("Please input Sku!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txmasterbarcodemodel.Text))
                {
                    MessageBox.Show("Please input Sku!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txmasterlotno.Text))
                {
                    MessageBox.Show("Please input SN!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string lotno = txmasterlotno.Text;

                if (comboBoxPrinters.SelectedItem == null)
                {
                    MessageBox.Show("Please select a printer (e.g., Zebra ZT411).");
                    return;
                }
                if (cbxMasterPrinterCartonName.SelectedItem == null)
                {
                    MessageBox.Show("Please select a printer (e.g., Zebra ZT411).");
                    return;
                }
                string masterqty = txmasterqty.Text /*+ " PCS"*/;
                string mtbarcodelotno = txmastersku.Text + lotno + " " + qty.ToString("D3") + mastervendorcode;

                Action mtUpdatbarcodelotno = () =>
                {

                    txmasterbarcode.Text = mtbarcodelotno;
                };

                if (this.InvokeRequired)
                    this.Invoke(mtUpdatbarcodelotno);
                else
                    mtUpdatbarcodelotno();

                string packingdate = null;

                if (string.IsNullOrWhiteSpace(txCartonID.Text))
                {
                    MessageBox.Show("CARTON ID is not null");
                    return;
                }

                if (string.IsNullOrWhiteSpace(dateMasterBox.Value.ToString()))
                {
                    packingdate = DateTime.Now.ToString("MM/dd/yy");
                }
                else
                {
                    if (DateTime.TryParse(dateMasterBox.Value.ToString(), out DateTime parsedDate))
                    {
                        packingdate = parsedDate.ToString("MM/dd/yy");
                    }
                    else
                    {
                        packingdate = DateTime.Now.ToString("MM/dd/yy");
                    }
                }


                var serialNumbers = dgvmastersn.Rows
              .Cast<DataGridViewRow>()
              .Where(row => !row.IsNewRow)
              .Select(row => row.Cells[0].Value?.ToString())
              .Where(value => !string.IsNullOrEmpty(value))
              .ToList();
                if (serialNumbers.Count() <= 0)
                {
                    MessageBox.Show("Please input SN!");
                    return;
                }




                string serialNumbersString = string.Join(",", serialNumbers);
                string matrixdata = txmastersku.Text + "," + lotno + "," + qty.ToString("D3") + "," + serialNumbersString;

                MASTERDATA masterdata = new MASTERDATA
                {
                    EAN_UPC = txmasterbarcodeean.Text,
                    SKU = txmastersku.Text,
                    Item = txmasteritem.Text,
                    BarcodeLotno = mtbarcodelotno,
                    MODEL = txmastermodel.Text,
                    LOTNO = lotno,
                    BarcodeMODEL = txmasterbarcodemodel.Text,
                    QTY = masterqty,
                    ORIGIN = origin,
                    Matrixdata = matrixdata,
                };
                string printername = comboBoxPrinters.SelectedItem.ToString();
                string printerCartonname = cbxMasterPrinterCartonName.SelectedItem.ToString();


                DialogResult confirmResult = MessageBox.Show(
                   "Are you sure you want to print the Master box label?",
                   "Confirm Print",
                   MessageBoxButtons.YesNo,
                   MessageBoxIcon.Question
               );

                if (confirmResult == DialogResult.Yes)
                {

                    printer.PrintMasterBoxLabel(printername, masterdata);

                    printer.PrintMasterBoxLabel2(txCartonID.Text, printerCartonname, packingdate);

                    dgvmastersn.Rows.Clear();
                    txmasterlotno.Text = "";
                    txmasterbarcode.Text = "";
                    txmasterbarcodemodel.Text = "";
                    MessageBox.Show($"Send Print comand to Printer successfully!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing master box label: {ex.Message}");
            }
        }

        private void txmastersku_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    if (!string.IsNullOrWhiteSpace(txmastersku.Text.Trim()))
                    {
                        string model = txmastersku.Text.Substring(0, 9);

                        long newmodel = long.Parse(Global.CurrentMiddleSerial) + 1;

                        var modelconfig = Global.configmodel.Where(r => r.Model == txmastersku.Text).FirstOrDefault();
                        string barcodemodel = txmastersku.Text + newmodel.ToString("D3");
                        Action Updatmodel = () =>
                        {
                            txmasteritem.Text = modelconfig.Item;
                            txmasterbarcodeean.Text = modelconfig.UpcCode;
                            txmastermodel.Text = model;
                            txmasterbarcodemodel.Text = barcodemodel;
                        };

                        if (this.InvokeRequired)
                            this.Invoke(Updatmodel);
                        else
                            Updatmodel();
                        txmasteritem.Focus();








                    }
                }
            }
            catch (Exception ex)
            {
                Global.WriteLog($"txmiddlesn_KeyPress failed: {ex.Message}");
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void txmtsn_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    if (!string.IsNullOrWhiteSpace(txmtsn.Text.Trim()))
                    {
                        if (txmtsn.Text.Length < Global.SNlen)
                        {
                            MessageBox.Show($"Serial number not correct :!{txmtsn.Text}", "SN Format", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txmtsn.Clear();
                            return;
                        }
                        mastervendorcode = txmtvendercode.Text;
                        if (string.IsNullOrWhiteSpace(mastervendorcode))
                        {
                            MessageBox.Show("Please input Vendor Code!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        if (mastervendorcode.Length != 2)
                        {
                            MessageBox.Show("Vendor code must be 2 characters!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        if (dgvmastersn.Rows.Count == 1)
                        {

                            long newmodel = long.Parse(Global.CurrentMiddleSerial) + 1;


                            string barcodemodel = txmastersku.Text + newmodel.ToString("D3");

                            string lotno = Global.GenerateMiddleLotno(mastervendorcode);

                            //chuỗi cartonID fix cứng 04 + lotno.Substring(2)
                            string cartonId = "04" + lotno.Substring(2);
                            Action UpdatLot = () =>
                            {

                                txmasterlotno.Text = lotno;
                                txmasterbarcodemodel.Text = barcodemodel;
                                txCartonID.Text = cartonId;
                            };

                            if (this.InvokeRequired)
                                this.Invoke(UpdatLot);
                            else
                                UpdatLot();
                        }
                        AddSNsFromInput(txmtsn.Text);


                        //dgvmastersn.Rows.Insert(0, new object[] { txmtsn.Text });
                        //txmtsn.Clear();
                        //var serialNumbers = dgvmastersn.Rows
                        // .Cast<DataGridViewRow>()
                        // .Where(row => !row.IsNewRow)
                        // .Select(row => row.Cells[0].Value?.ToString())
                        // .Where(value => !string.IsNullOrEmpty(value))
                        // .ToList();
                        //txmasterqty.Text = serialNumbers.Count().ToString();

                    }

                }
            }
            catch (Exception ex)
            {
                Global.WriteLog($"txmiddlesn_KeyPress failed: {ex.Message}");
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void RemoveSNsByStringFromGrid(DataGridView dgv, string rawData)
        {
            if (string.IsNullOrWhiteSpace(rawData))
                return;

            string[] parts = rawData.Split(',');

            if (parts.Length <= 3)
                return;

            List<string> removed = new List<string>();
            List<string> notFound = new List<string>();

            for (int i = 3; i < parts.Length; i++)
            {
                string sn = parts[i].Trim();
                bool found = false;

                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (!row.IsNewRow && row.Cells[0].Value?.ToString() == sn)
                    {
                        dgv.Rows.Remove(row);
                        removed.Add(sn);
                        found = true;
                        break;
                    }
                }

                if (!found)
                    notFound.Add(sn);
            }

            if (removed.Any())
            {
                MessageBox.Show("Đã xóa SN:\n" + string.Join("\n", removed),
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (notFound.Any())
            {
                MessageBox.Show("Không tìm thấy SN để xóa:\n" + string.Join("\n", notFound),
                                "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // Cập nhật lại số lượng
            txmasterqty.Text = dgv.Rows
                .Cast<DataGridViewRow>()
                .Where(r => !r.IsNewRow && r.Cells[0].Value != null)
                .Count()
                .ToString();
        }
        private void btnmtdelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txmtsn.Text))
                {
                    RemoveSNsByStringFromGrid(dgvmastersn, txmtsn.Text);
                }
                else if (dgvmastersn.SelectedRows.Count > 0)
                {
                    foreach (DataGridViewRow row in dgvmastersn.SelectedRows)
                    {
                        if (!row.IsNewRow)
                            dgvmastersn.Rows.Remove(row);
                    }

                    var serialNumbers = dgvmastersn.Rows
                        .Cast<DataGridViewRow>()
                        .Where(row => !row.IsNewRow)
                        .Select(row => row.Cells[0].Value?.ToString())
                        .Where(value => !string.IsNullOrEmpty(value))
                        .ToList();
                    txmasterqty.Text = serialNumbers.Count.ToString();
                    txmtsn.Focus();
                }
                else
                {
                    MessageBox.Show("Please select row or enter SN list!", "Alarm", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Global.WriteLog($"delete error: {ex.Message}");
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void btnmtdeleteall_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txmtsn.Text))
                {
                    RemoveSNsByStringFromGrid(dgvmastersn, txmtsn.Text);
                    txmtsn.Focus();
                }
                else
                {
                    dgvmastersn.Rows.Clear();
                    txmasterqty.Text = "0";
                }
            }
            catch (Exception ex)
            {
                Global.WriteLog($"delete error: {ex.Message}");
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tabmain_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabmain.SelectedIndex)
            {
                case 0:
                    txvendor.Focus();
                    break;
                case 1:
                    txmiddlevendorcode.Focus();
                    break;
                case 2:
                    txmtvendercode.Focus();
                    break;
            }
        }

        private void txmasteritem_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (Char)Keys.Enter)
                {
                    if (!string.IsNullOrWhiteSpace(txmasteritem.Text.Trim()))
                    {
                        txmtsn.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Global.WriteLog("txmasteritem_KeyPress fail:" + ex.Message);
            }
        }

        private void txmtvendercode_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (Char)Keys.Enter)
                {
                    if (!string.IsNullOrWhiteSpace(txmtvendercode.Text))
                    {

                        mastervendorcode = txmtvendercode.Text.ToUpper();
                        if (string.IsNullOrWhiteSpace(mastervendorcode))
                        {
                            MessageBox.Show("Please input Vendor Code!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        if (mastervendorcode.Length != 2)
                        {
                            MessageBox.Show("Vendor code must be 2 characters!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        string lotno = Global.GenerateMiddleLotno(mastervendorcode);

                        //chuỗi cartonID fix cứng 04 + lotno.Substring(2)
                        string cartonId = "04" + lotno.Substring(2);
                        Action UpdatLot = () =>
                        {
                            txmasterlotno.Text = lotno;
                            txCartonID.Text = cartonId;
                        };

                        if (this.InvokeRequired)
                            this.Invoke(UpdatLot);
                        else
                            UpdatLot();
                        txmastersku.Focus();

                    }
                }
            }
            catch (Exception ex)
            {
                Global.WriteLog("txmtvendercode_KeyPress KeyPress fail:" + ex.Message);
            }

        }

        private void txmasterbarcodeean_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (Char)Keys.Enter)
                {
                    if (!string.IsNullOrWhiteSpace(txmasterbarcodeean.Text))
                    {

                        txmastersku.Focus();

                    }
                }
            }
            catch (Exception ex)
            {
                Global.WriteLog("txmasterbarcodeean_KeyPress fail:" + ex.Message);
            }
        }

        private void AddSNsFromInput(string rawData)
        {
            if (string.IsNullOrWhiteSpace(rawData))
                return;

            string[] parts = rawData.Split(',');

            if (parts.Length <= 3)
                return;

            HashSet<string> snToAdd = new HashSet<string>(); // Tránh trùng trong list
            List<string> duplicatedSNs = new List<string>();  // Danh sách SN trùng

            for (int i = 3; i < parts.Length; i++) // Bắt đầu từ phần tử thứ 4
            {
                string sn = parts[i].Trim();

                // Kiểm tra trùng trong danh sách và trong dgv
                if (snToAdd.Contains(sn) || IsMasterDuplicate(sn))
                {
                    duplicatedSNs.Add(sn);
                }
                else
                {
                    snToAdd.Add(sn); // Cho vào danh sách hợp lệ
                    dgvmastersn.Rows.Add(sn);
                }
            }

            if (duplicatedSNs.Count > 0)
            {
                MessageBox.Show("Các SN bị trùng đã bị bỏ qua:\n" + string.Join("\n", duplicatedSNs),
                                "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // Cập nhật lại số lượng nếu cần
            txmasterqty.Text = dgvmastersn.Rows
                .Cast<DataGridViewRow>()
                .Where(row => !row.IsNewRow && row.Cells[0].Value != null)
                .Count()
                .ToString();
            txmtsn.Text = "";
        }
        private bool IsMasterDuplicate(string sn)
        {
            foreach (DataGridViewRow row in dgvmastersn.Rows)
            {
                if (!row.IsNewRow && row.Cells[0].Value?.ToString() == sn)
                {
                    return true;
                }
            }
            return false;
        }

        private void cmbModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            var model = cmbModel.SelectedItem;
            var modelconfig = Global.configmodel.Where(r => r.Model == model).FirstOrDefault();
            txtunititemmodel.Text = modelconfig.Item.ToString();
            txtunitearncode.Text = modelconfig.UpcCode.ToString();
            txunitcolor.Text = modelconfig.Color.ToString();
            txtunitskucode.Text = modelconfig.Model.ToString();


        }

        private void btnreprintunit_Click(object sender, EventArgs e)
        {
            string sn = "";
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
            if (string.IsNullOrWhiteSpace(txtunitsn1.Text))
            {
                MessageBox.Show("Please Input SN need Reprint!!!");
                return;
            }

            unititemmodel = txtunititemmodel.Text.Trim();

            string unitmanufacturedate;
            if (string.IsNullOrWhiteSpace(datepick.Value.ToString()))
            {
                unitmanufacturedate = DateTime.Now.ToString("yyyy.MM.dd");
            }
            else
            {
                if (DateTime.TryParse(datepick.Value.ToString(), out DateTime parsedDate))
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

            string printername = comboBoxPrinters.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(printername))
            {
                MessageBox.Show("Please select a printer (e.g., Zebra ZT411).");
                return;
            }


            string color = txunitcolor.Text;

            sn = txtunitsn1.Text;

            UNITDATA unitdata = new UNITDATA
            {
                EAN_UPC = unitean_upc,
                SKU = unitsku,
                ITEM_MODEL = unititemmodel,
                MANUFACTURE_DATE = unitmanufacturedate,
                ORIGIN = origin,
                SN = sn,
                COLOR = color,
            };

            bool result = RePrintAndSaveLabel(unitdata, printername);
            if (result)
            {
                MessageBox.Show($"Printed successfully.");

            }
            else
            {
                MessageBox.Show("Failed to print any labels.");
            }

        }



        //public void LoadDataToGridView(DataGridView dataGridView, string sn = null, string lot = null, DateTime? dateFrom = null, DateTime? dateTo = null)
        //{
        //    try
        //    {
        //        // Gọi phương thức SearchData để lấy dữ liệu
        //        DataSet ds = SearchData(sn, lot, dateFrom, dateTo);

        //        // Kiểm tra xem DataSet có dữ liệu hay không
        //        if (ds != null && ds.Tables.Count > 0)
        //        {
        //            // Gán DataTable đầu tiên trong DataSet vào DataSource của DataGridView
        //            dataGridView.DataSource = ds.Tables[0];

        //            // Tùy chỉnh hiển thị cột (tùy chọn)
        //            dataGridView.Columns["EARN"].HeaderText = "Earn";
        //            dataGridView.Columns["SKU"].HeaderText = "SKU";
        //            dataGridView.Columns["LOTNO"].HeaderText = "Lot Number";
        //            dataGridView.Columns["QTY"].HeaderText = "Quantity";
        //            dataGridView.Columns["SN"].HeaderText = "Serial Number";
        //            dataGridView.Columns["UserID"].HeaderText = "User ID";
        //            dataGridView.Columns["TIME"].HeaderText = "Time";

        //            // Định dạng cột TIME (nếu cần)
        //            dataGridView.Columns["TIME"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";

        //            // Tùy chỉnh kích thước cột (tự động điều chỉnh)
        //            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        //        }
        //        else
        //        {
        //            // Nếu không có dữ liệu, xóa DataSource và thông báo
        //            dataGridView.DataSource = null;
        //            MessageBox.Show("Không tìm thấy dữ liệu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}




    }
}