using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Microsoft.Win32;


namespace woloff
{
    public class UDPListener
    {
        private const int listenPort = 9;
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;



        private static void StartListener()
        {
            var handle = GetConsoleWindow();

            // Hide
            ShowWindow(handle, SW_HIDE);

            UdpClient listener = new UdpClient(listenPort);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);

            try
            {
                while (true)
                {
                    Console.WriteLine("Waiting for broadcast");
                    byte[] bytes = listener.Receive(ref groupEP);

                    Console.WriteLine($"Received broadcast from {groupEP} :");
                    string strCmdText;
                    strCmdText = "/c shutdown -s -f -t 0";
                    System.Diagnostics.Process.Start("CMD.exe", strCmdText);

                    Console.WriteLine($" {Encoding.ASCII.GetString(bytes, 0, bytes.Length)}");

                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                listener.Close();
            }

        }

        public static void Main()
        {
            string appName = "woloff"; // Uygulama adını burada belirtin
            string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            // Otomatik başlatılacak kayıt defteri anahtarının adını belirleyin
            string runKeyName = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

            // Kayıt defteri anahtarını açın
            RegistryKey runKey = Registry.CurrentUser.OpenSubKey(runKeyName, true);

            // Anahtar varsa kaldırın
            if (runKey.GetValue(appName) != null)
            {
                runKey.DeleteValue(appName);
                Console.WriteLine("Kayıt defteri girdisi kaldırıldı.");
            }

            // Anahtarı yeniden oluşturun ve uygulamayı otomatik olarak başlatın
            runKey.SetValue(appName, appPath);
            Console.WriteLine("Kayıt defteri girdisi oluşturuldu ve uygulama otomatik olarak başlatılacak.");

            // Uygulama devam etmek için bekler
            Console.ReadLine();

            StartListener();

        }


    }

}

