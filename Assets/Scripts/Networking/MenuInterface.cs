//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class MenuInterface : MonoBehaviour
//{
//    //Root UI objects to enable/disable once menu is finished being used
//    public GameObject ConnectServerObject;
//    public GameObject LoginRootObject;
//    public GameObject RegisterRootObject;
//    public GameObject ChatWindowObject;
//    public GameObject ConnectingAnimationObject;
//    public GameObject CharacterCreationObject;

//    //Connect server input fields
//    public Text IPField;
//    public Text PortField;
//    //Account registration input fields
//    public Text RegisterName;
//    public Text RegisterPassword;
//    //Account login input fields
//    public Text LoginName;
//    public Text LoginPassword;

//    //Attempts to connect to the game server
//    public void ClickConnect()
//    {
//        //Get the entered server IP and port from the UI input components
//        string ServerIP = IPField.text;
//        int ServerPort = 0;
//        Int32.TryParse(PortField.text, out ServerPort);
//        //Use this information and try to connect to that server
//        GetComponent<ServerConnection>().ConnectToServer(ServerIP, ServerPort);
//    }
    
//    //Sends a request to the game server to register a new account into the database
//    public void ClickRegister()
//    {
//        //grab the credentials from the UI
//        string Username = RegisterName.text;
//        string Password = RegisterPassword.text;
//        //send the account registration request to the game server
//        PacketSender.instance.SendRegisterRequest(Username, Password);
//    }
//    //Sends a request to the game server to login to an account and start playing the game
//    public void ClickLogin()
//    {
//        //grab the credentials from the UI
//        string Username = LoginName.text;
//        string Password = LoginPassword.text;
//        //send the account login request to the game server
//        PacketSender.instance.SendLoginRequest(Username, Password);
//    }

//    public void ClickCreate()
//    {
//        //Get all the data of the character that is being created
//        string Username = LoginName.text;
//        CharacterCreation CharacterSheet = CharacterCreationObject.GetComponent<CharacterCreation>();
//        int StatPointsLeft = CharacterSheet.StatPointsLeft;
//        //Only allow the character creation to be completed once the user has assigned all of their alloted stat points
//        if (StatPointsLeft != 0)
//            return;
//        PacketSender.instance.SendCreateCharacter(Username, CharacterSheet.Strength, CharacterSheet.Agility, CharacterSheet.Stamina, CharacterSheet.Intelligence);
//    }
    
//    //hides the account registration and login menu interface components
//    public void HideAccountUI()
//    {
//        LoginRootObject.SetActive(false);
//        RegisterRootObject.SetActive(false);
//    }
//    //unhides the account registration and login menu interface components
//    public void ShowAccountUI()
//    {
//        LoginRootObject.SetActive(true);
//        RegisterRootObject.SetActive(true);
//    }
//    //hides the server connection interface components
//    public void HideConnectionMenu()
//    {
//        ConnectServerObject.SetActive(false);
//    }
//    //unhides the server connection interface components
//    public void ShowConnectionMenu()
//    {
//        ConnectServerObject.SetActive(true);
//    }
//    //hides the connection waiting animation UI element
//    public void HideConnectingAnimation()
//    {
//        ConnectingAnimationObject.SetActive(false);
//    }
//    //unhides the connection waiting animation UI element
//    public void ShowConnectingAnimation()
//    {
//        ConnectingAnimationObject.SetActive(true);
//    }
//    public void ShowGameUI(string AccountName)
//    {
//        ChatWindowObject.SetActive(true);
//        ChatWindowObject.GetComponent<ChatWindow>().AccountName = AccountName;
//    }
//    public void ShowCharacterCreation() { CharacterCreationObject.SetActive(true); }
//    public void HideCharacterCreation() { CharacterCreationObject.SetActive(false); }
//}