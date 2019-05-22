using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class PlayerCommunicationPacketHandler
{
    public static void HandlePlayerMessage(PacketReader Reader)
    {
        Log.PrintIncomingPacket("PlayerCommunication.HandlePlayerMessage");
        string MessageSender = Reader.ReadString();
        string MessageContents = Reader.ReadString();
        //Display the message to the UI chat window
        Log.PrintChatMessage(MessageSender + MessageContents);
    }
}