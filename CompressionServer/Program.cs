using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace CompressionServer
{
    class Program
    {
        const int PORT = 9000;
        static int clientCounter = 0;

        static void Main(string[] args)
        {
            TcpListener server = new TcpListener(IPAddress.Any, PORT);
            server.Start();
            Console.WriteLine("=======================================");
            Console.WriteLine("  Multi-threaded Compression Server   ");
            Console.WriteLine($"  Listening on port {PORT}...          ");
            Console.WriteLine("=======================================");
            Console.WriteLine();

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                int clientId = Interlocked.Increment(ref clientCounter);
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Client #{clientId} connected from " +
                    $"{((IPEndPoint)client.Client.RemoteEndPoint).Address}");

                Thread thread = new Thread(() => HandleClient(client, clientId));
                thread.IsBackground = true;
                thread.Start();
            }
        }

        static void HandleClient(TcpClient tcpClient, int clientId)
        {
            try
            {
                tcpClient.ReceiveTimeout = 30000;
                tcpClient.SendTimeout    = 30000;

                using (tcpClient)
                using (NetworkStream netStream = tcpClient.GetStream())
                using (BinaryReader reader = new BinaryReader(netStream))
                using (BinaryWriter writer = new BinaryWriter(netStream))
                {
                    // Step 1: Read file size (8 bytes)
                    long fileSize = reader.ReadInt64();
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Client #{clientId} -> Receiving {fileSize:N0} bytes...");

                    // Step 2: Read file data
                    byte[] fileData = reader.ReadBytes((int)fileSize);

                    // Step 3: Compress
                    byte[] compressed = Compress(fileData);
                    double ratio = (1.0 - (double)compressed.Length / fileSize) * 100;
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Client #{clientId} -> Compressed to {compressed.Length:N0} bytes (saved {ratio:F1}%)");

                    // Step 4: Send compressed size
                    writer.Write((long)compressed.Length);

                    // Step 5: Send compressed data
                    writer.Write(compressed);
                    writer.Flush();

                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Client #{clientId} -> Done!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Client #{clientId} ERROR: {ex.Message}");
            }
        }

        static byte[] Compress(byte[] data)
        {
            using (var output = new MemoryStream())
            {
                using (var gzip = new GZipStream(output, CompressionLevel.Optimal))
                    gzip.Write(data, 0, data.Length);
                return output.ToArray();
            }
        }
    }
}
