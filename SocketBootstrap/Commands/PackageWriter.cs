using Shared;
using Shared.Packets.Server;
using SocketServer.Servers.Custom;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocketServer.Commands
{
    public static class PackageWriter
    {
        private static readonly byte[] header = Encoding.ASCII.GetBytes("##");
        private static readonly byte[] footer = Encoding.ASCII.GetBytes("$$");

        public static void Write(CustomSession session, BaseServerPacket obj)
        {
            Message m = MessageHelper.Serialize(obj);
            byte[] b = MessageHelper.SerializeMessage(m);

            byte[] rv = new byte[header.Length + b.Length + footer.Length];

            Buffer.BlockCopy(header, 0, rv, 0, header.Length);
            Buffer.BlockCopy(b, 0, rv, header.Length, b.Length);
            Buffer.BlockCopy(footer, 0, rv, header.Length + b.Length, footer.Length);
            
            session.Send(rv, 0, rv.Length);
        }
        
        // TODO: manueel de packets serializen en kijken of er plaats is in de buffer alvorens toe te voegen, anders packet al versturen en nieuwe maken
        public static void FlushAll(Dictionary<CustomSession, Queue<BaseServerPacket>> outgoingPackages)
        {
            if (outgoingPackages != null)
            {
                var packages = outgoingPackages;

                foreach (KeyValuePair<CustomSession, Queue<BaseServerPacket>> kv in packages)
                {
                    svMulti m = new svMulti();
                    while (kv.Value.Count > 0)
                    {
                        m.packets.Add(kv.Value.Dequeue());
                    }
                    Write(kv.Key, m);
                }
            }
        }
    }
}