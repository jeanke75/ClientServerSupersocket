using System;
using System.Text;
using ClassLibrary;
using SocketServer.Servers.Custom;

namespace SocketServer.Commands
{
    public static class PackageWriter
    {
        private static readonly byte[] header = Encoding.ASCII.GetBytes("##");
        private static readonly byte[] footer = Encoding.ASCII.GetBytes("$$");

        public static void Write(CustomSession session, object obj)
        {
            Message m = MessageHelper.Serialize(obj);
            byte[] b = MessageHelper.SerializeMessage(m);

            byte[] rv = new byte[header.Length + b.Length + footer.Length];

            Buffer.BlockCopy(header, 0, rv, 0, header.Length);
            Buffer.BlockCopy(b, 0, rv, header.Length, b.Length);
            Buffer.BlockCopy(footer, 0, rv, header.Length + b.Length, footer.Length);

            session.Send(rv, 0, rv.Length);
        }
    }
}
