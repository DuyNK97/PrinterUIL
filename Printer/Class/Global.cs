using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using LicenseContext = OfficeOpenXml.LicenseContext;
using OfficeOpenXml;
using Printer.Class.Model;
using System.Windows.Forms;

namespace Printer.Class
{
    public static class Global
    {
        private static readonly Dictionary<int, char> YearCodeMapping = new Dictionary<int, char>
        {
            { 2021, 'R' }, { 2022, 'T' }, { 2023, 'W' }, { 2024, 'X' }, { 2025, 'Y' },
            { 2026, 'L' }, { 2027, 'P' }, { 2028, 'Q' }, { 2029, 'S' }, { 2030, 'Z' },
            { 2031, 'B' }, { 2032, 'C' }, { 2033, 'D' }, { 2034, 'F' }, { 2035, 'G' },
            { 2036, 'H' }, { 2037, 'J' }, { 2038, 'K' }, { 2039, 'M' }, { 2040, 'N' }
        };


        // Bộ ký tự hệ 33 (bỏ các chữ I, O, U để tránh nhầm lẫn)
        public static readonly char[] Base33Chars = "0123456789ABCDEFGHJKLMNPQRSTVWXYZ".ToCharArray();
        public static readonly int Base = 33;

        // Serial hiện tại
        public static string CurrentUnitSerial { get;  set; }
        public static string CurrentMiddleSerial { get;  set; }
        public static string CurrentMasterSerial { get;  set; }
        public static string UnitExcelfoler = @"D:\Printer\Unit";
        public static string MiddleExcelfoler = @"D:\Printer\Middle";
        public static string MasterExcelfoler = @"D:\Printer\Master";


        private static readonly object _lockData = new object();
        // Static constructor để khởi tạo serial từ file
    

        /// <summary>
        /// Đọc số thứ tự gần nhất từ file.
        /// </summary>
        public static long ReadLastSerial(string filePath)
        {
            if (File.Exists(filePath))
            {
                var content = File.ReadAllText(filePath).Trim();
                if (long.TryParse(content, out long lastSerial) && lastSerial >= 0)
                    return lastSerial;
            }
            return 0; // Mặc định bắt đầu từ 0 nếu file không hợp lệ
        }

        /// <summary>
        /// Lưu số thứ tự vào file.
        /// </summary>
        public static void SaveLastSerial(string filePath, long serial)
        {
            File.WriteAllText(filePath, serial.ToString());
        }

        public static void SaveLastSerialToSetting(string key, long serial)
        {
            Global.CurrentUnitSerial = serial.ToString();
            var values = new Dictionary<string, string>
            {
                [key] = serial.ToString()
            };
            WriteFileToTxt(GetFilePathSetting(), values);
        }
        /// <summary>
        /// Chuyển số thập phân sang hệ 33 (5 ký tự, không dùng I, O, U).
        /// </summary>
        public static string ToBase33(long number)
        {
            if (number < 0 || number > 39135392) // Giới hạn tối đa: ZZZZZ (base33^5)
                throw new ArgumentException("Số thứ tự vượt quá giới hạn cho phép.");

            char[] result = new char[5];
            for (int i = 4; i >= 0; i--)
            {
                result[i] = Base33Chars[number % Base];
                number /= Base;
            }
            return new string(result);
        }

