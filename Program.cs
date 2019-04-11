using System;  
using System.Net;  
using System.Net.Sockets;  
using System.Text;  

namespace NMS {
    public class SynchronousSocketListener {  
    
        // Incoming data from the client.  
        public static string data = null;  
    
        public static void StartListening() {  
            // Data buffer for incoming data.  
            byte[] bytes = new Byte[1024];  
    
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());  
            IPAddress ipAddress = IPAddress.Loopback;  
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);  
    
            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily,  
                SocketType.Stream, ProtocolType.Tcp );  
    
            try {  
                listener.Bind(localEndPoint);  
                listener.Listen(10);  
 
                while (true) {  
                    Console.WriteLine("Waiting for a connection...");  
                    // Program is suspended while waiting for an incoming connection.  
                    new Handler(listener.Accept());
                }  
    
            } catch (Exception e) {  
                Console.WriteLine(e.ToString());  
            }  
    
            Console.WriteLine("\nPress ENTER to continue...");  
            Console.Read();  
    
        }  
    
        public static int Main(String[] args) {  
            StartListening();  
            return 0;  
        }  
    }
}  