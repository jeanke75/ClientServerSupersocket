using System;
using System.Text;

namespace Shared.Packets
{
    class PacketSerializer
    {
        private byte[] buffer = new byte[4096];
        private int index = 0;

        private byte ReadByte()
        {
            byte data = buffer[index];
            index++;

            return data;
        }

        public void WriteByte(byte data)
        {
            buffer[index] = data;
            index++;
        }

        public byte[] ReadBytes(int count)
        {
            byte[] data = new byte[count];
            Buffer.BlockCopy(buffer, index, data, 0, count);
            index += count;

            return data;
        }

        public void WriteBytes(byte[] data)
        {
            Buffer.BlockCopy(data, 0, buffer, index, data.Length);
            index += data.Length;
        }

        public int ReadInteger()
        {
            return BitConverter.ToInt32(ReadBytes(4), 0);
        }

        public void WriteInteger(int data)
        {
            WriteBytes(BitConverter.GetBytes(data));
        }

        public short ReadShort()
        {
            return BitConverter.ToInt16(ReadBytes(2), 0);
        }

        public void WriteShort(short data)
        {
            WriteBytes(BitConverter.GetBytes(data));
        }

        public float ReadFloat()
        {
            return BitConverter.ToSingle(ReadBytes(4), 0);
        }

        public void WriteFloat(float data)
        {
            WriteBytes(BitConverter.GetBytes(data));
        }

        public string ReadString()
        {
            int length = ReadInteger();
            string data = Encoding.ASCII.GetString(buffer, index, length);
            index += length;

            return data;
        }

        public void WriteString(string data)
        {
            WriteInteger(data.Length);
            WriteBytes(Encoding.ASCII.GetBytes(data));
        }
    }
}
