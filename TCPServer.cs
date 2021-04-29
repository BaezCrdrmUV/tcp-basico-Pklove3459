using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using System.Threading;
using System.Collections.Generic;
using System.Text.Json;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace tcp_com
{
    public class TCPServer
    {
        public TcpListener listener { get; set; }
        public bool acceptFlag { get; set; }
        public List <Message> mensajes { get; set; }
        public List <int> hilosId { get; set; }
        public bool hilosAbiertos;


        public TCPServer(string ip, int port, bool start = false)
        {
            mensajes = new List<Message>();
            hilosId = new List<int>();
            hilosAbiertos = false;

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
         public void watchOpenedThreads()
        {
            while(true)
            {
                if(hilosAbiertos == true && hilosId.Count == 0)
                {
                    Console.WriteLine("Terminado");
                    listener.Stop();
                    listener = null;
                    break;
                }
            }
            Console.WriteLine("Opened messages");
            displayMessages();
            Thread.CurrentThread.Join();
        }

        public void displayMessages()
        {
            Console.WriteLine("Mensajes en la colección");
            foreach (Message msg in mensajes)
            {
                Console.WriteLine("{0} >> {1}", msg.User, msg.MessageString);
            }
        }
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
        TypeNameHandling = TypeNameHandling.Auto,
        };

        public void HandleCommunication(Object obj)
        {
            ThreadParams param = (ThreadParams)obj;
            Socket client = param.obj;

            if(client != null)
            {
                Console.WriteLine("Cliente conectado. Esperando datos");
                string msg = "|";
                DateTime now  = DateTime.Now;
                Message mensaje = new Message();
                        

                while(mensaje != null && !mensaje.MessageString.Equals("bye"))
                {
                    try
                    {
                        switch(msg)
                        {
                            case "list":
                            var asString = JsonConvert.SerializeObject(mensajes,SerializerSettings);
                            client.Send(Encoding.Unicode.GetBytes(asString));
                            break;

                            case "change":
                                bool cambio = false;
                                byte[] buffer1 = new byte[1024];
                                client.Receive(buffer1);
                                var uft8Reader1 = new Utf8JsonReader(buffer1);
                                mensaje = System.Text.Json.JsonSerializer.Deserialize<Message>(ref uft8Reader1);
                                foreach(Message mes12 in mensajes)
                                {
                                    if(mes12.id == mensaje.id)
                                    {
                                        cambio = true;
                                        mes12.MessageString = mensaje.MessageString;
                                        break;
                                    }
                                }
                                if(cambio == true)
                                {
                                    byte[] data1 = Encoding.UTF8.GetBytes("Mensaje Actualizado");
                                    client.Send(data1);
                                }
                                else
                                {
                                    byte[] data1 = Encoding.UTF8.GetBytes("No se encontro el mensaje");
                                    client.Send(data1);
                                }
                            break;

                            case "delete":
                                byte[] buffer2 = new byte[1024];
                                client.Receive(buffer2);
                                var uft8Reader2 = new Utf8JsonReader(buffer2);
                                mensaje = System.Text.Json.JsonSerializer.Deserialize<Message>(ref uft8Reader2);
                                mensajes.RemoveAt(mensaje.id);
                                byte[] data3 = Encoding.UTF8.GetBytes("Mensaje Eliminado");
                                client.Send(data3);
                            break;

                            case "|":
                            break;
                            
                            default:
                                // Enviar un mensaje al cliente
                                byte[] data = Encoding.UTF8.GetBytes("Mensaje Enviado");
                                client.Send(data);
                            break;
                        }
                                // Escucha por nuevos mensajes
                                byte[] buffer = new byte[1024];
                                client.Receive(buffer);
                                var uft8Reader = new Utf8JsonReader(buffer);
                                mensaje = System.Text.Json.JsonSerializer.Deserialize<Message>(ref uft8Reader);
                                msg = mensaje.MessageString;
                                if(msg != "list" && msg != "bye" && msg != "change" && msg != "delete" )
                                {
                                    Console.WriteLine(mensaje.Hour.Hour +":"+ mensaje.Hour.Minute + " "+mensaje.User+ "- "+ mensaje.MessageString);
                                    mensajes.Add(mensaje);
                                }
                        
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("Exception", msg, ex.Message);
                    }
                }

                Console.WriteLine("Cerrando conexión");
                client.Dispose();
                foreach (var item in hilosId)
                {
                    Console.WriteLine(item);
                }
                Console.WriteLine("------------");
                hilosId.Remove(param.id);
                foreach (var item in hilosId)
                {
                    Console.WriteLine(item);
                }
                Thread.CurrentThread.Join();
            }
        }
        public void Listen()
        {
            if(listener != null && acceptFlag == true)
            {
                int id = 0;
                Thread watch = new Thread(new ThreadStart(watchOpenedThreads));
                watch.Start();

                while(true)
                {
                    Console.WriteLine("Esperando conexión del cliente...");

                    if(hilosAbiertos) break;
                     try
                    {
                        var clientSocket = listener.AcceptSocket();
                        Console.WriteLine("Cliente aceptado");

                        Thread thread = new Thread(new ParameterizedThreadStart(HandleCommunication));
                        thread.Start(new ThreadParams(clientSocket, id));
                        hilosId.Add(id);
                        id++;
                        hilosAbiertos = true;
                    }
                    catch (System.Exception)
                    {
                        
                    }

                }
            }
        }

        public class ThreadParams
        {
            public Socket obj { get; set; }
            public int id { get; set; }

                public ThreadParams(Socket obj, int id)
                {
                    this.obj = obj;
                    this.id = id;
                }
        }

    }
}