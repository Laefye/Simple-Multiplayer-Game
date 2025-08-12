using System.IO;
using UnityEngine;

namespace Net
{
    /// <summary>
    /// Represents a network packet with binary serialization capabilities
    /// </summary>
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

        /// <summary>
        /// Creates an empty packet for writing
        /// </summary>
        /// <returns>A new empty packet</returns>
        public static Packet Empty()
        {
            return new Packet(new MemoryStream(), Mode.Write);
        }

        /// <summary>
        /// Creates a packet from existing byte data for reading
        /// </summary>
        /// <param name="bytes">The byte array containing packet data</param>
        /// <param name="count">Number of bytes to read from the array</param>
        /// <returns>A packet ready for reading</returns>
        public static Packet From(byte[] bytes, int count)
        {
            return new Packet(new MemoryStream(bytes, 0, count), Mode.Read);
        }

        /// <summary>
        /// Gets the packet data as a byte array
        /// </summary>
        /// <returns>The serialized packet data</returns>
        public byte[] GetBytes()
        {
            _stream.Flush();
            return _stream.ToArray();
        }

        /// <summary>
        /// Binary reader for reading packet data (available only in read mode)
        /// </summary>
        public BinaryReader Reader { get; }

        /// <summary>
        /// Binary writer for writing packet data (available only in write mode)
        /// </summary>
        public BinaryWriter Writer { get; }

        /// <summary>
        /// Length of the packet data in bytes
        /// </summary>
        public long Length => _stream.Length;

        /// <summary>
        /// Writes a Vector3 to the packet
        /// </summary>
        /// <param name="position">The Vector3 to write</param>
        public void Write(Vector3 position)
        {
            Writer.Write(position.x);
            Writer.Write(position.y);
            Writer.Write(position.z);
        }
        
        /// <summary>
        /// Writes a Quaternion to the packet
        /// </summary>
        /// <param name="rotation">The Quaternion to write</param>
        public void Write(Quaternion rotation)
        {
            Writer.Write(rotation.x);
            Writer.Write(rotation.y);
            Writer.Write(rotation.z);
            Writer.Write(rotation.w);
        }
        
        /// <summary>
        /// Reads a Vector3 from the packet
        /// </summary>
        /// <returns>The read Vector3</returns>
        public Vector3 ReadVector3()
        {
            return new Vector3(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle());
        }
        
        /// <summary>
        /// Reads a Quaternion from the packet
        /// </summary>
        /// <returns>The read Quaternion</returns>
        public Quaternion ReadQuaternion()
        {
            return new Quaternion(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle());
        }
    }
}