namespace Printer
{
    partial class Form1
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
            this.comboBoxPrinters = new System.Windows.Forms.ComboBox();
            this.btPrint = new System.Windows.Forms.Button();
            this.txBarcode = new System.Windows.Forms.TextBox();
            this.txtModel = new System.Windows.Forms.TextBox();
            this.txtDate = new System.Windows.Forms.TextBox();
            this.txtBarcode2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btPrinttest = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.txtunitearncode = new System.Windows.Forms.TextBox();
            this.txtunitskucode = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtunititemmodel = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtunitmanufacturedate = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtunitorigin = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtunitsn1 = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtunitsn2 = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtunitsn3 = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxPrinters
            // 
            this.comboBoxPrinters.FormattingEnabled = true;
            this.comboBoxPrinters.Location = new System.Drawing.Point(408, 17);
            this.comboBoxPrinters.Name = "comboBoxPrinters";
            this.comboBoxPrinters.Size = new System.Drawing.Size(253, 21);
            this.comboBoxPrinters.TabIndex = 0;
            // 
            // btPrint
            // 
            this.btPrint.Location = new System.Drawing.Point(699, 17);
            this.btPrint.Name = "btPrint";
            this.btPrint.Size = new System.Drawing.Size(75, 23);
            this.btPrint.TabIndex = 1;
            this.btPrint.Text = "Print";
            this.btPrint.UseVisualStyleBackColor = true;
            this.btPrint.Click += new System.EventHandler(this.btPrint_Click);
            // 
            // txBarcode
            // 
            this.txBarcode.Location = new System.Drawing.Point(88, 16);
            this.txBarcode.Name = "txBarcode";
            this.txBarcode.Size = new System.Drawing.Size(253, 20);
            this.txBarcode.TabIndex = 2;
            // 
            // txtModel
            // 
            this.txtModel.Location = new System.Drawing.Point(88, 72);
            this.txtModel.Name = "txtModel";
            this.txtModel.Size = new System.Drawing.Size(253, 20);
            this.txtModel.TabIndex = 3;
            // 
            // txtDate
            // 
            this.txtDate.Location = new System.Drawing.Point(88, 129);
            this.txtDate.Name = "txtDate";
            this.txtDate.Size = new System.Drawing.Size(253, 20);
            this.txtDate.TabIndex = 4;
            // 
            // txtBarcode2
            // 
            this.txtBarcode2.Location = new System.Drawing.Point(88, 191);
            this.txtBarcode2.Name = "txtBarcode2";
            this.txtBarcode2.Size = new System.Drawing.Size(253, 20);
            this.txtBarcode2.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Barcode";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Model";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(31, 136);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(28, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "date";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(31, 198);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "barcode2";
            // 
            // btPrinttest
            // 
            this.btPrinttest.Location = new System.Drawing.Point(803, 17);
            this.btPrinttest.Name = "btPrinttest";
            this.btPrinttest.Size = new System.Drawing.Size(75, 23);
            this.btPrinttest.TabIndex = 10;
            this.btPrinttest.Text = "Print test";
            this.btPrinttest.UseVisualStyleBackColor = true;
            this.btPrinttest.Click += new System.EventHandler(this.btPrintTest_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtunitsn3);
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.txtunitsn2);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.txtunitsn1);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.txtunitorigin);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.txtunitmanufacturedate);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.txtunititemmodel);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.txtunitskucode);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.txtunitearncode);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Location = new System.Drawing.Point(295, 255);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(462, 257);
            this.panel1.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(19, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(129, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "[BAR CODE(EAN/UPC)] :";
            // 
            // txtunitearncode
            // 
            this.txtunitearncode.Location = new System.Drawing.Point(178, 24);
            this.txtunitearncode.Name = "txtunitearncode";
            this.txtunitearncode.Size = new System.Drawing.Size(253, 20);
            this.txtunitearncode.TabIndex = 8;
            // 
            // txtunitskucode
            // 
            this.txtunitskucode.Location = new System.Drawing.Point(178, 59);
            this.txtunitskucode.Name = "txtunitskucode";
            this.txtunitskucode.Size = new System.Drawing.Size(253, 20);
            this.txtunitskucode.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(30, 62);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(74, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "[SKU CODE] :";
            // 
            // txtunititemmodel
            // 
            this.txtunititemmodel.Location = new System.Drawing.Point(178, 85);
            this.txtunititemmodel.Name = "txtunititemmodel";
            this.txtunititemmodel.Size = new System.Drawing.Size(253, 20);
            this.txtunititemmodel.TabIndex = 12;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(30, 88);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(92, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "[ITEM (MODEL)] :";
            // 
            // txtunitmanufacturedate
            // 
            this.txtunitmanufacturedate.Location = new System.Drawing.Point(178, 111);
            this.txtunitmanufacturedate.Name = "txtunitmanufacturedate";
            this.txtunitmanufacturedate.Size = new System.Drawing.Size(253, 20);
            this.txtunitmanufacturedate.TabIndex = 14;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(30, 114);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(133, 13);
            this.label8.TabIndex = 13;
            this.label8.Text = "[MANUFACTURE DATE ]:";
            // 
            // txtunitorigin
            // 
            this.txtunitorigin.Location = new System.Drawing.Point(178, 139);
            this.txtunitorigin.Name = "txtunitorigin";
            this.txtunitorigin.Size = new System.Drawing.Size(253, 20);
            this.txtunitorigin.TabIndex = 16;
            this.txtunitorigin.Text = "MADE IN VIETNAM / FABRIQUÉAUVIETNAM";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(30, 142);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(57, 13);
            this.label9.TabIndex = 15;
            this.label9.Text = "[ORIGIN] :";
            // 
            // txtunitsn1
            // 
            this.txtunitsn1.Location = new System.Drawing.Point(178, 165);
            this.txtunitsn1.Name = "txtunitsn1";
            this.txtunitsn1.Size = new System.Drawing.Size(253, 20);
            this.txtunitsn1.TabIndex = 18;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(30, 165);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(48, 13);
            this.label10.TabIndex = 17;
            this.label10.Text = "[S/N] 1 :";
            // 
            // txtunitsn2
            // 
            this.txtunitsn2.Location = new System.Drawing.Point(178, 191);
            this.txtunitsn2.Name = "txtunitsn2";
            this.txtunitsn2.Size = new System.Drawing.Size(253, 20);
            this.txtunitsn2.TabIndex = 20;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(30, 191);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(48, 13);
            this.label11.TabIndex = 19;
            this.label11.Text = "[S/N] 2 :";
            // 
            // txtunitsn3
            // 
            this.txtunitsn3.Location = new System.Drawing.Point(178, 217);
            this.txtunitsn3.Name = "txtunitsn3";
            this.txtunitsn3.Size = new System.Drawing.Size(253, 20);
            this.txtunitsn3.TabIndex = 22;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(30, 217);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(48, 13);
            this.label12.TabIndex = 21;
            this.label12.Text = "[S/N] 3 :";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(955, 536);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btPrinttest);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtBarcode2);
            this.Controls.Add(this.txtDate);
            this.Controls.Add(this.txtModel);
            this.Controls.Add(this.txBarcode);
            this.Controls.Add(this.btPrint);
            this.Controls.Add(this.comboBoxPrinters);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxPrinters;
        private System.Windows.Forms.Button btPrint;
        private System.Windows.Forms.TextBox txBarcode;
        private System.Windows.Forms.TextBox txtModel;
        private System.Windows.Forms.TextBox txtDate;
        private System.Windows.Forms.TextBox txtBarcode2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btPrinttest;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtunitskucode;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtunitearncode;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtunititemmodel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtunitsn2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtunitsn1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtunitorigin;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtunitmanufacturedate;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtunitsn3;
        private System.Windows.Forms.Label label12;
    }
}

