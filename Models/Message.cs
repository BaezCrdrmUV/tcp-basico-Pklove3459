using System;

namespace tcp_com
{
    public class Message
    {
        public int id {get; set;}
        public string MessageString { get; set; }
        public string User { get; set; }

        public DateTime Hour { get; set; }

        public Message()
        {
            MessageString = "";
            User = "Default";
        }

        public Message(int id,string messageString, string user,DateTime hour)
        {
            this.id = id;
            this.MessageString = messageString;
            this.User = user;
            this.Hour = hour;
        }
    }
}