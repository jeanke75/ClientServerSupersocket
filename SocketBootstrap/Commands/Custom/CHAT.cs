﻿using System;
using System.Linq;
using ClassLibrary;
using ClassLibrary.Models;
using SocketServer.Servers.Custom;
using SuperSocket.SocketBase.Command;

namespace SocketServer.Commands.Custom
{
    public class CHAT : CommandBase<CustomSession, CustomDataRequest>
    {
        public override void ExecuteCommand(CustomSession session, CustomDataRequest requestInfo)
        {
            if (session.userName == "")
            {
                session.Send("LOGINERR Not logged in!\r\n");
                return;
            }

            try
            {
                Chat chat = MessageHelper.Deserialize(requestInfo.Message) as Chat;

                if (chat.Message == "")
                {
                    session.Send("CHATERR Please provide a message.\r\n");
                }

                switch (chat.Type)
                {
                    case ChatTypes.Whisper:
                        HandleWhisper(session, chat);
                        break;
                    default:
                        HandleNormal(session, chat);
                        break;
                }
            }
            catch (Exception ex)
            {
                session.Send("CHATERR " + ex.Message + " " + ex.GetType().ToString() + "\r\n");
            }
        }

        private void HandleWhisper(CustomSession session, Chat chat)
        {
            if (chat.Recipient != "")
            {
                CustomSession sOther = session.AppServer.GetAllSessions().Where(x => x.userName == chat.Recipient).FirstOrDefault();
                if (sOther != null)
                {
                    if (sOther.userName != session.userName)
                    {
                        session.Send(string.Format("CHAT {0} -> {1}: {2}\r\n", session.userName, sOther.userName, chat.Message));
                        sOther.Send(string.Format("CHAT {0} -> {1}: {2}\r\n", session.userName, sOther.userName, chat.Message));
                    }
                    else
                    {
                        session.Send("CHATERR Can't whisper yourself!\r\n");
                    }
                }
                else
                {
                    session.Send("CHATERR Didn't find the user " + chat.Recipient + ".\r\n");
                }
            }
            else
            {
                session.Send("CHATERR Please provide a username.\r\n");
            }
        }

        private void HandleNormal(CustomSession session, Chat chat)
        {
            foreach (var s in session.AppServer.GetAllSessions())
            {
                s.Send("CHAT " + session.userName + ": " + chat.Message + "\r\n");
            }
        }
    }
}
