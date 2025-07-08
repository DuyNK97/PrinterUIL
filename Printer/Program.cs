using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Printer
{
    internal static class Program
    {
        private const string MutexName = "Global\\OnlyOnePrinterApp";

        [STAThread]
        static void Main()
        {
            bool isNewInstance = false;
            using (Mutex mutex = new Mutex(true, MutexName, out isNewInstance))
            {
                if (!isNewInstance || IsAnotherInstanceRunning())
                {
                    MessageBox.Show("Ứng dụng đã đang chạy!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
        }

        private static bool IsAnotherInstanceRunning()
        {
            var current = Process.GetCurrentProcess();
            var processes = Process.GetProcessesByName(current.ProcessName);

            // Đếm xem có process nào cùng tên nhưng khác PID không
            return processes.Any(p => p.Id != current.Id);
        }
    }
}