        public static long FromBase33(string base33)
        {
            const string base33Chars = "0123456789ABCDEFGHJKLMNPQRSTVWXYZ";
            long result = 0;
            foreach (char c in base33)
            {
                int value = base33Chars.IndexOf(c);
                if (value == -1) throw new ArgumentException($"Ký tự không hợp lệ trong base33: {c}");
                result = result * 33 + value;
            }
            return result;
        }

       
        public static string GenerateSerialNumber(
            char productGroup = 'R',
            char customer = 'F',
            char productType = '7',
            char monthCode = '5',
            string vendorCode = "TY",
            char deliveryType = 'A')
        {

            int currentYear = DateTime.Now.Year;
            if (!YearCodeMapping.TryGetValue(currentYear, out char yearCode))
            {
                throw new ArgumentException($"Year {currentYear} is not supported (must be between 2021 and 2040).");
                
            }
            // Kiểm tra hợp lệ
            if (vendorCode.Length != 2 || !vendorCode.All(char.IsLetter))
                throw new ArgumentException("Mã nhà cung cấp phải gồm 2 chữ cái.");
            if (!"R1M".Contains(productGroup))
                throw new ArgumentException("Mã nhóm sản phẩm không hợp lệ.");
            if (!"VFZQRX356A".Contains(customer))
                throw new ArgumentException("Mã nơi sản xuất không hợp lệ.");
            if (productType != '7')
                throw new ArgumentException("Mã loại sản phẩm phải là '7' cho APS.");
            if (!"123456789ABC".Contains(monthCode))
                throw new ArgumentException("Mã tháng không hợp lệ.");
            if (!"AB".Contains(deliveryType))
                throw new ArgumentException("Loại giao hàng chỉ được là 'A' hoặc 'B'.");

            long currentSerialDecimal = long.Parse(Global.CurrentUnitSerial);
            long newSerialDecimal = currentSerialDecimal + 1;
            string serialBase33 = ToBase33(newSerialDecimal);

            //SaveLastSerial(UnitSerialFile, newSerial); // Ghi lại serial mới



            string serialNumber = $"{productGroup}{customer}{productType}{yearCode}{monthCode}{serialBase33}X{vendorCode}{deliveryType}";

            return serialNumber;
        }


       
        public static string GenerateMiddleLotno(string vendorcode)
        {
           
            string  currentMiddleLotno = Global.CurrentMiddleSerial;
            long newSerialDecimal = long.Parse(currentMiddleLotno) + 1;

            int currentYear = DateTime.Now.Year;
            if (!YearCodeMapping.TryGetValue(currentYear, out char yearCode))
            {
                throw new ArgumentException($"Year {currentYear} is not supported (must be between 2021 and 2040).");

            }
            char monthCode = "123456789ABC"[DateTime.Now.Month - 1];
            string dayCode = DateTime.Now.Day.ToString("D2");
            string Lotno = $"{vendorcode}{yearCode}{monthCode}{dayCode}{newSerialDecimal.ToString("D4")}";

            return Lotno;
        }





