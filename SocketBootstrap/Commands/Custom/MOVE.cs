using Shared;
using Shared.Maps;
using Shared.Packets.Client;
using Shared.Packets.Server;
using SocketServer.Servers.Custom;
using SuperSocket.SocketBase.Command;
using System;
using System.Linq;

namespace SocketServer.Commands.Custom
{
    public class MOVE : CommandBase<CustomSession, CustomDataRequest>
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

            svMove moves = new svMove();
            try
            {
                cMove movec = MessageHelper.Deserialize(requestInfo.Message) as cMove;

                BaseMap map = new BaseMap();
                if (movec.X > map.Width) movec.X = map.Width;
                if (movec.Y > map.Height) movec.Y = map.Height;

                // player didn't move so do nothing, client should only send when there is a move
                if (session.player.X == movec.X && session.player.Y == movec.Y) return;
                

                // player jumps a large distance, teleport hack probably
                if (Math.Abs(session.player.X - movec.X) > 50 || Math.Abs(session.player.Y - movec.Y) > 50)
                {
                    moves.Success = false;
                    moves.X = session.player.X;
                    moves.Y = session.player.Y;
                    PackageWriter.Write(session, moves);
                }
                else
                {
                    // update server position
                    session.player.X = movec.X;
                    session.player.Y = movec.Y;
                    movec = null;

                    moves.Success = true;
                    PackageWriter.Write(session, moves);

                    moves.Username = session.player.Username;
                    moves.X = session.player.X;
                    moves.Y = session.player.Y;

                    // send movement to all players in the same server (change to map or part of map only later)
                    foreach (CustomSession otherSession in session.AppServer.GetAllSessions().Where(x => x.player != null && x.player.MapName == session.player.MapName && x.SessionID != session.SessionID))
                    {
                        PackageWriter.Write(otherSession, moves);
                    }
                }
            }
            catch (Exception ex)
            {
                moves.Success = false;
                moves.ErrorMessage = ex.Message + " " + ex.GetType().ToString();
                PackageWriter.Write(session, moves);
            }
        }
    }
}