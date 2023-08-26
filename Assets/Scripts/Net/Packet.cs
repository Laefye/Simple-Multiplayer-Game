using System.Collections;
using System.Diagnostics.Contracts;
using System.IO;
using UnityEngine;

namespace Net
{
    public class Packet
    {
        private readonly MemoryStream _stream;

        private enum Mode
        {
            Read,
            Write,
        }

        private Packet(MemoryStream stream, Mode mode)
        {
            _stream = stream;
            if (mode == Mode.Read)
            {
                Reader = new BinaryReader(stream);
            }
            else if (mode == Mode.Write)
            {
                Writer = new BinaryWriter(stream);
            }
        }

        public static Packet Empty()
        {
            return new Packet(new MemoryStream(), Mode.Write);
        }

        public static Packet From(byte[] bytes, int count)
        {
            return new Packet(new MemoryStream(bytes, 0, count), Mode.Read);
        }

        public byte[] GetBytes()
        {
            _stream.Flush();
            return _stream.ToArray();
        }

        public BinaryReader Reader { get; }

        public BinaryWriter Writer { get; }

        public long Length => _stream.Length;

        public void Write(Vector3 position)
        {
            Writer.Write(position.x);
            Writer.Write(position.y);
            Writer.Write(position.z);
        }
        
        public void Write(Quaternion position)
        {
            Writer.Write(position.x);
            Writer.Write(position.y);
            Writer.Write(position.z);
            Writer.Write(position.w);
        }
        
        public Vector3 ReadVector3()
        {
            return new Vector3(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle());
        }
        
        public Quaternion ReadQuaternion()
        {
            return new Quaternion(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle());
        }
    }
}