        public static string GetFilePathSetting()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "setting.txt");
        }

        public static Dictionary<string, string> ReadValueFileTxt(string filePath, List<string> keys)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();

            try
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    string[] parts = line.Split('=');

                    if (parts.Length == 2)
                    {
                        string key = parts[0].Trim();

                        if (keys.Contains(key))
                        {
                            values[key] = parts[1].Trim();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error can not read value from file txt: {ex.Message}");
            }

            return values;
        }
        public static void WriteFileToTxt(string filePath, Dictionary<string, string> values)
        {
            lock (_lockData)
            {
                try
                {
                    var lines = File.ReadAllLines(filePath).ToList();
                    var keysToUpdate = values.Keys.ToList();

                    var updatedKeys = new HashSet<string>();

                    for (int i = 0; i < lines.Count; i++)
                    {
                        var parts = lines[i].Split(new[] { '=' }, 2);
                        if (parts.Length == 2)
                        {
                            string key = parts[0].Trim();
                            if (values.ContainsKey(key))
                            {
                                lines[i] = $"{key}= {values[key]}";
                                updatedKeys.Add(key);
                            }
                        }
                    }

                    foreach (var key in keysToUpdate)
                    {
                        if (!updatedKeys.Contains(key))
                        {
                            lines.Add($"{key}= {values[key]}");
                        }
                    }

                    File.WriteAllLines(filePath, lines);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error can not write value to file txt: {ex.Message}");
                }
            }
        }
        private static readonly object lockExcel = new object();
        public static void CreateExcelFile(string path, UNITDATA unitdata)
        {
            lock (lockExcel)
            {
                try
                {
                    string localFolderMES = Path.Combine(path, DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"), DateTime.Now.ToString("dd"));

                    if (!Directory.Exists(localFolderMES))
                    {
                        Directory.CreateDirectory(localFolderMES);
                    }

                    path = Path.Combine(localFolderMES, DateTime.Now.ToString("yyyyMMdd") + ".xlsx");

                    using (var package = new ExcelPackage(new FileInfo(path)))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();

                        if (worksheet == null)
                        {
                            worksheet = package.Workbook.Worksheets.Add("Data");

                            string[] headers = { "EAN/UPC", "SKU", "ITEM MODEL", "MANUFACTURE DATE", "ORIGIN", "SN", "Print Time" };
                            for (int i = 0; i < headers.Length; i++)
                            {
                                worksheet.Cells[1, i + 1].Value = headers[i];
                                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                            }

                            package.Save();
                        }

                        int rowIndex = worksheet.Dimension?.Rows + 1 ?? 2;

                        worksheet.Cells[rowIndex, 1].Value = unitdata.EAN_UPC;
                        worksheet.Cells[rowIndex, 2].Value = unitdata.SKU;
                        worksheet.Cells[rowIndex, 3].Value = unitdata.ITEM_MODEL;
                        worksheet.Cells[rowIndex, 4].Value = unitdata.MANUFACTURE_DATE;
                        worksheet.Cells[rowIndex, 5].Value = unitdata.ORIGIN;
                        worksheet.Cells[rowIndex, 6].Value = unitdata.SN;
                        worksheet.Cells[rowIndex, 7].Value = DateTime.Now.ToString("yyyy-mm-dd HH:mm:ss");
                        package.Save();
                    }
                }
                catch (Exception ex)
                {
                    WriteLog($"Error cannot save to Excel file: {ex.Message}");
                }
            }
        }


        public static void CreateMiddleExcelFile(string path, MIDDLECODE middledata)
        {
            lock (lockExcel)
            {
                try
                {
                    string localFolderMES = Path.Combine(path, DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"), DateTime.Now.ToString("dd"));

                    if (!Directory.Exists(localFolderMES))
                    {
                        Directory.CreateDirectory(localFolderMES);
                    }

                    path = Path.Combine(localFolderMES, DateTime.Now.ToString("yyyyMMdd") + "_Middle.xlsx");

                    using (var package = new ExcelPackage(new FileInfo(path)))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();

                        if (worksheet == null)
                        {
                            worksheet = package.Workbook.Worksheets.Add("MiddleData");

                            string[] headers = { "Barcode Lotno", "Item", "MODEL", "SKU", "LOTNO", /*"MANUFACTURE DATE", */"QTY", "Barcode MODEL", "EAN/UPC", "ORIGIN", "Matrixdata", "Print Time" };
                            for (int i = 0; i < headers.Length; i++)
                            {
                                worksheet.Cells[1, i + 1].Value = headers[i];
                                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                            }

                            package.Save();
                        }

                        int rowIndex = worksheet.Dimension?.Rows + 1 ?? 2;

                        worksheet.Cells[rowIndex, 1].Value = middledata.BarcodeLotno;
                        worksheet.Cells[rowIndex, 2].Value = middledata.Item;
                        worksheet.Cells[rowIndex, 3].Value = middledata.MODEL;
                        worksheet.Cells[rowIndex, 4].Value = middledata.SKU;
                        worksheet.Cells[rowIndex, 5].Value = middledata.LOTNO;
                        //worksheet.Cells[rowIndex, 6].Value = middledata.MANUFACTURE_DATE;
                        worksheet.Cells[rowIndex, 6].Value = middledata.QTY;
                        worksheet.Cells[rowIndex, 7].Value = middledata.BarcodeMODEL;
                        worksheet.Cells[rowIndex, 8].Value = middledata.EAN_UPC;
                        worksheet.Cells[rowIndex, 9].Value = middledata.ORIGIN;
                        worksheet.Cells[rowIndex, 10].Value = middledata.Matrixdata;
                        worksheet.Cells[rowIndex, 11].Value = DateTime.Now.ToString("yyyy-mm-dd HH:mm:ss");

                        package.Save();
                    }
                }
                catch (Exception ex)
                {
                    WriteLog($"Error cannot save to Excel file: {ex.Message}");
                }
            }
        }


        public static void CreateMasterExcelFile(string path, MASTERDATA masterdata)
        {
            lock (lockExcel)
            {
                try
                {
                    string localFolderMES = Path.Combine(path, DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"), DateTime.Now.ToString("dd"));

                    if (!Directory.Exists(localFolderMES))
                    {
                        Directory.CreateDirectory(localFolderMES);
                    }

                    path = Path.Combine(localFolderMES, DateTime.Now.ToString("yyyyMMdd") + "_Master.xlsx");

                    using (var package = new ExcelPackage(new FileInfo(path)))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();

                        if (worksheet == null)
                        {
                            worksheet = package.Workbook.Worksheets.Add("MasterData");

                            string[] headers = { "Barcode Lotno", "Item", "MODEL", "SKU", "LOTNO",  "QTY", "Barcode MODEL", "EAN/UPC", "ORIGIN", "Matrixdata","Print Time" };
                            for (int i = 0; i < headers.Length; i++)
                            {
                                worksheet.Cells[1, i + 1].Value = headers[i];
                                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                            }

                            package.Save();
                        }

                        int rowIndex = worksheet.Dimension?.Rows + 1 ?? 2;

                        worksheet.Cells[rowIndex, 1].Value = masterdata.BarcodeLotno;
                        worksheet.Cells[rowIndex, 2].Value = masterdata.Item;
                        worksheet.Cells[rowIndex, 3].Value = masterdata.MODEL;
                        worksheet.Cells[rowIndex, 4].Value = masterdata.SKU;
                        worksheet.Cells[rowIndex, 5].Value = masterdata.LOTNO;
                        worksheet.Cells[rowIndex, 6].Value = masterdata.QTY;
                        worksheet.Cells[rowIndex, 7].Value = masterdata.BarcodeMODEL;
                        worksheet.Cells[rowIndex, 8].Value = masterdata.EAN_UPC;
                        worksheet.Cells[rowIndex, 9].Value = masterdata.ORIGIN;
                        worksheet.Cells[rowIndex, 10].Value = masterdata.Matrixdata;
                        worksheet.Cells[rowIndex, 11].Value = DateTime.Now.ToString("yyyy-mm-dd HH:mm:ss");

                        package.Save();
                    }
                }
                catch (Exception ex)
                {
                    WriteLog($"Error cannot save to Excel file: {ex.Message}");
                }
            }
        }

        private static readonly object lockWriteLog = new object();

        public static void WriteLog(string logMessage)
        {
            lock (lockWriteLog)
            {
                string logPath = $@"D:\Logs\Printer\{DateTime.Now.ToString("yyyy")}\{DateTime.Now.ToString("MM")}";

                string logFormat = DateTime.Now.ToLongDateString().ToString() + " - " + DateTime.Now.ToLongTimeString().ToString() + " ==> ";

                if (!Directory.Exists(logPath))
                {
                    Directory.CreateDirectory(logPath);
                }

                try
                {
                    using (StreamWriter writer = File.AppendText(logPath + "\\" + DateTime.Now.ToString("dd") + ".txt"))
                    {
                        writer.WriteLine(logFormat + logMessage);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error can not write log, error: {ex.Message}");
                }
            }
        }





    }
}
