using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ChatMessage
{
    public string Text;  //raw string content of this message
    public Text Component;  //UI component used to display the message
    public MessageType Type;    //What type of message this is

    //Different message types appear differently in the chat window
    public enum MessageType
    {
        PlayerMessage,
        PrivateMessage,
        ClanMessage,
        GroupMessage,
        SystemMessage,
        ErrorMessage
    }
}

public class ChatWindow : MonoBehaviour
{
    //Different types of messages are differentiated by their text color
    [SerializeField] private Color PlayerMessageColor;
    [SerializeField] private Color PrivateMessageColor;
    [SerializeField] private Color ClanMessageColor;
    [SerializeField] private Color GroupMessageColor;
    [SerializeField] private Color SystemMessageColor;
    [SerializeField] private Color ErrorMessageColor;

    public GameObject PlaceholderText;

    //Singleton design allows quick and easy global access anywhere in the project code e.g: ChatWindow.Instance.DisplayMessage("easy");
    public static ChatWindow Instance = null;
    private void Awake() { Instance = this; }

    //UI Components used for displaying the chat window
    [SerializeField] private GameObject ChatPanel;  //window used to display message
    [SerializeField] private CustomInputField ChatInput;  //input field used to type a new message

    //Message window stores up to the last 100 messages that were received
    private List<ChatMessage> MessageList = new List<ChatMessage>();
    private int MaxMessageCount = 100;

    //Chat object used to store and display 1 message in the window
    public GameObject ChatMessagePrefab;

    private PlayerCameraController CameraController = null;
    private bool ChatActive = false;
    
    private void Update()
    {
        //Chat window may only be controlled while the player is in the game world
        if (PlayerInfo.GameState != GameStates.PlayingGame)
            return;

        //Assign the players camera controller if we havnt yet, we use this to enable/disable player controls as needed
        if (CameraController == null)
            CameraController = PlayerInfo.PlayerObject.GetComponent<PlayerCharacterController>().CameraTransform.GetComponent<PlayerCameraController>();

        if (ChatActive)
        {//If the user is currently typing a message in the chat window
            //Press enter to finish typing a message
            if(Input.GetKeyDown(KeyCode.Return))
            {
                DeliverMessage();
                StopTyping();
            }
            //Press escape to cancel typing a message
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                ClearMessage();
                StopTyping();
            }
        }
        else
        {//If the chat window is inactive and the player is currently controlling their character, but only if the cursor isnt internally locked by the controller
            //Press enter to start typing a message
            if(Input.GetKeyDown(KeyCode.Return) && !CameraController.IsInternalLocked())
                StartTyping();
        }
    }

    private void StartTyping()
    {
        //Disable the player controls and release the mouse cursor lock
        CameraController.DisableChatCursorLock();
        //Hide the placeholder text, bring focus to the input field and note that the chat window is now active
        PlaceholderText.SetActive(false);
        ChatInput.ActivateInputField();
        ChatActive = true;
    }

    private void StopTyping()
    {
        //Re-enable the player controls and lock the mouse cursor again
        CameraController.EnableChatCursorLock();
        //Display the placeholder text and remove the focus from the input field
        PlaceholderText.SetActive(true);
        ChatInput.DeactivateInputField();
        ChatActive = false;
    }

    private void DeliverMessage()
    {
        //Deliver nothing if the chat box is empty
        if (ChatInput.text == "")
            return;

        //Display this players message to the chat window
        DisplayPlayerMessage(PlayerInfo.CharacterName, ChatInput.text);
        //Send this players message to the server to be shared with all other players
        PacketSender.Instance.SendPlayerMessage(ChatInput.text);
        //Clear the chat input field
        ChatInput.text = "";
    }

    private void ClearMessage()
    {
        ChatInput.text = "";
    }

    //Returns what color a message should be based on its type
    private Color GetMessageColor(ChatMessage.MessageType Type)
    {
        //Set the color to system message by default
        Color MessageColor = SystemMessageColor;

        //Update the color based on the message type
        switch (Type)
        {
            case (ChatMessage.MessageType.PlayerMessage):
                MessageColor = PlayerMessageColor;
                break;
            case (ChatMessage.MessageType.PrivateMessage):
                MessageColor = PrivateMessageColor;
                break;
            case (ChatMessage.MessageType.ClanMessage):
                MessageColor = ClanMessageColor;
                break;
            case (ChatMessage.MessageType.GroupMessage):
                MessageColor = GroupMessageColor;
                break;
            case (ChatMessage.MessageType.SystemMessage):
                MessageColor = SystemMessageColor;
                break;
            case (ChatMessage.MessageType.ErrorMessage):
                MessageColor = ErrorMessageColor;
                break;
        }

        //return the color the message should be
        return MessageColor;
    }

    private void DisplayMessage(string Text, ChatMessage.MessageType Type)
    {
        //If the message list is full make more room
        if (MessageList.Count >= MaxMessageCount)
        {
            //Destroy the last message object and remove it from the list
            Destroy(MessageList[0].Component.gameObject);
            MessageList.Remove(MessageList[0]);
        }

        //Store the chat message in a new content object
        ChatMessage NewMessageContent = new ChatMessage();
        NewMessageContent.Text = Text;
        //Instantiate the UI component used to display the message in the chat window
        GameObject NewMessageObject = Instantiate(ChatMessagePrefab, ChatPanel.transform);
        //Assign the UI component to the content class so it can be accessed when it needs to be destroyed
        NewMessageContent.Component = NewMessageObject.GetComponent<Text>();
        //Update the text component with the new message text
        NewMessageContent.Component.text = NewMessageContent.Text;
        //Set the message text color based on message type
        NewMessageContent.Component.color = GetMessageColor(Type);
        //Store the message with the rest of them
        MessageList.Add(NewMessageContent);
    }
    
    //public functions for other classes to send messages to the chat
    public void DisplayPlayerMessage(string Sender, string Message)
    {//public player chat
        DisplayMessage(Sender + ": " + Message, ChatMessage.MessageType.PlayerMessage);
    }
    
    public void DisplayPrivateMessage(string Sender, string Message)
    {//private player chat
        DisplayMessage(Sender + ": " + Message, ChatMessage.MessageType.PrivateMessage);
    }

    public void DisplayClanMessage(string ClanName, string Message)
    {//clan chat
        DisplayMessage(ClanName + ": " + Message, ChatMessage.MessageType.ClanMessage);
    }

    public void DisplayGroupMessage(string Sender, string Message)
    {//party chat
        DisplayMessage(Sender + ": " + Message, ChatMessage.MessageType.GroupMessage);
    }

    public void DisplaySystemMessage(string Message)
    {//game system message
        DisplayMessage(Message, ChatMessage.MessageType.SystemMessage);
    }

    public void DisplayErrorMessage(string Message)
    {//game system error message
        DisplayMessage(Message, ChatMessage.MessageType.ErrorMessage);
    }

    public void DisplayReplyMessage(bool Success, string Message)
    {//Success = SystemMessage, Failure = ErrorMessage
        if (Success)
            DisplaySystemMessage(Message);
        else
            DisplayErrorMessage(Message);
    }
}