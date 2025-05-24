using System;
using System.IO;
using System.Linq;

public class UnitBoxSN
{
    private const string FilePath = "last_serial.txt";
    private static readonly char[] Base33Chars = "0123456789ABCDEFGHJKLMNPQRSTVWXYZ".ToCharArray(); // Bỏ I, O, U
    private static readonly int Base = 33;

    /// <summary>
    /// Chuyển số thập phân sang hệ 33 (5 chữ số, không dùng I, O, U).
    /// </summary>
    private static string ToBase33(long number)
    {
        if (number < 0 || number > 39135392) // Giới hạn tối đa: ZZZZZ (39,135,392)
            throw new ArgumentException("Số thứ tự vượt quá giới hạn cho phép.");

        char[] result = new char[5];
        for (int i = 4; i >= 0; i--)
        {
            result[i] = Base33Chars[number % Base];
            number /= Base;
        }
        return new string(result);
    }

    /// <summary>
    /// Đọc số thứ tự gần nhất từ file.
    /// </summary>
    private static long ReadLastSerial()
    {
        if (File.Exists(FilePath))
        {
            string content = File.ReadAllText(FilePath).Trim();
            if (long.TryParse(content, out long lastSerial) && lastSerial >= 0)
                return lastSerial;
        }
        return 0; // Bắt đầu từ 0 nếu file không tồn tại hoặc lỗi
    }

    /// <summary>
    /// Lưu số thứ tự gần nhất vào file.
    /// </summary>
    private static void SaveLastSerial(long serial)
    {
        File.WriteAllText(FilePath, serial.ToString());
    }

    /// <summary>
    /// Sinh mã S/N cho Unit Box (14 chữ số).
    /// </summary>
    /// <param name="productGroup">Mã nhóm sản phẩm (R, 1, M).</param>
    /// <param name="customer">Mã nơi sản xuất (V, F, 5, ...).</param>
    /// <param name="productType">Mã loại sản phẩm (7 cho APS).</param>
    /// <param name="yearCode">Mã năm sản xuất (Y cho 2025).</param>
    /// <param name="monthCode">Mã tháng sản xuất (1-9, A-C).</param>
    /// <param name="vendorCode">Mã nhà cung cấp (2 chữ cái).</param>
    /// <param name="deliveryType">Loại giao hàng (A: Inbox, B: Bán lẻ).</param>
    /// <returns>Mã S/N 14 chữ số.</returns>
    public static string GenerateSerialNumber(char productGroup = 'R', char customer ='F',   char productType = '7',
        char yearCode = 'Y', // 2025
        char monthCode = '5', // Tháng 5
        string vendorCode = "TY", // Nhà cung cấp giả định
        char deliveryType = 'A') // Inbox
    {
        // Kiểm tra đầu vào
        if (vendorCode.Length != 2 || !vendorCode.All(char.IsLetter))
            throw new ArgumentException("Mã nhà cung cấp phải là 2 chữ cái.");
        if (!"R1M".Contains(productGroup))
            throw new ArgumentException("Mã nhóm sản phẩm không hợp lệ.");
        if (!"VFZQRX356A".Contains(customer))
            throw new ArgumentException("Mã nơi sản xuất không hợp lệ.");
        if (productType != '7')
            throw new ArgumentException("Mã loại sản phẩm phải là 7 cho APS.");
        if (!"123456789ABC".Contains(monthCode))
            throw new ArgumentException("Mã tháng không hợp lệ.");
        if (!"AB".Contains(deliveryType))
            throw new ArgumentException("Loại giao hàng phải là A hoặc B.");

        // Đọc số thứ tự gần nhất
        long lastSerial = ReadLastSerial();
        long newSerial = lastSerial + 1;

        // Chuyển sang hệ 33
        string serialBase33 = ToBase33(newSerial);

        // Tạo mã S/N: [Nhóm sản phẩm][Nơi sản xuất][Loại sản phẩm][Năm][Tháng][Số thứ tự][Check Digit][Nhà cung cấp][Loại giao hàng]
        string serialNumber = $"{productGroup}{customer}{productType}{yearCode}{monthCode}{serialBase33}X{vendorCode}{deliveryType}";

        // Lưu số thứ tự mới
        SaveLastSerial(newSerial);

        return serialNumber;
    }

}