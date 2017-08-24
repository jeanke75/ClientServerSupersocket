using System;
using System.Collections.Generic;
using System.Linq;
using ClassLibrary;
using ClassLibrary.Packets.Client;
using ClassLibrary.Packets.Enums;
using ClassLibrary.Packets.Server;
using SocketServer.Servers.Custom;
using SuperSocket.SocketBase.Command;

namespace SocketServer.Commands.Custom
{
    public class CHAT : CommandBase<CustomSession, CustomDataRequest>
    {
        public override string Name
        {
            get
            {
                return "c" + base.Name;
            }
        }

        public override void ExecuteCommand(CustomSession session, CustomDataRequest requestInfo)
        {
            // not logged in
            if (session.player == null) return;

            cChat chatc = MessageHelper.Deserialize(requestInfo.Message) as cChat;

            // empty message
            if (string.IsNullOrEmpty(chatc.Message)) return;
            try
            {
                svChat chats = new svChat() { Type = chatc.Type, Message = chatc.Message, Sender = session.player.Username, Recipient = chatc.Recipient };

                switch (chatc.Type)
                {
                    case ChatTypes.Whisper:
                        HandleWhisper(session, chats);
                        break;
                    case ChatTypes.Party:
                        HandleParty(session, chats);
                        break;
                    case ChatTypes.Guild:
                        HandleGuild(session, chats);
                        break;
                    case ChatTypes.Server:
                        HandleServer(session, chats);
                        break;
                    case ChatTypes.All:
                        HandleAll(session, chats);
                        break;
                    default:
                        HandleNormal(session, chats);
                        break;
                }
            }
            catch (Exception ex)
            {
                HandleError(session, ex.Message + " " + ex.GetType().ToString());
            }
        }

        private void HandleError(CustomSession session, string message)
        {
            svChat c = new svChat() { Message = message, Type = ChatTypes.Error };
            PackageWriter.Write(session, c);
        }

        private void HandleWhisper(CustomSession session, svChat chat)
        {
            if (chat.Recipient != "")
            {
                CustomSession sOther = session.AppServer.GetAllSessions().Where(x => x.player != null && x.player.Username == chat.Recipient).FirstOrDefault();
                if (sOther != null)
                {
                    if (sOther.player.Username != session.player.Username)
                    {
                        PackageWriter.Write(session, chat);
                        PackageWriter.Write(sOther, chat);
                    }
                    else
                    {
                        HandleError(session, "Can't whisper yourself!");
                    }
                }
                else
                {
                    HandleError(session, "Didn't find the user " + chat.Recipient + ".");
                }
            }
            else
            {
                HandleError(session, "Please provide a username.");
            }
        }

        private void HandleNormal(CustomSession session, svChat chat)
        {
            foreach (var s in session.AppServer.GetAllSessions().Where(x => x.player != null && x.player.MapName == session.player.MapName))
            {
                PackageWriter.Write(s, chat);
            }
        }

        private void HandleParty(CustomSession session, svChat chat)
        {
            throw new NotImplementedException();
        }

        private void HandleGuild(CustomSession session, svChat chat)
        {
            throw new NotImplementedException();
        }

        private void HandleServer(CustomSession session, svChat chat)
        {
            foreach (var s in session.AppServer.GetAllSessions().Where(x => x.player != null))
            {
                PackageWriter.Write(s, chat);
            }
        }

        private void HandleAll(CustomSession session, svChat chat)
        {
            chat.Server = session.AppServer.Name;
            List<CustomServer> servers = ((CustomServer)session.AppServer).GetAllServersOfSameType();
            foreach(CustomServer server in servers)
            {
                foreach(CustomSession s in server.GetAllSessions())
                {
                    PackageWriter.Write(s, chat);
                }
            }
        }
    }
}
