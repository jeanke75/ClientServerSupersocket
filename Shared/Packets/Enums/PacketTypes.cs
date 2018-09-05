namespace Shared.Packets.Enums
{
    public enum PacketType : byte
    {
        SV_REGISTER,
        SV_LOGIN,
        SV_LOGOUT,
        SV_CHAT,
        SV_SYNC,
        SV_MOVE,

        C_REGISTER,
        C_LOGIN,
        C_LOGOUT,
        C_CHAT,
        C_SYNC,
        C_MOVE,
    }
}