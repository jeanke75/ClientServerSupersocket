using SuperSocket.ProtoBase;

namespace Client.Filters
{
    internal class CustomPackageInfo : IPackageInfo
    {
        public CustomPackageInfo(object data)
        {
            Data = data;
        }
        public object Data { get; private set; }
    }
}
