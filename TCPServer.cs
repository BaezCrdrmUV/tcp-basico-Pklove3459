using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;

namespace tcp_com
{
    public class TCPServer
    {
        public TcpListener listener { get; set; }
        public bool acceptFlag { get; set; }


        public TCPServer(string ip, int port, bool start = false)
        {
            IPAddress address = IPAddress.Parse(ip);
            this.listener = new TcpListener(address, port);

            if(start == true)
            {
                listener.Start();
                Console.WriteLine("Servidor iniciado en la dirección {0}:{1}",
                    address.MapToIPv4().ToString(), port.ToString());
                acceptFlag = true;
            }
        }

        public void Listen()
        {
            if(listener != null && acceptFlag == true)
            {
                while(true)
                {
                    Console.WriteLine("Esperando conexión del cliente...");
                    // Empieza a escuchar
                    var clientTask = listener.AcceptSocketAsync();

                    if(clientTask.Result != null)
                    {
                        Console.WriteLine("Cliente conectado. Esperando datos");
                        var client = clientTask.Result;
                        string msg = "";
                        DateTime now  = DateTime.Now;
                        Message mensaje = new Message();
                        

                        while(msg != null && !msg.Equals("bye"))
                        {
                            // Enviar un mensaje al cliente
                            byte[] data = Encoding.UTF8.GetBytes("Envía datos. Envía \"bye\" para terminar");
                            client.Send(data);

                            // Escucha por nuevos mensajes
                            byte[] buffer = new byte[1024];
                            client.Receive(buffer);
                            var uft8Reader = new Utf8JsonReader(buffer);
                            mensaje = System.Text.Json.JsonSerializer.Deserialize<Message>(ref uft8Reader);

                            msg = mensaje.MessageString;
                            Console.WriteLine(mensaje.Hour.Hour +":"+ mensaje.Hour.Minute + " "+mensaje.User+ "- "+ mensaje.MessageString);
                        }
                        Console.WriteLine("Cerrando conexión");
                        client.Dispose();
                    }
                }
            }
        }
    }
}