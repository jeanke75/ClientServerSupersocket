using System;
using ClassLibrary;
using SuperSocket.ProtoBase;

namespace Client.Filters
{
    internal class CustomReceiveFilter : TerminatorReceiveFilter<CustomPackageInfo>
    {
        public byte[] header;
        public CustomReceiveFilter(byte[] header, byte[] terminator) : base(terminator)
        {
            this.header = header;
        }

        public override CustomPackageInfo ResolvePackage(IBufferStream bufferStream)
        {
            var total = (int)bufferStream.Length;
            if (total > 100)
            {
                byte[] buffer = new byte[total];
                bufferStream.Read(buffer, 0, total - 2);

                byte[] needle = header;

                int index = GetIndex(buffer, needle);
                byte[] r = new byte[buffer.Length - index];
                Buffer.BlockCopy(buffer, index + 2, r, 0, buffer.Length - index - needle.Length);
                Message m = MessageHelper.DeserializeMessage(r);

                return new CustomPackageInfo(MessageHelper.Deserialize(m));
            }

            bufferStream.Clear();

            return null;
        }

        private static int GetIndex(byte[] haystack, byte[] needle)
        {
            var len = needle.Length;
            var limit = haystack.Length - len;
            for (var i = 0; i <= limit; i++)
            {
                var k = 0;
                for (; k < len; k++)
                {
                    if (needle[k] != haystack[i + k]) break;
                }
                if (k == len) return i;
            }
            return -1;
        }
    }
}
