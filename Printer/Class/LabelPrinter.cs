using Printer.Class.Model;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Printer
{
    public class LabelPrinter
    {
        private const string UnitSerialFile = "last_unit_serial.txt";
        private const string MiddleSerialFile = "last_middle_serial.txt";
        private const string MasterSerialFile = "last_master_serial.txt";
        private static readonly char[] Base33Chars = "0123456789ABCDEFGHJKLMNPQRSTVWXYZ".ToCharArray(); // Bỏ I, O, U
        private static readonly int Base = 33;
        private long currentUnitSerial;
        private long currentMiddleSerial;
        private long currentMasterSerial;

        public long CurrentUnitSerial => currentUnitSerial;
        public long CurrentMiddleSerial => currentMiddleSerial;
        public long CurrentMasterSerial => currentMasterSerial;

        public LabelPrinter()
        {
            LoadSerialNumbers();
        }

        private void LoadSerialNumbers()
        {
            currentUnitSerial = ReadLastSerial(UnitSerialFile);
            currentMiddleSerial = ReadLastSerial(MiddleSerialFile);
            currentMasterSerial = ReadLastSerial(MasterSerialFile);
        }

        private long ReadLastSerial(string filePath)
        {
            if (File.Exists(filePath))
            {
                string content = File.ReadAllText(filePath).Trim();
                if (long.TryParse(content, out long lastSerial) && lastSerial >= 0)
                    return lastSerial;
            }
            return 0;
        }

        private void SaveLastSerial(string filePath, long serial)
        {
            File.WriteAllText(filePath, serial.ToString());
        }

        private string ToBase33(long number, int length = 5)
        {
            long maxLimit = (long)Math.Pow(33, length) - 1; // ZZZZZ cho 5 chữ số, ZZZZ cho 4 chữ số
            if (number < 0 || number > maxLimit)
                throw new ArgumentException($"Số thứ tự vượt quá giới hạn cho phép ({maxLimit}).");

            char[] result = new char[length];
            for (int i = length - 1; i >= 0; i--)
            {
                result[i] = Base33Chars[number % Base];
                number /= Base;
            }
            return new string(result);
        }

        private string GenerateSerialNumber(
            char productGroup = 'R', // HHP
            char productionSite = '5', // SEVT
            char productType = '7', // APS
            char yearCode = 'Y', // 2025
            char monthCode = '5', // Tháng 5
            string vendorCode = "TY", // Nhà cung cấp
            char deliveryType = 'A', // Inbox
            long serialNumber = 0)
        {
            if (vendorCode.Length != 2 || !vendorCode.All(char.IsLetter))
                throw new ArgumentException("Mã nhà cung cấp phải là 2 chữ cái.");
            if (!"R1M".Contains(productGroup))
                throw new ArgumentException("Mã nhóm sản phẩm không hợp lệ.");
            if (!"VFZQRX356A".Contains(productionSite))
                throw new ArgumentException("Mã nơi sản xuất không hợp lệ.");
            if (productType != '7')
                throw new ArgumentException("Mã loại sản phẩm phải là 7 cho APS.");
            if (!"123456789ABC".Contains(monthCode))
                throw new ArgumentException("Mã tháng không hợp lệ.");
            if (!"AB".Contains(deliveryType))
                throw new ArgumentException("Loại giao hàng phải là A hoặc B.");

            string serialBase33 = ToBase33(serialNumber);
            return $"{productGroup}{productionSite}{productType}{yearCode}{monthCode}{serialBase33}X{vendorCode}{deliveryType}";
        }

        private string GenerateLotNumber(string vendorCode, char yearCode, char monthCode, long serial)
        {
            string lotSerial = ToBase33(serial, 4); // 4 chữ số cho Lot No
            return $"{vendorCode}{yearCode}{monthCode}01{lotSerial}";
        }

        public string PrintUnitBoxLabel(string printerName)
        {
            currentUnitSerial++;
            string serialNumber = GenerateSerialNumber(serialNumber: currentUnitSerial);
            SaveLastSerial(UnitSerialFile, currentUnitSerial);

            string zplCommand = $@"^XA^PON^LH0,0^LL0444
                                ^FO306,124^GFA,00256,00256,00004,:Z64:eJxjYKAnOADEDxgYGD8AMZBmBvKZgULsQMxEV4fAAQBLYQRC:3CC2
                                ^BY3,2,59^FT100,60^BEN,,Y,N^FD8888888855287^FS
                                ^FT120,110^A0N,24,24^FH\\^FDET-SFR82MBEGMX^FS
                                ^FT30,130^A0N,16,16^FH\\^FDSPORT BAND(ET-SFR82) COLOR : AUQA BLACK^FS
                                ^FT30,150^A0N,12,12^FH\\^FDMANUFACTURE DATE: 20250524^FS
                                ^FT30,180^A0N,25,22^FH\\^FDMADE IN VIETNAM / FABRIQUE EN VIETNAM^FS
                                ^BY2,3,24^FT30,210^BCN,,Y,N^FD>:RF7Y5500016KXDU3^FS
                                ^FT98,230^A0N,20,25^FH^FDS/N:^FS
                                ^FT450,230^A0N,18,20^FH^FDA^FS
                                ^BY2,3,24^FT30,260^BCN,,Y,N^FD>:RF7Y5500016KXDU3^FS
                                ^FT98,280^A0N,20,25^FH^FDS/N:^FS
                                ^FT450,280^A0N,18,20^FH^FDB^FS
                                ^BY2,3,24^FT30,310^BCN,,Y,N^FD>:RF7Y5500016KXDU3^FS
                                ^FT98,330^A0N,20,25^FH^FDS/N:^FS
                                ^FT450,330^A0N,18,20^FH^FDC^FS
                                ^PQ1,0,1,Y^XZ";

            if (!RawPrinterHelper.SendStringToPrinter(printerName, zplCommand))
                throw new Exception("Failed to send Unit Box label to printer.");
            return serialNumber;
        }
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
            return data.SN;
        }

        public string PrintMiddleBoxLabel(string printerName, int quantity = 10)
        {
            currentMiddleSerial++;
            string lotNumber = GenerateLotNumber("TY", 'Y', '5', currentMiddleSerial);
            string serialList = string.Join(",", Enumerable.Range(1, quantity).Select(i => GenerateSerialNumber(serialNumber: currentUnitSerial + i)));
            currentUnitSerial += quantity;
            SaveLastSerial(MiddleSerialFile, currentMiddleSerial);
            SaveLastSerial(UnitSerialFile, currentUnitSerial);

            string zplCommand = $@"^XA
^CF0,30
^FO50,30^BCN,50,Y,N,N^FD8806086927079^FS
^FO50,90^FDBATTERY PACK^FS
^CF0,28
^FO50,120^FDMODEL : ET-SFR82^FS
^FO50,160^FDSKU   : ET-SFR82MBEGMX^FS
^FO50,200^FDLOT NO: {lotNumber}^FS
^FO50,240^FDMADE IN VIETNAM^FS
^FO50,280^FDQTY: {quantity} PCS^FS
^FO50,320^DMN,60,60^FD{serialList}^FS
^XZ";

            if (!RawPrinterHelper.SendStringToPrinter(printerName, zplCommand))
                throw new Exception("Failed to send Middle Box label to printer.");
            return lotNumber;
        }

        public string PrintMasterBoxLabel(string printerName, int quantity = 50)
        {
            currentMasterSerial++;
            string lotNumber = GenerateLotNumber("TY", 'Y', '5', currentMasterSerial);
            string serialList = string.Join(",", Enumerable.Range(1, quantity).Select(i => GenerateSerialNumber(serialNumber: currentUnitSerial + i)));
            currentUnitSerial += quantity;
            SaveLastSerial(MasterSerialFile, currentMasterSerial);
            SaveLastSerial(UnitSerialFile, currentUnitSerial);

            string zplCommand = $@"^XA
^CF0,30
^FO50,30^BCN,50,Y,N,N^FD8806086927079^FS
^FO50,90^FDBATTERY PACK^FS
^CF0,28
^FO50,120^FDMODEL : ET-SFR82^FS
^FO50,160^FDSKU   : ET-SFR82MBEGMX^FS
^FO50,200^FDLOT NO: {lotNumber}^FS
^FO50,240^FDMADE IN VIETNAM^FS
^FO50,280^FDQTY: {quantity} PCS^FS
^FO50,320^DMN,80,80^FD{serialList}^FS
^XZ";

            if (!RawPrinterHelper.SendStringToPrinter(printerName, zplCommand))
                throw new Exception("Failed to send Master Box label to printer.");
            return lotNumber;
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