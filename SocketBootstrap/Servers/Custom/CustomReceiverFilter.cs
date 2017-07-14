using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using ClassLibrary;
using SuperSocket.SocketBase.Protocol;

namespace SocketServer.Servers.Custom
{
    public class CustomReceiverFilter : IReceiveFilter<CustomDataRequest>
    {
        private const string ProtocolHeaderStartKey = "#";
        private const int ProtocolHeaderKeyRepetitions = 2;
        private const int ProtocolDataLength = 2;
        private const int MaxBufferSize = 5 * 1024 * 1024; // 5 Megabyte
        private readonly byte[] _inputBuffer;
        private readonly byte _protocolHeaderStartBinaryKey;
        private int _bufferLastOffset;

        public int LeftBufferSize { get; private set; }

        public IReceiveFilter<CustomDataRequest> NextReceiveFilter { get; private set; }

        public FilterState State { get; private set; }

        public CustomReceiverFilter()
        {
            _inputBuffer = new byte[MaxBufferSize];
            _protocolHeaderStartBinaryKey = Encoding.ASCII.GetBytes(ProtocolHeaderStartKey)[0];
        }

        public CustomDataRequest Filter(byte[] readBuffer,
                   int offset, int length,
                   bool toBeCopied, out int rest)
        {
            // Copy to local buffer
            Array.Copy(readBuffer, offset, _inputBuffer, _bufferLastOffset, length);
            _bufferLastOffset += length;

            if (_bufferLastOffset < ProtocolHeaderKeyRepetitions + ProtocolDataLength)
            {
                // Wait for full portion
                rest = 0;
                return null;
            }

            // Detect protocol header            
            var protocolHeaderStartIndex = Array.IndexOf(
                _inputBuffer,
                _protocolHeaderStartBinaryKey,
                0,
                ProtocolHeaderKeyRepetitions);
            
            if (-1 == protocolHeaderStartIndex)
            {
                // Flush data
                Reset();
                rest = 0;
                return null;
            }

            byte[] data = new byte[length - ProtocolHeaderKeyRepetitions];
            Buffer.BlockCopy(_inputBuffer, _bufferLastOffset - length + protocolHeaderStartIndex + ProtocolHeaderKeyRepetitions, data, 0, length - ProtocolHeaderKeyRepetitions);

            Message msg = MessageHelper.DeserializeMessage(data);
            object o = MessageHelper.Deserialize(msg);
            Type x = o.GetType();

            rest = 0;

            return new CustomDataRequest(msg.Key, msg);
        }

        public void Reset()
        {
            _bufferLastOffset = 0;
        }
    }
}
