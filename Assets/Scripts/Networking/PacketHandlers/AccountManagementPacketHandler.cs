using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AccountManagementPacketHandler
{
    public static void HandleAccountRegistrationReply(PacketReader Reader)
    {
        Log.PrintIncomingPacket("AccountManagement.AccountRegistrationReply");
        bool RegistrationSuccess = Reader.ReadInt() == 1;
        string ReplyMessage = Reader.ReadString();
        if (RegistrationSuccess)
        {
            //Log.Print("new account registered, logging in now...");
            AccountManagementPacketSender.SendLoginRequest(PlayerManager.Instance.LocalPlayer.AccountName, PlayerManager.Instance.LocalPlayer.AccountPass);
        }
        else
        {
            //Log.Print("account registration failed: " + ReplyMessage);
            MenuPanelDisplayManager.Instance.DisplayPanel("Account Creation Panel");
        }

        //Log.Print(Reader.BytesLeft() + " bytes of data left when reading inside the account registration handler");
    }

    public static void HandleLoginReply(PacketReader Reader)
    {
        Log.PrintIncomingPacket("AccountManagement.AccountLoginReply");
        bool LoginSuccess = Reader.ReadInt() == 1;
        string ReplyMessage = Reader.ReadString();
        if (LoginSuccess)
        {
            //Log.Print("account log in success");
            MenuPanelDisplayManager.Instance.DisplayPanel("Character Selection Panel");
            //AccountManagementPacketSender.SendCharacterDataRequest(PlayerManager.Instance.LocalPlayer.AccountName);
        }
        else
        {
            //Log.Print("account login failed: " + ReplyMessage);
            MenuPanelDisplayManager.Instance.DisplayPanel("Account Login Panel");
        }
    }

    public static void HandleCreateCharacterReply(PacketReader Reader)
    {
        Log.PrintIncomingPacket("AccountManagement.CreateCharacterReply");
        bool CreationSuccess = Reader.ReadInt() == 1;
        string ReplyMessage = Reader.ReadString();
        if (CreationSuccess)
        {
            //Log.Print("New character created!");
            //When a new character has been created, request all character data and display the waiting animation until its received, then go to the character select screen
            MenuPanelDisplayManager.Instance.DisplayPanel("Waiting Panel");
            AccountManagementPacketSender.SendCharacterDataRequest(PlayerManager.Instance.LocalPlayer.AccountName);
        }
        else
        {
            //Return to the character creation screen
            //Log.Print("Character creation failed: " + ReplyMessage);
            MenuPanelDisplayManager.Instance.DisplayPanel("Character Creation Panel");
        }
    }

    public static void HandleCharacterData(PacketReader Reader)
    {
        Log.PrintIncomingPacket("AccountManagement.HandleCharacterData");
        int CharacterCount = Reader.ReadInt();

        //If 0 characters data was sent through, go straight to the character creation screen
        if (CharacterCount == 0)
        {
            MenuPanelDisplayManager.Instance.DisplayPanel("Character Creation Panel");
            return;
        }
        //Otherwise go to the character selection screen
        else
        {
            MenuPanelDisplayManager.Instance.DisplayPanel("Character Selection Panel");

            //Loop through and extract each of the characters information
            for (int i = 0; i < CharacterCount; i++)
            {
                //Extract this characters information from the network packet and store it into the new CharacterData structure
                CharacterData Data = new CharacterData();
                Data.AccountOwner = Reader.ReadString();    //The account this character belongs to
                Data.CharacterName = Reader.ReadString();   //This characters ingame name
                Data.CharacterPosition = Reader.ReadVector3();  //This characters position in the game world
                Data.CharacterLevel = Reader.ReadInt(); //This characters current level
                Data.CharacterGender = (Reader.ReadInt() == 1 ? Gender.Male : Gender.Female);
                //Store this data structure in the local players list of characters
                PlayerManager.Instance.LocalPlayer.PlayerCharacters[i] = Data;
            }
        }
        //Now update and display the character selection screen
        CharacterSelectionButtonFunctions.Instance.CharacterCount = CharacterCount;
        CharacterSelectionButtonFunctions.Instance.CharacterDataLoaded();
    }
}
