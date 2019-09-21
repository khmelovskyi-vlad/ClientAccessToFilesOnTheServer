using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientAccessToFilesOnTheServer
{
    class Program
    {
        static void Main(string[] args)
        {
            NetClient netClient = new NetClient();
            netClient.Client();
            Console.WriteLine("Conection is ending");
            Console.ReadKey();
        }
    }
}
