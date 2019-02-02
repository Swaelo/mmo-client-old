using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MenuState
{
    Connecting = 1,  //waiting to establish a connection with the server
    MainMenu = 2,    //from here the user can choose to register a new account, login to an already existing account, or exit from the game
    AccountRegister = 3,  //menu interface is active allowing user to enter in their desired account information
    AwaitingRegisterReply = 4,  //waiting for the server to tell us if our account registration was sucessful or not
    AccountLogin = 5,   //menu ui is active allowing user to enter in their account login details
    AwaitingLoginReply = 6,      //waiting for the server to tell us if our login request was succesful or not
    AwaitingCharacterData = 7,  //waiting for the server to tell us what characters we have created and registered to our account
    CharacterSelect = 8, //Menu screen where user can choose to enter the game world with one of their characters, or go to the character creation menu
    CharacterCreation = 9,   //Menu to create a brand new player character
    AwaitingCharacterCreation = 10, 
    PlayingGame = 11
}

public class MenuHandler : MonoBehaviour
{
    //Current state of the menu we are in, starts off by displaying a loading animation until the connection to the server has been established
    private MenuState CurrentMenu = MenuState.Connecting;

    //Account credentials
    private string AccountUsername;
    private string AccountPassword;

    //Connection establishment and main menu
    [SerializeField] private GameObject ConnectingAnimationObject;
    [SerializeField] private GameObject MainMenuObject;
    //Account registration and login windows
    [SerializeField] private GameObject AccountRegisterObject;
    [SerializeField] private GameObject RegisteringAnimationObject;
    [SerializeField] private GameObject AccountLoginObject;
    [SerializeField] private GameObject LoggingInAnimationObject;
    //Character selection screen
    [SerializeField] private GameObject CharacterSelectObject;
    [SerializeField] private GameObject LoadingCharacterDataAnimationObject;
    [SerializeField] private GameObject CharacterCreationObject;
    [SerializeField] private GameObject CreatingCharacterAnimationObject;
    [SerializeField] private InputField CreateCharacterNameInput;
    [SerializeField] private Slider CreateCharacterGenderSwitch;
    //Game interface objects
    public GameObject ChatWindowObject;

    //Character data for up to 3 characters to be displayed in the character selection menu
    public int CharactersCreated = -1;
    public CharacterData SelectedCharacterData;
    public CharacterData FirstCharacterData;
    public CharacterData SecondCharacterData;
    public CharacterData ThirdCharacterData;
    [SerializeField] private GameObject FirstCharacterSlot;
    [SerializeField] private GameObject SecondCharacterSlot;
    [SerializeField] private GameObject ThirdCharacterSlot;
    [SerializeField] private GameObject CreateNewCharacterButton;

    //Text input fields used in account creation and account login menu states
    [SerializeField] private InputField RegisterUsernameInput;
    [SerializeField] private InputField RegisterPasswordInput;
    [SerializeField] private InputField RegisterPasswordVerifyInput;
    [SerializeField] private InputField LoginUsernameInput;
    [SerializeField] private InputField LoginPasswordInput;

    //The menu will interface with the connection manager to establish a connection to the game server
    [SerializeField] private ServerConnection GameServer;
    private float ConnectionTimeout = -1;   //We will time out server connection requests after 5 seconds
    private float RegisterTimeout = -1;
    private float LoginTimeout = -1;
    private bool Connecting = false;

    private void Update()
    {
        //Certain menu states have timeout limits, count them down while they are active
        switch(CurrentMenu)
        {
            case (MenuState.Connecting):
                TryConnection();    //waiting to connect to the server
                break;
            case (MenuState.AwaitingRegisterReply):
                AwaitRegisterReply();
                break;
            case (MenuState.AwaitingLoginReply):
                AwaitLoginReply();
                break;
        }
    }
    private void TryConnection()
    {
        //First time we enter this function immediately try connecting to the game server
        if(!Connecting)
        {
            Console.Instance.Print("Connecting...");
            Connecting = true;
            ConnectionTimeout = 3;
            GameServer.TryConnect();
        }

        //Check to see if the connection to the server has been established yet
        if(GameServer.IsServerConnected())
        {
            Console.Instance.Print("Connected");
            CurrentMenu = MenuState.MainMenu;
            //Disable the connection waiting animation and display the main menu
            ConnectingAnimationObject.SetActive(false);
            MainMenuObject.SetActive(true);
            return;
        }

        //If we are still waiting to connect to the server see if this request has timed out yet
        ConnectionTimeout -= Time.deltaTime;
        if(ConnectionTimeout <= 0.0f)
        {
            Console.Instance.Print("Connection timed out, trying again...");
            ConnectionTimeout = 3;
            GameServer.TryConnect();
        }
    }

