using System.Linq;
using System.Text;
using SuperSocket.ProtoBase;

namespace Client.Filters
{
    internal class TelnetReceiveFilter : TerminatorReceiveFilter<StringPackageInfo>
    {
        public TelnetReceiveFilter(byte[] terminator) : base(terminator) { }

        public override StringPackageInfo ResolvePackage(IBufferStream bufferStream)
        {
            var total = (int)bufferStream.Length;
            var chunksizeString = bufferStream.ReadString(total - 2, Encoding.ASCII);

            string[] stringArray = chunksizeString.Split(' ');
            string[] parameters = stringArray.Skip(1).ToArray();
            string body = string.Join(" ", parameters);

            return new StringPackageInfo(stringArray.First(), body, parameters);
        }
    }
}
