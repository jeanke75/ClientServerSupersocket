using System;
using System.Linq;
using ClassLibrary;
using ClassLibrary.Models;
using ClassLibrary.Packets.Client;
using ClassLibrary.Packets.Server;
using SocketServer.Config;
using SocketServer.Servers.Custom;
using SuperSocket.SocketBase.Command;

namespace SocketServer.Commands.Custom
{
    public class REGISTER : CommandBase<CustomSession, CustomDataRequest>
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
            (session.AppServer as CustomServer).simulation.packetsIn.Enqueue(requestInfo.Message);
            svRegister regs = new svRegister();
            try
            {
                cRegister regc = MessageHelper.Deserialize(requestInfo.Message) as cRegister;
                if (string.IsNullOrEmpty(regc.Username?.Trim()) || (regc.Password?.Trim() ?? "").Length < ConfigHelper.Account.MinPasswordLength || string.IsNullOrEmpty(regc.Email?.Trim()))
                {
                    regs.ErrorMessage = "Please fill in the required fields.";
                }
                else if (CustomServer.Accounts.FirstOrDefault(x => x.Username.Equals(regc.Username)) != null)
                {
                    regs.ErrorMessage = "Username already in use.";
                }
                else
                {
                    RegisterConfigElement rce = ConfigHelper.Account.Register;
                    CustomServer.Accounts.Add(new Player() { Username = regc.Username, Password = regc.Password, Email = regc.Email, X = (ushort)rce.X, Y = (ushort)rce.Y, MapName = rce.MapName });
                    regs.Success = true;
                }
            }
            catch (Exception ex)
            {
                regs.ErrorMessage = ex.Message + " " + ex.GetType().ToString();
            }
            finally
            {
                regs.Success = string.IsNullOrEmpty(regs.ErrorMessage);
                PackageWriter.Write(session, regs);
            }
        }
    }
}