    public void HideAll()
    {
        ConnectingAnimationObject.SetActive(false);
        MainMenuObject.SetActive(false);
        AccountRegisterObject.SetActive(false);
        RegisteringAnimationObject.SetActive(false);
        AccountLoginObject.SetActive(false);
        LoggingInAnimationObject.SetActive(false);
        CharacterSelectObject.SetActive(false);
        LoadingCharacterDataAnimationObject.SetActive(false);
        CharacterCreationObject.SetActive(false);
        CreatingCharacterAnimationObject.SetActive(false);
    }

    //Main menu button functions
    public void ClickEnterRegisterMenuButton()
    {
        //disable main menu and enable account registration menu interface
        CurrentMenu = MenuState.AccountRegister;
        MainMenuObject.SetActive(false);
        AccountRegisterObject.SetActive(true);
    }
    public void ClickEnterLoginMenuButton()
    {
        //disable main menu and enable account login menu interface
        CurrentMenu = MenuState.AccountLogin;
        MainMenuObject.SetActive(false);
        AccountLoginObject.SetActive(true);
    }
    public void ClickQuitButton()
    {
        //close server connection and quit the game
        GameServer.CloseConnection();
        Application.Quit();
    }
    public void ClickCancelAccountLoginButton()
    {
        CurrentMenu = MenuState.MainMenu;
        AccountLoginObject.SetActive(false);
        MainMenuObject.SetActive(true);
    }
    public void ClickCancelCreateAccountButton()
    {
        CurrentMenu = MenuState.MainMenu;
        AccountRegisterObject.SetActive(false);
        MainMenuObject.SetActive(true);
    }
    //Attempt to create an account with whatever has been entered in
    public void ClickCreateAccountButton()
    {
        //Extract the desired account credentials entered into the user interface
        AccountUsername = RegisterUsernameInput.text;
        AccountPassword = RegisterPasswordInput.text;
        string PasswordVerify = RegisterPasswordVerifyInput.text;
        //Ignore the request if any of the input fields were left empty
        if(AccountUsername == "" || AccountPassword == "" || PasswordVerify == "")
        {
            Console.Instance.Print("Please fill the form correctly, something has been left empty.");
            return;
        }
        //Make sure the passwords match
        if(AccountPassword != PasswordVerify)
        {
            Console.Instance.Print("Password verification does not match");
            return;
        }

        //Send a request to the server to register the new account and display a waiting animation until we receive a reply from them, this times out after 3 seconds
        PacketSender.instance.SendRegisterRequest(AccountUsername, AccountPassword);
        CurrentMenu = MenuState.AwaitingRegisterReply;
        AccountRegisterObject.SetActive(false);
        RegisteringAnimationObject.SetActive(true);
        RegisterTimeout = 3;
    }
    //Attempt to log into an account with whatever has been entered in
    public void ClickLoginAccountButton()
    {
        //Extract the desired account credentials entered into the user interface
        AccountUsername = LoginUsernameInput.text;
        AccountPassword  = LoginPasswordInput.text;
        //Ignore the request if any of the input fields were left empty
        if (AccountUsername == "" || AccountPassword == "")
        {
            Console.Instance.Print("Please fill the form correctly, something has been left empty.");
            return;
        }
        //Send a request to the server to log into this account and display a waiting animation until the request times our or we receive a reply from the server
        PacketSender.instance.SendLoginRequest(AccountUsername, AccountPassword);
        CurrentMenu = MenuState.AwaitingLoginReply;
        AccountLoginObject.SetActive(false);
        LoggingInAnimationObject.SetActive(true);
        LoginTimeout = 3;
    }
    public void ClickCreateCharacterButton()
    {
        string CharacterName = CreateCharacterNameInput.text;
        bool IsMale = CreateCharacterGenderSwitch.value == 0;
        //Ignore the request if they have not entered any character name
        if(CharacterName == "")
        {
            Console.Instance.Print("Please enter a name for your character");
            return;
        }
        //Send a request to the server to create a brand new character with this name, display a waiting animation until we hear something back or the request times out
        PacketSender.instance.SendCreateCharacterRequest(AccountUsername, CharacterName, IsMale);
        CreatingCharacterAnimationObject.SetActive(true);
        CharacterCreationObject.SetActive(false);
        CurrentMenu = MenuState.AwaitingCharacterCreation;
    }
    public void ClickCancelCreateCharacterButton()
    {

    }

