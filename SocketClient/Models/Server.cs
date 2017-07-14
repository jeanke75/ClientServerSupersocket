using System.ComponentModel;
using SocketClient.Extra;

namespace SocketClient.Models
{
    public class Server
    {
        [TypeConverter(typeof(EnumDescriptionTypeConverter))]
        public enum Types {
            [Description("Telnet")]
            Telnet,
            [Description("Custom")]
            Custom
        }

        public Types Type { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
    }
}
