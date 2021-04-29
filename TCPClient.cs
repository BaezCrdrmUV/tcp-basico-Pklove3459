using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace tcp_com
{
    public class TCPClient
    {
        TcpClient client;
        string IP;
        int Port;
        string Username;
        bool activo;

        public TCPClient(string ip, int port, string username)
        {
            try
            {
                client = new TcpClient();
                this.IP = ip;
                this.Port = port;
                this.Username = username;
            }
            catch (System.Exception)
            {
                
            }
        }
        public static Message Deserialize<Message>(byte[] source)
        {
            var asString = Encoding.Unicode.GetString(source);
            return JsonConvert.DeserializeObject<Message>(asString);
        }
        public void Chat()
        {
            int idMensaje = 0;
            activo = true;
            client.Connect(IP, Port);

            Console.WriteLine("IP: " + IP);
            Console.WriteLine("Port: " + Port.ToString());   
            Console.WriteLine("Conectado");
            Console.WriteLine("\nSigue las instrucciones o envia el mensaje que desees");
            Console.WriteLine("Escribe list para mostrar los mensajes que has escrito");
            Console.WriteLine("Escribe change para cambiar un mensaje");
            Console.WriteLine("Escribe delete para eliminar un mensaje");
            Console.WriteLine("Escribe bye para terminar el programa\n");

            while(activo)
            {
                try
                {
                    
                    string msg = Console.ReadLine();
                    if(msg.Equals("bye"))
                    {
                        Message newMessage = new Message(idMensaje,msg, Username, DateTime.Now);
                        string jsonMessage = JsonConvert.SerializeObject(newMessage);
                        // Envío de datos
                        var stream = client.GetStream();
                        byte[] data = Encoding.UTF8.GetBytes(jsonMessage);
                        Console.WriteLine("Enviando datos...\n");
                        stream.Write(data, 0, data.Length);
                        activo = false;
                        Console.WriteLine("--TERMINANDO EJECUCION--");
                    }else
                    {
                        if(msg.Equals("list"))
                        {
                            Message newMessage = new Message(idMensaje,msg, Username, DateTime.Now);
                            string jsonMessage = JsonConvert.SerializeObject(newMessage);
                            // Envío de datos
                            var stream = client.GetStream();
                            byte[] data = Encoding.UTF8.GetBytes(jsonMessage);
                            Console.WriteLine("Imprimiendo lista mensajes\n");
                            stream.Write(data, 0, data.Length);

                            // Recepción de mensajes
                            byte[] package = new byte[1024];
                            stream.Read(package);
                            var asString = Encoding.Unicode.GetString(package);
                            List<Message> mensajes = new List<Message>();
                            mensajes = JsonConvert.DeserializeObject<List<Message>>(asString);
                            foreach (Message mesg in mensajes)
                            {
                                Console.WriteLine(mesg.id +" - "+ mesg.MessageString);
                            }
                            Console.WriteLine("Fin de la lista de mensajes");
                        }
                        else
                        {
                            if(msg.Equals("change"))
                            {
                                Message newMessaged = new Message(idMensaje,msg, Username, DateTime.Now);
                                string jsonMessaged = JsonConvert.SerializeObject(newMessaged);
                                // Envío de datos
                                var stream = client.GetStream();
                                byte[] data = Encoding.UTF8.GetBytes(jsonMessaged);
                                stream.Write(data, 0, data.Length);

                                Console.WriteLine("Dijite el identificador de mensaje que quiere cambiar");
                                int idas = Convert.ToInt32(Console.ReadLine());
                                Console.WriteLine("Ingrese el nuevo texto del mensaje");
                                String mensaj = Console.ReadLine();
                                Message newMessage = new Message(idas,mensaj, Username, DateTime.Now);
                                string jsonMessage = JsonConvert.SerializeObject(newMessage);

                                // Envío de datos
                                var streame = client.GetStream();
                                byte[] datae = Encoding.UTF8.GetBytes(jsonMessage);
                                Console.WriteLine("Enviando datos...");
                                streame.Write(datae, 0, datae.Length);

                                // Recepción de mensajes
                                byte[] package = new byte[1024];
                                streame.Read(package);
                                string serverMessage = Encoding.UTF8.GetString(package);
                                Console.WriteLine(serverMessage);

                            }
                            else
                            {
                                if(msg.Equals("delete"))
                                {
                                Message newMessaged = new Message(idMensaje,msg, Username, DateTime.Now);
                                string jsonMessaged = JsonConvert.SerializeObject(newMessaged);
                                // Envío de datos
                                var stream = client.GetStream();
                                byte[] data = Encoding.UTF8.GetBytes(jsonMessaged);
                                stream.Write(data, 0, data.Length);

                                Console.WriteLine("Dijite el numero de mensaje que quiere eliminar");
                                int idas = Convert.ToInt32(Console.ReadLine());
                                Message newMessage = new Message(idas,"empty", Username, DateTime.Now);
                                string jsonMessage = JsonConvert.SerializeObject(newMessage);

                                // Envío de datos
                                var streame = client.GetStream();
                                byte[] datae = Encoding.UTF8.GetBytes(jsonMessage);
                                Console.WriteLine("Enviando datos...");
                                streame.Write(datae, 0, datae.Length);

                                // Recepción de mensajes
                                byte[] package = new byte[1024];
                                streame.Read(package);
                                string serverMessage = Encoding.UTF8.GetString(package);
                                Console.WriteLine(serverMessage);
                                }
                                else
                                {
                                    idMensaje++;
                                    Message newMessage = new Message(idMensaje,msg, Username, DateTime.Now);
                                    string jsonMessage = JsonConvert.SerializeObject(newMessage);

                                    // Envío de datos
                                    var stream = client.GetStream();
                                    byte[] data = Encoding.UTF8.GetBytes(jsonMessage);
                                    Console.WriteLine("Enviando datos...");
                                    stream.Write(data, 0, data.Length);

                                    // Recepción de mensajes
                                    byte[] package = new byte[1024];
                                    stream.Read(package);
                                    string serverMessage = Encoding.UTF8.GetString(package);
                                    Console.WriteLine(serverMessage);
                                }
                            }
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error {0}", ex.Message);
                }
            }
        }
    }
}