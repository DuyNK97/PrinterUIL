using Printer.Class;
using Printer.Class.Model;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Printer
{
    public class LabelPrinter
    {
        public string PrintUnitBoxLabel(UNITDATA data,string printerName)
        {
            string origin;
            string comma = "^FO288,124^GFA,00256,00256,00004,:Z64:eJxjYKAnOADEDxgYGD8AMZBmBvKZgULsQMxEV4fAAQBLYQRC:3CC2";
            string sku;
            string model;
            switch (data.SKU)
            {
                case "ET-SLL50LWEGUJ":
                case "ET-SNL33LWEGUJ":
                    model = $@"^FT30,130^A0N,16,16^FH\\^FD{data.ITEM_MODEL}({data.SKU.Substring(0,9)}) COLOR : WHITE^FS";
                    break;
                case "ET-SLL50LNEGUJ":
                case "ET-SNL33LNEGUJ":
                    model = $@"^FT30,130^A0N,16,16^FH\\^FD{data.ITEM_MODEL}({data.SKU.Substring(0, 9)}) COLOR : NAVY^FS";
                    break;
                case "ET-SLL50LJEGUJ":
                    model = $@"^FT30,130^A0N,16,16^FH\\^FD{data.ITEM_MODEL}({data.SKU.Substring(0, 9)}) COLOR : TAUPE^FS";
                    break;
                case "ET-SLL50LBEGUJ":
                    model = $@"^FT30,130^A0N,16,16^FH\\^FD{data.ITEM_MODEL}({data.SKU.Substring(0, 9)}) COLOR : BLACK^FS";
                    break;
                case "ET-SLL50LAEGUJ":
                    model = $@"^FT30,130^A0N,16,16^FH\\^FD{data.ITEM_MODEL}({data.SKU.Substring(0, 9)}) COLOR : CARMEL^FS";
                    break;
                case "ET-SNL33LPEGUJ":
                    model = $@"^FT30,130^A0N,16,16^FH\\^FD{data.ITEM_MODEL}({data.SKU.Substring(0, 9)}) COLOR : PINK^FS";
                    break;
                case "ET-SNL33LMEGUJ":
                    model = $@"^FT30,130^A0N,16,16^FH\\^FD{data.ITEM_MODEL}({data.SKU.Substring(0, 9)}) COLOR : MINT^FS";
                    break;
                case "ET-SNL33LBEGUJ":
                    model = $@"^FT30,130^A0N,16,16^FH\\^FD{data.ITEM_MODEL}({data.SKU.Substring(0, 9)}) COLOR : DARK GRAY^FS";
                    break;            
                default:
                    model = $@"^FT30,130^A0N,16,16^FH\\^FD{data.ITEM_MODEL}({data.SKU.Substring(0, 9)})^FS";
                    break;
            }

            if (data.ORIGIN.Trim() == "MADE IN VIETNAM / FABRIQUE AU VIETNAM")
            {
                origin = $" ^FT30,180^A0N,25,22^FH\\^FD{data.ORIGIN}^FS"+comma;
            }
            else 
            {
                origin = $" ^FT30,180^A0N,25,22^FH\\^FD{data.ORIGIN}^FS";
            }
            if (data.SKU.Length >= 13)
            {
                sku = "^BY3,2,59^FT0100,60^BEN,,Y,N";
            }
            else 
            {
                sku = "^BY3,2,59^FT0100,60^BUN,,Y,N";
            }


            string zplCommand = $@"^XA^PON^LH0,0^LL0444                               
                                {sku}^FD{data.EAN_UPC}^FS
                                ^FT120,110^A0N,24,24^FH\\^FD{data.SKU}^FS                                
                                ^FT30,150^A0N,12,12^FH\\^FDMANUFACTURE DATE: {data.MANUFACTURE_DATE}^FS    
                                {model} {origin}  
                                ^FT30,180^A0N,25,22^FH\\^FD{data.ORIGIN}^FS
                                ^BY2,3,24^FT30,210^BCN,,Y,N^FD>:{data.SN}^FS
                                ^FT85,230^A0N,20,25^FH^FDS/N:^FS
                                ^FT450,230^A0N,18,20^FH^FDA^FS
                                ^BY2,3,24^FT30,260^BCN,,Y,N^FD>:{data.SN}^FS
                                ^FT85,280^A0N,20,25^FH^FDS/N:^FS
                                ^FT450,280^A0N,18,20^FH^FDB^FS
                                ^BY2,3,24^FT30,310^BCN,,Y,N^FD>:{data.SN}^FS
                                ^FT85,330^A0N,20,25^FH^FDS/N:^FS
                                ^FT450,330^A0N,18,20^FH^FDC^FS
                                ^PQ1,0,1,Y^XZ";


            if (!RawPrinterHelper.SendStringToPrinter(printerName, zplCommand))
                throw new Exception("Failed to send Unit Box label to printer.");



            long serial = Global.FromBase33(data.SN.Substring(5, 5));
            
            Global.SaveLastSerialToSetting("CurrentUnitSerial", serial);
            Global.CreateExcelFile(Global.UnitExcelfoler, data);
            return data.SN;
        }

        public bool PrintUnitBoxLabelbool(UNITDATA data, string printerName)
        {
            string origin;
            string comma = "^FO268,113^GFA,00256,00256,00004,:Z64:eJxjYKAnOADEDxgYGD8AMZBmBvKZgULsQMxEV4fAAQBLYQRC:3CC2";
            string sku;
            string model;
            if (!string.IsNullOrWhiteSpace(data.COLOR))
            {
                model = $@"^FT10,130^A0N,16,16^FH\\^FD{data.ITEM_MODEL}({data.SKU.Substring(0, 9)}) COLOR : {data.COLOR}^FS";
            }
            else
            {
                model = $@"^FT10,130^A0N,16,16^FH\\^FD{data.ITEM_MODEL}({data.SKU.Substring(0, 9)})^FS";
            }            

            if (data.ORIGIN.Trim() == "MADE IN VIETNAM / FABRIQUE AU VIETNAM")
            {
                origin = $" ^FT10,170^A0N,25,22^FH\\^FD{data.ORIGIN}^FS" + comma;
            }
            else
            {
                origin = $" ^FT10,170^A0N,25,22^FH\\^FD{data.ORIGIN}^FS";
            }

            if (data.EAN_UPC.Length >= 13) //check upc >=13 se la ma uan-13
            {
                sku = "^BY3,2,59^FT080,60^BEN,,Y,N";
            }
            else
            {
                sku = "^BY3,2,59^FT080,60^BUN,,Y,N";
            }

            //^FT30,170 ^ A0N,25,22 ^ FH\\^FD{ data.ORIGIN}
            //^FS
            string zplCommand = $@"^XA^PON^LH0,0^LL0444                               
                            {sku}^FD{data.EAN_UPC}^FS
                            ^FT100,110^A0N,24,24^FH\\^FD{data.SKU}^FS                                 
                            ^FT10,145^A0N,12,12^FH\\^FDMANUFACTURE DATE: {data.MANUFACTURE_DATE}^FS    
                            {model} {origin}  
                            ^BY2,3,24^FT10,200^BCN,,Y,N^FD>:{data.SN}^FS
                            ^FT65,220^A0N,20,25^FH^FDS/N:^FS
                            ^FT430,220^A0N,18,20^FH^FDA^FS
                            ^BY2,3,24^FT10,259^BCN,,Y,N^FD>:{data.SN}^FS
                            ^FT65,277^A0N,20,25^FH^FDS/N:^FS
                            ^FT430,278^A0N,18,20^FH^FDB^FS
                            ^BY2,3,24^FT10,316^BCN,,Y,N^FD>:{data.SN}^FS
                            ^FT65,336^A0N,20,25^FH^FDS/N:^FS
                            ^FT430,336^A0N,18,20^FH^FDC^FS
                            ^PQ1,0,1,Y^XZ";

            if (!RawPrinterHelper.SendStringToPrinter(printerName, zplCommand))
            {
                return false; // Return false if printing failed
            }


            return true; // Return true if printing succeeded
        }


        public string PrintMiddleBoxLabel(string printerName, MIDDLECODE middledata)
        {
             
            string earncommand;
            if (middledata.EAN_UPC.Length >= 13)
            {
                earncommand = @"^FO100,100^BY3,2.0^BER,80,Y,N^FD"+ middledata.EAN_UPC + "^FS";
            }
            else
            {
                earncommand = @"^FO100,100^BY3,2.0^BUR,80,Y,N^FD" + middledata.EAN_UPC + "^FS";
            }
            


            string zplCommand = $@"^XA^PON^LH0,0
            ^FO570,60^BY2,1.0^BCR,80,N,N,N^FD{middledata.BarcodeLotno}^FS
            ^FO500,60^A0R,50,50^FD{middledata.Item}^FS
            ^FO450,60^A0R,50,50^FDMODEL : {middledata.MODEL}^FS
            ^FO400,60^A0R,50,50^FDSKU : {middledata.SKU}^FS
            ^FO350,60^A0R,50,50^FDLOT NO : {middledata.LOTNO}^FS
            ^FO300,60^A0R,50,50^FD{middledata.ORIGIN}^FS
            ^FO200,60^A0R,50,50^FDQ'TY : {middledata.QTY} PCS^FS
            ^FO200,380^BY2,1.0^BCR,80,N,N,N^FD{middledata.BarcodeMODEL}^FS
            {earncommand}
            ^FO300,1100^BY3.0^BXR,4,200^FD{middledata.Matrixdata}^FS
            ^FO20,900^GB1000,0,5^FS^XZ";



            //            string zplCommand = $@"^XA^PON^LH0,0
            //^FO570,60^BY2,1.0^BCR,80,N,Y,N^FDlotnobarcodeSG^FS
            //^FO500,60^A0R,50,50^FDItem^FS
            //^FO450,60^A0R,50,50^FDMODEL : modelcode^FS
            //^FO400,60^A0R,50,50^FDSKU : Sku^FS
            //^FO350,60^A0R,50,50^FDLOT NO : Lonot^FS
            //^FO300,60^A0R,50,50^FDMADE IN VIETNAM^FS
            //^FO200,60^A0R,50,50^FDQ'TY : 50 PCS^FS
            //^FO200,380^BY2,1.0^BCR,80,N,Y,N^FDmodelbarcod^FS
            //{earncommand}
            //^FO300,1100^BY3.0^BXR,4,200^FDmatrix^FS
            //^FO20,900^GB1000,0,5^FS^XZ";

            if (!RawPrinterHelper.SendStringToPrinter(printerName, zplCommand))
                throw new Exception("Failed to send Middle Box label to printer.");

            int middlecurr = int.Parse(middledata.LOTNO.Substring(middledata.LOTNO.Length - 4));

            Global.SaveLastSerialToSetting("CurrentMiddleSerial", middlecurr);
            Global.CurrentMiddleSerial = middlecurr.ToString();
            Global.CreateMiddleExcelFile(Global.MiddleExcelfoler, middledata);
            return middledata.LOTNO;

        }

        public string RePrintMiddleBoxLabel(string printerName, MIDDLECODE middledata)
        {

            string earncommand;
            if (middledata.EAN_UPC.Length >= 13)
            {
                earncommand = @"^FO100,100^BY3,2.0^BER,80,Y,N^FD" + middledata.EAN_UPC + "^FS";
            }
            else
            {
                earncommand = @"^FO100,100^BY3,2.0^BUR,80,Y,N^FD" + middledata.EAN_UPC + "^FS";
            }



            string zplCommand = $@"^XA^PON^LH0,0
            ^FO570,60^BY2,1.0^BCR,80,N,N,N^FD{middledata.BarcodeLotno}^FS
            ^FO500,60^A0R,50,50^FD{middledata.Item}^FS
            ^FO450,60^A0R,50,50^FDMODEL : {middledata.MODEL}^FS
            ^FO400,60^A0R,50,50^FDSKU : {middledata.SKU}^FS
            ^FO350,60^A0R,50,50^FDLOT NO : {middledata.LOTNO}^FS
            ^FO300,60^A0R,50,50^FD{middledata.ORIGIN}^FS
            ^FO200,60^A0R,50,50^FDQ'TY : {middledata.QTY} PCS^FS
            ^FO200,380^BY2,1.0^BCR,80,N,N,N^FD{middledata.BarcodeMODEL}^FS
            {earncommand}
            ^FO300,1100^BY3.0^BXR,4,200^FD{middledata.Matrixdata}^FS
            ^FO20,900^GB1000,0,5^FS^XZ";



            //            string zplCommand = $@"^XA^PON^LH0,0
            //^FO570,60^BY2,1.0^BCR,80,N,Y,N^FDlotnobarcodeSG^FS
            //^FO500,60^A0R,50,50^FDItem^FS
            //^FO450,60^A0R,50,50^FDMODEL : modelcode^FS
            //^FO400,60^A0R,50,50^FDSKU : Sku^FS
            //^FO350,60^A0R,50,50^FDLOT NO : Lonot^FS
            //^FO300,60^A0R,50,50^FDMADE IN VIETNAM^FS
            //^FO200,60^A0R,50,50^FDQ'TY : 50 PCS^FS
            //^FO200,380^BY2,1.0^BCR,80,N,Y,N^FDmodelbarcod^FS
            //{earncommand}
            //^FO300,1100^BY3.0^BXR,4,200^FDmatrix^FS
            //^FO20,900^GB1000,0,5^FS^XZ";

            if (!RawPrinterHelper.SendStringToPrinter(printerName, zplCommand))
                throw new Exception("Failed to send Middle Box label to printer.");

            Global.CreateMiddleExcelFile(Global.MiddleExcelfoler, middledata);
            return middledata.LOTNO;

        }

        public bool PrintMiddleBoxLabelbool(string printerName, MIDDLECODE middledata)
        {
            try
            {
                string earncommand;
                if (middledata.EAN_UPC.Length >= 13)
                {
                    earncommand = @"^FO100,100^BY3,2.0^BER,80,Y,N^FD" + middledata.EAN_UPC + "^FS";
                }
                else
                {
                    earncommand = @"^FO100,100^BY3,2.0^BUR,80,Y,N^FD" + middledata.EAN_UPC + "^FS";
                }

                string zplCommand = $@"^XA^PON^LH0,0
        ^FO570,60^BY2,1.0^BCR,80,N,N,N^FD{middledata.BarcodeLotno}^FS
        ^FO500,60^A0R,50,50^FD{middledata.Item}^FS
        ^FO450,60^A0R,50,50^FDMODEL : {middledata.MODEL}^FS
        ^FO400,60^A0R,50,50^FDSKU : {middledata.SKU}^FS
        ^FO350,60^A0R,50,50^FDLOT NO : {middledata.LOTNO}^FS
        ^FO300,60^A0R,50,50^FD{middledata.ORIGIN}^FS
        ^FO200,60^A0R,50,50^FDQ'TY : {middledata.QTY}^FS
        ^FO200,380^BY2,1.0^BCR,80,N,N,N^FD{middledata.BarcodeMODEL}^FS
        {earncommand}
        ^FO300,1100^BY3.0^BXR,4,200^FD{middledata.Matrixdata}^FS
        ^FO20,900^GB1000,0,5^FS^XZ";

                bool printResult = RawPrinterHelper.SendStringToPrinter(printerName, zplCommand);

                if (!printResult)
                    return false;  // Nếu không thành công, trả về false

                int middlecurr = int.Parse(middledata.LOTNO.Substring(middledata.LOTNO.Length - 4));

                Global.SaveLastSerialToSetting("CurrentMiddleSerial", middlecurr);
                Global.CurrentMiddleSerial = middlecurr.ToString();
                Global.CreateMiddleExcelFile(Global.MiddleExcelfoler, middledata);

                return true;  // Nếu tất cả các bước thành công, trả về true
            }
            catch (Exception)
            {
                return false;  // Nếu có lỗi, trả về false
            }
        }


        public string PrintMasterBoxLabel(string printerName, MASTERDATA masterdata)
        {

            string earncommand;
            if (masterdata.EAN_UPC.Length >= 13)
            {
                earncommand = @"^FO100,100^BY3,2.0^BER,80,Y,N^FD" + masterdata.EAN_UPC + "^FS";
            }
            else
            {
                earncommand = @"^FO100,100^BY3,2.0^BUR,80,Y,N^FD" + masterdata.EAN_UPC + "^FS";
            }



            string zplCommand = $@"
^XA^PON^FO750,60^BY2,1.0^BCR,130,N,N,N^FD:{masterdata.BarcodeLotno}^FS
^FO660,60^A0R,60,60^FD{masterdata.Item}^FS
^FO580,60^A0R,60,60^FDMODEL : {masterdata.MODEL}^FS
^FO510,60^A0R,60,60^FDSKU : {masterdata.SKU}^FS
^FO440,60^A0R,60,60^FDLOT NO : {masterdata.LOTNO}^FS
^FO370,60^A0R,60,60^FD{masterdata.ORIGIN}^FS
^FO270,60^A0R,60,60^FDQ'TY : {masterdata.QTY} PCS^FS
^FO260,440^BY2,1.0^BCR,100,N,N,N^FD{masterdata.QTY}^FS
{earncommand}
^FO150,1020^BY4.3^BXR,8,200^FD{masterdata.Matrixdata}^FS
^FO0,1000^GB1400,0,5^FS^XZ
";

            
            if (!RawPrinterHelper.SendStringToPrinter(printerName, zplCommand))
                throw new Exception("Failed to send Middle Box label to printer.");

            int middlecurr = int.Parse(masterdata.LOTNO.Substring(masterdata.LOTNO.Length - 4));

            Global.SaveLastSerialToSetting("CurrentMiddleSerial", middlecurr);
            Global.CurrentMiddleSerial = middlecurr.ToString();
            Global.CreateMasterExcelFile(Global.MasterExcelfoler, masterdata);
            return masterdata.LOTNO;
        }
        public bool PrintMasterBoxLabel2(string Cartonid, string printerName, string date = null)
        {

            if (string.IsNullOrWhiteSpace(date))
            {
                date = DateTime.Today.ToString("dd/MM/yy");
            }
            // trường hợp tem  dọc
            string zplCommand = $@"^XA^PON
^FO100,10^A0R,55,55^FDCARTON ID: {Cartonid}^FS
^FO60,550^BY1.5,1.0^BCR,100,N,N,N^FD{Cartonid}^FS
^FO30,10^A0R,55,55^FDDate :{date}^FS
^XZ";
            //XA^PON ^XZ"  đây là ký tự bắt buộc không xóa

            //^FO100,10^A0R,55,55^FDCARTON ID: {Cartonid}^FS =====^FO x,y  A0 : font chữ mặc định arial 55,55  điểm dot (dpi ) font 14 =(14/72)*300(dpi)=58 dot   N (Normal): 0° (hiện tại là mặc định). R (Rotated): 90° ngược chiều kim đồng hồ (mã vạch nằm dọc, các thanh song song với trục Y, đọc từ dưới lên trên). I (Inverted): 180° (mã vạch nằm ngang nhưng ngược hướng). B (Bottom): 270° ngược chiều kim đồng hồ (mã vạch nằm dọc, các thanh song song với trục Y, đọc từ trên xuống dưới).
            //link review label https://labelary.com/viewer.html

            //// trường hợp tem  ngang
//            string zplCommand = $@"^XA^PON
//^FO10,10^A0N,55,55^FDCARTON ID: {Cartonid}^FS
//^FO570,10^BY1.5,1.0^BCN,100,N,N,N^FD{Cartonid}^FS
//^FO10,70^A0N,55,55^FDDate : {date}^FS
//^XZ";



            if (!RawPrinterHelper.SendStringToPrinter(printerName, zplCommand))
            {
                return false;
            }
            Global.CreateCartonExcelFile(Global.CartonExcelfoler, Cartonid, date);
            return true; 
        }

        public bool PrintMasterBoxLabelbool(string printerName, MASTERDATA masterdata)
        {
            try
            {
                string earncommand;
                if (masterdata.EAN_UPC.Length >= 13)
                {
                    earncommand = @"^FO100,100^BY3,2.0^BER,80,Y,N^FD" + masterdata.EAN_UPC + "^FS";
                }
                else
                {
                    earncommand = @"^FO100,100^BY3,2.0^BUR,80,Y,N^FD" + masterdata.EAN_UPC + "^FS";
                }

                string zplCommand = $@"
        ^XA^PON^FO750,60^BY2,1.0^BCR,130,N,N,N^FD:{masterdata.BarcodeLotno}^FS
        ^FO660,60^A0R,60,60^FD{masterdata.Item}^FS
        ^FO580,60^A0R,60,60^FDMODEL : {masterdata.MODEL}^FS
        ^FO510,60^A0R,60,60^FDSKU : {masterdata.SKU}^FS
        ^FO440,60^A0R,60,60^FDLOT NO : {masterdata.LOTNO}^FS
        ^FO370,60^A0R,60,60^FD{masterdata.ORIGIN}^FS
        ^FO270,60^A0R,60,60^FDQ'TY : {masterdata.QTY}^FS
        ^FO260,440^BY2,1.0^BCR,100,N,N,N^FD{masterdata.BarcodeMODEL}^FS
        {earncommand}
        ^FO150,1020^BY4.3^BXR,8,200^FD{masterdata.Matrixdata}^FS
        ^FO0,1000^GB1400,0,5^FS^XZ";

                bool printResult = RawPrinterHelper.SendStringToPrinter(printerName, zplCommand);

                if (!printResult)
                    return false;  // Nếu không thành công, trả về false

                int middlecurr = int.Parse(masterdata.LOTNO.Substring(masterdata.LOTNO.Length - 4));

                Global.SaveLastSerialToSetting("CurrentMiddleSerial", middlecurr);
                Global.CurrentMiddleSerial = middlecurr.ToString();
                Global.CreateMasterExcelFile(Global.MasterExcelfoler, masterdata);

                return true;  // Nếu tất cả các bước thành công, trả về true
            }
            catch (Exception)
            {
                return false;  // Nếu có lỗi, trả về false
            }
        }


        public void PrintTestLabel(string printerName)
        {
            string zplCommand = "^XA^FO50,50^A0N,50,50^FDTEST PRINT^FS^XZ";
            if (!RawPrinterHelper.SendStringToPrinter(printerName, zplCommand))
                throw new Exception("Failed to send test label to printer.");
        }
    }

    public static class RawPrinterHelper
    {
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter(string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, int level, [In] ref DOCINFOA pDocInfo);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);

        public static bool SendStringToPrinter(string szPrinterName, string szString)
        {
            IntPtr pBytes;
            int dwCount = (szString.Length + 1) * Marshal.SizeOf(typeof(char));
            pBytes = Marshal.StringToCoTaskMemAnsi(szString);
            try
            {
                IntPtr hPrinter;
                DOCINFOA di = new DOCINFOA();
                di.pDocName = "Label Print Job";
                di.pOutputFile = null;
                di.pDataType = "RAW";

                if (!OpenPrinter(szPrinterName, out hPrinter, IntPtr.Zero))
                    return false;

                if (!StartDocPrinter(hPrinter, 1, ref di))
                {
                    ClosePrinter(hPrinter);
                    return false;
                }

                if (!StartPagePrinter(hPrinter))
                {
                    EndDocPrinter(hPrinter);
                    ClosePrinter(hPrinter);
                    return false;
                }

                int dwWritten;
                if (!WritePrinter(hPrinter, pBytes, dwCount, out dwWritten))
                {
                    EndPagePrinter(hPrinter);
                    EndDocPrinter(hPrinter);
                    ClosePrinter(hPrinter);
                    return false;
                }

                EndPagePrinter(hPrinter);
                EndDocPrinter(hPrinter);
                ClosePrinter(hPrinter);
                return true;
            }
            finally
            {
                Marshal.FreeCoTaskMem(pBytes);
            }
        }

        public struct DOCINFOA
        {
            public string pDocName;
            public string pOutputFile;
            public string pDataType;
        }
    }
}