    public void WorldEntered()
    {
        LoggingInAnimationObject.SetActive(false);
        ChatWindowObject.SetActive(true);
        CurrentMenu = MenuState.PlayingGame;
    }

    public void CreateCharacterSuccess()
    {
        if (CurrentMenu != MenuState.AwaitingCharacterCreation)
            return;

        CurrentMenu = MenuState.AwaitingCharacterData;
        CreatingCharacterAnimationObject.SetActive(false);
        LoadingCharacterDataAnimationObject.SetActive(true);
        PacketSender.instance.SendGetCharacterDataRequest(AccountUsername);
    }
    public void CreateCharacterFailure()
    {
        if (CurrentMenu != MenuState.AwaitingCharacterCreation)
            return;

        CharacterCreationObject.SetActive(true);
        CreatingCharacterAnimationObject.SetActive(false);
        CurrentMenu = MenuState.CharacterCreation;
    }

    public void ClickLoginFirstCharacter()
    {
        SelectedCharacterData = FirstCharacterData;
        GameServer.CurrentCharacterName = FirstCharacterData.Name;
        CharacterSelectObject.SetActive(false);
        LoggingInAnimationObject.SetActive(true);
        CurrentMenu = MenuState.AwaitingLoginReply;
        PacketSender.instance.SendEnterWorldRequest(FirstCharacterData);
    }
    public void ClickLoginSecondCharacter()
    {
        SelectedCharacterData = SecondCharacterData;
        GameServer.CurrentCharacterName = SecondCharacterData.Name;
        CharacterSelectObject.SetActive(false);
        LoggingInAnimationObject.SetActive(true);
        CurrentMenu = MenuState.AwaitingLoginReply;
        PacketSender.instance.SendEnterWorldRequest(SecondCharacterData);
    }
    public void ClickLoginThirdCharacter()
    {
        SelectedCharacterData = ThirdCharacterData;
        GameServer.CurrentCharacterName = ThirdCharacterData.Name;
        CharacterSelectObject.SetActive(false);
        LoggingInAnimationObject.SetActive(true);
        CurrentMenu = MenuState.AwaitingLoginReply;
        PacketSender.instance.SendEnterWorldRequest(ThirdCharacterData);
    }

    public void InitialiseCharacterSelectionScreen()
    {
        //Set up the first character select menu
        if(CharactersCreated >= 1)
        {
            FirstCharacterSlot.SetActive(true);
            FirstCharacterSlot.GetComponent<SetCharacterSlot>().SetData(FirstCharacterData);
        }
        //second character select
        if(CharactersCreated >= 2)
        {
            SecondCharacterSlot.SetActive(true);
            SecondCharacterSlot.GetComponent<SetCharacterSlot>().SetData(SecondCharacterData);
        }
        //third character selection
        if(CharactersCreated >= 3)
        {
            ThirdCharacterSlot.SetActive(true);
            ThirdCharacterSlot.GetComponent<SetCharacterSlot>().SetData(ThirdCharacterData);
        }
        //Display the button to enter character creation screen if less than 3 characters exist in the users account
        if (CharactersCreated < 3)
            CreateNewCharacterButton.SetActive(true);
        else
            CreateNewCharacterButton.SetActive(false);
    }

