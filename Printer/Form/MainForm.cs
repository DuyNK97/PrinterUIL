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


        public MainForm()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            InitializeComponent();
            printer = new LabelPrinter();
            _boxsn = new UnitBoxSN();
            InitializeRadioButtonEvents();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadPrinters();

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
                    ["MasterExcelfoler"] = @"D:\Printer\Master"

                };

                Global.WriteFileToTxt(settingPath, defaultValues);
            }
            Dictionary<string, string> currentData = Global.ReadValueFileTxt(Global.GetFilePathSetting(), new List<string> { "CurrentUnitSerial", "CurrentMiddleSerial", "CurrentMasterSerial", "UnitExcelfoler", "MiddleExcelfoler", "MasterExcelfoler" });

            Global.CurrentUnitSerial = currentData["CurrentUnitSerial"];
            Global.CurrentMiddleSerial = currentData["CurrentMiddleSerial"];
            Global.CurrentMasterSerial = currentData["CurrentMasterSerial"];
            Global.UnitExcelfoler = currentData["UnitExcelfoler"];
            Global.MiddleExcelfoler = currentData["MiddleExcelfoler"];
            Global.MasterExcelfoler = currentData["MasterExcelfoler"];
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

                    sn = Global.GenerateSerialNumber(
                        productGroup[0],
                        customer[0],
                        '7',
                        monthCode,
                        vendorcode,
                        ordertype[0]
                    );
                }
                Action UpdateSN = () =>
                {

                    txtunitsn1.Text = sn;
                };

                if (this.InvokeRequired)
                    this.Invoke(UpdateSN);
                else
                    UpdateSN();

                //else
                //{
                //    sn = txtunitsn1.Text.Trim();
                //}

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

                if (comboBoxPrinters.SelectedItem == null)
                {
                    MessageBox.Show("Please select a printer (e.g., Zebra ZT411).");
                    return;
                }


                string serialNumbersString = string.Join(",", serialNumbers);
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
                    QTY = middleqty,
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
                    dgvsn.Rows.Clear();
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


        private void txmidleitem_KeyPress(object sender, KeyPressEventArgs e)
        {

            try
            {
                if (e.KeyChar == (Char)Keys.Enter)
                {
                    if (!string.IsNullOrWhiteSpace(txmidleitem.Text.Trim()))
                    {
                        txtunitsn1.Focus();
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
                        txtunititemmodel.Focus();
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


        private void txmiddlesn_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    if (dgvsn.Rows.Count == 1)
                    {
                        string lotno = Global.GenerateMiddleLotno(middlevendorcode);
                        Action UpdatLot = () =>
                        {

                            txmidlelotno.Text = lotno;
                        };

                        if (this.InvokeRequired)
                            this.Invoke(UpdatLot);
                        else
                            UpdatLot();

                        if (string.IsNullOrWhiteSpace(txmidlesku.Text))
                        {
                            long newmodel = long.Parse(Global.CurrentMiddleSerial) + 1;
                            string model = txmidlesku.Text.Substring(0, 9);

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
                        }

                    }





                    if (!string.IsNullOrWhiteSpace(txmiddlesn.Text))
                    {
                        dgvsn.Rows.Insert(0, new object[] { txmiddlesn.Text });
                        txmiddlesn.Clear();
                        var serialNumbers = dgvsn.Rows
                         .Cast<DataGridViewRow>()
                         .Where(row => !row.IsNewRow)
                         .Select(row => row.Cells[0].Value?.ToString())
                         .Where(value => !string.IsNullOrEmpty(value))
                         .ToList();
                        txmidleqty.Text = serialNumbers.Count().ToString();
                    }
                    e.Handled = true;
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
                        middlevendorcode = txmiddlevendorcode.Text;
                        string lotno = Global.GenerateMiddleLotno(middlevendorcode);
                        Action UpdatLot = () =>
                        {
                            txmidlelotno.Text = lotno;
                        };

                        if (this.InvokeRequired)
                            this.Invoke(UpdatLot);
                        else
                            UpdatLot();
                        txmidlebarcodeean.Focus();

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
                        }
                    }
                    var serialNumbers = dgvsn.Rows
                        .Cast<DataGridViewRow>()
                        .Where(row => !row.IsNewRow)
                        .Select(row => row.Cells[0].Value?.ToString())
                        .Where(value => !string.IsNullOrEmpty(value))
                        .ToList();
                    txmidleqty.Text = serialNumbers.Count().ToString();
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
                        Action Updatmodel = () =>
                        {

                            txmidlemodel.Text = model;
                            txmidlebarcodemodel.Text = barcodemodel;
                        };

                        if (this.InvokeRequired)
                            this.Invoke(Updatmodel);
                        else
                            Updatmodel();
                        txmidleitem.Focus();
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
                string masterqty = txmasterqty.Text + " PCS";
                string mtbarcodelotno = txmastersku.Text + lotno + " " + qty.ToString("D3") + mastervendorcode;

                Action mtUpdatbarcodelotno = () =>
                {

                    txmasterbarcode.Text = mtbarcodelotno;
                };

                if (this.InvokeRequired)
                    this.Invoke(mtUpdatbarcodelotno);
                else
                    mtUpdatbarcodelotno();



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


                DialogResult confirmResult = MessageBox.Show(
                   "Are you sure you want to print the Master box label?",
                   "Confirm Print",
                   MessageBoxButtons.YesNo,
                   MessageBoxIcon.Question
               );

                if (confirmResult == DialogResult.Yes)
                {
                    printer.PrintMasterBoxLabel(printername, masterdata);
                    dgvmastersn.Rows.Clear();
                    txmasterlotno.Text = "";
                    txmasterbarcode.Text = "";
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


                        string barcodemodel = txmastersku.Text + newmodel.ToString("D3");
                        Action Updatmodel = () =>
                        {

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
                            string lotno = Global.GenerateMiddleLotno(mastervendorcode);
                            Action UpdatLot = () =>
                            {

                                txmasterlotno.Text = lotno;
                            };

                            if (this.InvokeRequired)
                                this.Invoke(UpdatLot);
                            else
                                UpdatLot();
                        }
                        if (!string.IsNullOrWhiteSpace(txmtsn.Text))
                        {
                            dgvmastersn.Rows.Insert(0, new object[] { txmtsn.Text });
                            txmtsn.Clear();
                            var serialNumbers = dgvmastersn.Rows
                             .Cast<DataGridViewRow>()
                             .Where(row => !row.IsNewRow)
                             .Select(row => row.Cells[0].Value?.ToString())
                             .Where(value => !string.IsNullOrEmpty(value))
                             .ToList();
                            txmasterqty.Text = serialNumbers.Count().ToString();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Global.WriteLog($"txmiddlesn_KeyPress failed: {ex.Message}");
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnmtdelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvmastersn.SelectedRows.Count > 0)
                {
                    foreach (DataGridViewRow row in dgvmastersn.SelectedRows)
                    {
                        if (!row.IsNewRow)
                        {
                            dgvmastersn.Rows.Remove(row);
                        }
                    }
                    var serialNumbers = dgvmastersn.Rows
                        .Cast<DataGridViewRow>()
                        .Where(row => !row.IsNewRow)
                        .Select(row => row.Cells[0].Value?.ToString())
                        .Where(value => !string.IsNullOrEmpty(value))
                        .ToList();
                    txmasterqty.Text = serialNumbers.Count().ToString();
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

        private void btnmtdeleteall_Click(object sender, EventArgs e)
        {
            try
            {
                dgvmastersn.Rows.Clear();

                txmasterqty.Text = "0";
            }
            catch (Exception ex)
            {
                Global.WriteLog($"delete eror : {ex.Message}");
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

                        string lotno = Global.GenerateMiddleLotno(mastervendorcode);
                        Action UpdatLot = () =>
                        {
                            txmasterlotno.Text = lotno;
                        };

                        if (this.InvokeRequired)
                            this.Invoke(UpdatLot);
                        else
                            UpdatLot();
                        txmasterbarcodeean.Focus();

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