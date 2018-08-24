using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Shared
{
    public static class MessageHelper
    {
        public static Message Serialize(object anySerializableObject)
        {
            using (var memoryStream = new MemoryStream())
            {
                (new BinaryFormatter()).Serialize(memoryStream, anySerializableObject);
                return new Message { Key = anySerializableObject.GetType().Name, Data = memoryStream.ToArray() };
            }
        }

        public static object Deserialize(Message message)
        {
            using (var memoryStream = new MemoryStream(message.Data))
                return (new BinaryFormatter()).Deserialize(memoryStream);
        }

        public static byte[] SerializeMessage(Message message)
        {
            using (var memoryStream = new MemoryStream())
            {
                (new BinaryFormatter()).Serialize(memoryStream, message);
                return memoryStream.ToArray();
            }
        }

        public static Message DeserializeMessage(byte[] data)
        {
            using (var memoryStream = new MemoryStream(data))
                return (Message)(new BinaryFormatter()).Deserialize(memoryStream);
        }
    }
}