using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PacketWriter
{
    List<byte> DataBuffer;

    public PacketWriter()
    {
        DataBuffer = new List<byte>();
    }

    public byte[] ToArray()
    {
        return DataBuffer.ToArray();
    }

    public void WriteInt(int Value)
    {
        DataBuffer.AddRange(BitConverter.GetBytes(Value));
    }

    public void WriteFloat(float Value)
    {
        DataBuffer.AddRange(BitConverter.GetBytes(Value));
    }

    public void WriteString(string Value)
    {
        DataBuffer.AddRange(BitConverter.GetBytes(Value.Length));
        DataBuffer.AddRange(Encoding.ASCII.GetBytes(Value));
    }

    public void WriteVector3(Vector3 Value)
    {
        WriteFloat(Value.x);
        WriteFloat(Value.y);
        WriteFloat(Value.z);
    }

    public void WriteQuaternion(Quaternion Value)
    {
        WriteFloat(Value.x);
        WriteFloat(Value.y);
        WriteFloat(Value.z);
        WriteFloat(Value.w);
    }
}
