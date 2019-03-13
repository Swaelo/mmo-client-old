// ================================================================================================================================
// File:        PacketWriter.cs
// Description: Class used to define an array of information that is going to be send through the network to one of the clients
//              While the object is active, more values can be added dynamically and it will keep everything formatted correctly
//              When all the data has been added it can be acquired in the formatted array with the ToArray function which can be
//              passed onto the ConnectionManager class to be sent through the network to the target game client which is connected
// ================================================================================================================================

using System;
using System.Text;
using System.Collections.Generic;
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
