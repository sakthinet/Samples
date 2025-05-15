
using OfficeOpenXml;

namespace WinAppXMLLLM
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // ✅ Set the license context here
           // ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}