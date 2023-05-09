using System;
using System.IO;
using System.Windows.Forms;

namespace charposition
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.SetCompatibleTextRenderingDefault(false);

            string text = 
@"class Socket
{
   void Connect(string server)
   {
      SocketLibrary.Connect(mSocket, server);
   }

   void Disconnect()
   {
      SocketLibrary.Disconnect(mSocket);
   }
}";

            string title = "Sample code";

            if (args.Length > 0)
            {
                text = File.ReadAllText(args[0]);
                title = args[0];
            }

            Application.Run(new Form1(title, text));
        }
    }
}