    public void ClickCreateNewCharacterButton()
    {
        //Clicked from within the character selection screen
        CharacterSelectObject.SetActive(false);
        CurrentMenu = MenuState.CharacterCreation;
        CharacterCreationObject.SetActive(true);
        CurrentMenu = MenuState.AwaitingCharacterCreation;
    }

    public void RegisterSuccess()
    {
        //Only listen to account registration replies while in the await register reply menu state
        if (CurrentMenu != MenuState.AwaitingRegisterReply)
            return;
        //Once an account has been registered succesfully we will then automatically try to log into it
        CurrentMenu = MenuState.AwaitingLoginReply;
        RegisteringAnimationObject.SetActive(false);
        LoggingInAnimationObject.SetActive(true);
        PacketSender.instance.SendLoginRequest(RegisterUsernameInput.text, RegisterPasswordInput.text);
        LoginTimeout = 3;
    }
    public void RegisterFail()
    {
        //Only listen to account registration replies while in the await register reply menu state
        if (CurrentMenu != MenuState.AwaitingRegisterReply)
            return;
        //If an account was not able to be registered we will return to the account registration menu so they can try again
        CurrentMenu = MenuState.AccountRegister;
        RegisteringAnimationObject.SetActive(false);
        AccountRegisterObject.SetActive(true);
    }
    public void LoginSuccess()
    {
        //Only listen to account login replies while in the await login reply menu state
        if (CurrentMenu != MenuState.AwaitingLoginReply)
            return;
        //Once an account has been logged into successfully then we move onto the character select screen
        CurrentMenu = MenuState.CharacterSelect;
        LoggingInAnimationObject.SetActive(false);
        LoadingCharacterDataAnimationObject.SetActive(true);
        //Send a request to the server for all the information about any characters we have created so far
        PacketSender.instance.SendGetCharacterDataRequest(AccountUsername);
    }
    public void LoginFail()
    {
        //Only listen to account login replies while in the await login reply menu state
        if (CurrentMenu != MenuState.AwaitingLoginReply)
            return;
        //If an account was not able to be logged into, return to the account login menu
        CurrentMenu = MenuState.AccountLogin;
        LoggingInAnimationObject.SetActive(false);
        AccountLoginObject.SetActive(true);
    }

    //If the log into our account and there are no characters created we will be sent straight to the character creation screen
    public void NoCharactersCreated()
    {
        //Disable the waiting for character data animation and head to the character selection screen
        LoadingCharacterDataAnimationObject.SetActive(false);
        CharacterCreationObject.SetActive(true);
        CurrentMenu = MenuState.CharacterCreation;
    }
    public void CharacterDataLoaded()
    {
        //Move onto the character selection screen now
        LoadingCharacterDataAnimationObject.SetActive(false);
        CharacterSelectObject.SetActive(true);
        CurrentMenu = MenuState.CharacterSelect;
        InitialiseCharacterSelectionScreen();
    }

    private void AwaitRegisterReply()
    {
        //Wait until 3 seconds have passed then give up waiting for the server to tell us what happened
        RegisterTimeout -= Time.deltaTime;
        if (RegisterTimeout <= 0.0f)
        {
            //If we have not received the account registration reply yet then print an error and return to the register menu
            Console.Instance.Print("Account registration request timed out");
            CurrentMenu = MenuState.AccountRegister;
            RegisteringAnimationObject.SetActive(false);
            AccountRegisterObject.SetActive(true);
        }
    }
    private void AwaitLoginReply()
    {
        //Wait until 3 seconds have passed then give up waiting for the server to tell us what happened
        LoginTimeout -= Time.deltaTime;
        if (LoginTimeout <= 0.0f)
        {
            Console.Instance.Print("Account login request timed out");
            CurrentMenu = MenuState.AccountLogin;
            LoggingInAnimationObject.SetActive(false);
            AccountLoginObject.SetActive(true);
        }
    }
}
