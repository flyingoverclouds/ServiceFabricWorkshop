using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LegacyHttpServer
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 8080;
            if (args.Length > 0)
            {
                if (!int.TryParse(args[0], out port))
                    port = 8080;
            }

            TcpListener server = null;
            try
            {
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(localAddr, port);
                server.Start();


                while (true)
                {
                    Console.WriteLine("Waiting for connection ....");
                    TcpClient client = server.AcceptTcpClient();
                    try
                    {
                        Console.WriteLine("New connection arrived.");
                        NetworkStream stream = client.GetStream();
                        byte[] bytes = new byte[4096];
                        stream.Read(bytes, 0, bytes.Length);
                        
                        Console.WriteLine("    answering.");
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("HTTP/1.1 200 OK");
                        sb.AppendLine("Content-Type: text/html");
                        sb.AppendLine();

                        sb.AppendLine($"\r\n<html><head><title>{Environment.MachineName}</title></head><body><h1>{Environment.MachineName}</h1>local server time : [{DateTime.Now.ToString()}]<br/></body></html>");
                        byte[] msg = System.Text.Encoding.UTF8.GetBytes(sb.ToString());

                        stream.Write(msg, 0, msg.Length);
                        stream.Flush();
                        client.Close();
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(@"ERROR.txt", "EXCEPTION ** : ClientSocketException: " + ex.ToString());
                    }
                }
            }
            catch (SocketException e)
            {
                File.AppendAllText(@"ERROR.txt", "EXCEPTION : SocketException: " + e.ToString());
            }
            finally
            {
                server.Stop();
            }
        }
    }
}
