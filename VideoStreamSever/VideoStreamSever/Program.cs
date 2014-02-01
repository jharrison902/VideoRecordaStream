using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace VideoStreamSever
{
    class Program
    {
        static readonly int MAX_BUFFER = 2048;
        static readonly int MAX_CONNECTIONS = 10;
        static void Main(string[] args)
        {
            //byte[] dataBuffer = new byte[MAX_BUFFER];

            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11337);

            
        }
    }
}
