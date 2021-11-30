using System;
using System.IO;
using Microsoft.Win32;

namespace Win32Reg
{
    class Program
    {
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, uint pvParam, uint fWinIni);

        const int SPI_SETCURSORS = 0x0057;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDCHANGE = 0x02;

        static void Main(string[] args)
        {
            // 컴퓨터\HKEY_CURRENT_USER\Control Panel\Cursors
            string previousValue = ReadControlPanelCursorArrow();
            Console.WriteLine($"Previous value: {previousValue}");

            bool result = false;
            if (args.Length > 0)
            {
                result = WriteControlPanelCursorArrow(args[0]);
            }
            else
            {
                result = ResetControlPanelCursorArrow();
            }
            Console.WriteLine($"ControlPanel.Cursor.Arrow: {result}");
            SystemParametersInfo(SPI_SETCURSORS, 0, 0, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
        }

        public static string ReadControlPanelCursorArrow()
        {
            RegistryKey regKey;

            try
            {
                regKey = Registry.CurrentUser.OpenSubKey("Control Panel").OpenSubKey("Cursors");
            }
            catch (Exception e)
            {
                return "";
            }

            return regKey.GetValue("Arrow", "").ToString();
        }

        public static bool WriteControlPanelCursorArrow(string path)
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(Path.Combine("Control Panel", "Cursors"), true);

            try
            {
                path = Path.GetFullPath(path);
                regKey.SetValue("Arrow", path);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error: {e}");
                return false;
            }

            return true;
        }

        public static bool ResetControlPanelCursorArrow()
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(Path.Combine("Control Panel", "Cursors"), true);

            try
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "git", "TheNewFeature", "retrograde-tour-agency", "resources", "cursor-guide.cur");
                regKey.SetValue("Arrow", @"%SystemRoot%\cursors\aero_arrow.cur");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error: {e}");
                return false;
            }

            return true;
        }

        /*
        public string ReadReg(string appName, string subKey, string key)
        {
            RegistryKey regKey;

            try
            {
                regKey = Registry.CurrentUser.OpenSubKey(RegistryRoot + appName).OpenSubKey(subKey);
            }
            catch (Exception e)
            {
                return "";
            }

            return regKey.GetValue(key, "").ToString();
        }

        public bool WriteReg(string appName, string subKey, string key, string value)
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(RegistryRoot + appName, writable: true);
            if (regKey == null)
            {
                // regKey = Registry.CurrentUser.CreateSubKey(RegistryRoot + appName);
                return false;
            }

            RegistryKey regKeySub = Registry.CurrentUser.OpenSubKey(RegistryRoot + appName + "\\" + subKey, true);
            if (regKeySub == null)
            {
                // regKeySub = Registry.CurrentUser.CreateSubKey(RegistryRoot + appName + "\\" + subKey);
                return false;
            }

            try
            {
                regKeySub.SetValue("Arrow", "");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error: {e}");
                return false;
            }

            return true;
        }
        */
    }
}