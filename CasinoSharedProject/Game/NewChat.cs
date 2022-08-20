using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

using Classes;
using Microsoft.Xna.Framework.Input;

namespace Casino
{
    class NewChat
    {
        private readonly SpritesStorage storage;
        private SpriteBatch painter;

        private Vector2 position;

        public string PlayerName { get; set; }

        public int ChatMessagesWidth { get; set; }
        public int ChatMessagesHeight { get; set; }

        private StringBuilder messageText;

        public DrawingButton ChatButton { get; set; }
        public bool newMessagesAvialble = false;

        public DrawingButton SendMessageButton { get; set; }

        public DrawingButton MoveChatUpButton { get; set; }

        public DrawingButton MoveChatDownButton { get; set; }

        public DrawingButton MoveChatToLastMessage { get; set; }

        private bool isChatVisible = false;
        public bool IsChatVisible 
        { get { return isChatVisible; } }
        private bool isChatSelfUpdated = true;

        private List<Message> ChatData = new List<Message>(); //chat data is a list of all the messagse,
        //The list ISN'T seperated to sentences.
        private List<String> ChatMessage = new List<string>();//chat message is a list of the the messages,
        //The list IS seperated into sentences.
        private int requestedNumberOfVisibleMessages = 10;
        private int startingMessage = 0;
        private int endingMessage = 0;
        private int lastUsedChatDataMessage = 0;
        private int lastSeenMessage = 10;

        private string lastMessage;
        private bool isNewMessage = false;

        public NewChat(SpritesStorage i_storage, int i_chatMessageWidth, int i_chatMessageHeight)
        {
            storage = i_storage;
            ChatMessagesWidth = i_chatMessageWidth;
            ChatMessagesHeight = i_chatMessageHeight;

            messageText = new StringBuilder();
        }

        public void Load(SpriteBatch i_painter)
        {
            painter = i_painter;

            ChatButton = new DrawingButton(storage.GreenUI[0], storage.Fonts[0]);
            ChatButton.Text = "Chat";
            ChatButton.Click += ChatButton_Click;

            MoveChatUpButton = new DrawingButton(storage.GreenUI[1], storage.Fonts[0]);
            MoveChatUpButton.Click += MoveChatUpButton_Click;

            MoveChatDownButton = new DrawingButton(storage.GreenUI[2], storage.Fonts[0]);
            MoveChatDownButton.Click += MoveChatDownButton_Click;

            MoveChatToLastMessage = new DrawingButton(storage.GreenUI[2], storage.Fonts[0]);
            MoveChatToLastMessage.Click += MoveChatToLastMessage_Click;

            SendMessageButton = new DrawingButton(storage.GreenUI[0], storage.Fonts[0]);
            SendMessageButton.Text = "Send";
            SendMessageButton.Size = new MonoGame.Extended.Size(ChatMessagesWidth, 
                storage.GreenUI[0].Height);
            SendMessageButton.Click += SendMessageButton_Click;

            if (ChatData != null && ChatData.Count > 0)
            {
                endingMessage = ChatData.Count;
                if (endingMessage > requestedNumberOfVisibleMessages)
                {
                    startingMessage = endingMessage - requestedNumberOfVisibleMessages;
                }
                else
                {
                    startingMessage = 0;
                }
            }
        }

        private void MoveChatToLastMessage_Click(object sender, EventArgs e)
        {
            if (ChatMessage.Count >= requestedNumberOfVisibleMessages)
            {
                endingMessage = ChatMessage.Count - 1;
                startingMessage = endingMessage - requestedNumberOfVisibleMessages;
                lastSeenMessage = endingMessage;
            }
        }

        private void SendMessageButton_Click(object sender, EventArgs e)
        {
            sendMessage();
        }

        private void sendMessage()
        {
            if (!string.IsNullOrWhiteSpace(messageText.ToString()))
            {
                lastMessage = messageText.ToString();
                isNewMessage = true;
                messageText.Clear();
            }
        }

        private void MoveChatDownButton_Click(object sender, EventArgs e)
        {
            isChatSelfUpdated = true;

            if (startingMessage + 1 + requestedNumberOfVisibleMessages < ChatMessage.Count)
            {
                startingMessage++;
            }
            
            if(startingMessage + requestedNumberOfVisibleMessages < ChatMessage.Count)
            {
                lastSeenMessage = startingMessage + requestedNumberOfVisibleMessages > 10 ? 
                    startingMessage + requestedNumberOfVisibleMessages: 10;
            }

        }

        private void MoveChatUpButton_Click(object sender, EventArgs e)
        {
            isChatSelfUpdated = false;

            if (startingMessage > 0)
            {
                startingMessage--;
            }
        }

        private void ChatButton_Click(object sender, EventArgs e)
        {
            isChatVisible = !isChatVisible;
            newMessagesAvialble = false;
        }

        public string Update(GameTime i_gameTime, Vector2 i_pos, 
            Keys i_input, bool i_isCapsLockOn, bool i_isShiftOn)
        {
            position.X = i_pos.X;
            position.Y = i_pos.Y;
            string returnMessage = lastMessage;

            if (startingMessage + requestedNumberOfVisibleMessages < ChatMessage.Count)
            {
                endingMessage = startingMessage + requestedNumberOfVisibleMessages;
            }
            else
            {
                endingMessage = startingMessage + (ChatMessage.Count - startingMessage);
            }
            

            if (isChatVisible && isNewMessage)
            {
                isNewMessage = false;
            }
            else
            {
                returnMessage = null;
            }

            if (isChatVisible)
            {
                if (i_input != Keys.None)
                {
                    if (i_input == Keys.Space)
                    {
                        messageText.Append(' ');
                    }
                    else if(i_isShiftOn)
                    {
                        messageText.Append(shiftKeysToString(i_input));
                    }
                    else if (i_input == Keys.Back && messageText.Length > 0)
                    {
                        messageText.Remove(messageText.Length - 1, 1);
                    }
                    else if (isKeyLetter(i_input))
                    {
                        if (i_isCapsLockOn)
                        {
                            messageText.Append(i_input.ToString());
                        }
                        else
                        {
                            messageText.Append(i_input.ToString().ToLower());
                        }
                    }
                    else if (isKeyNumber(i_input))
                    {
                        messageText.Append(numKeyToNumString(i_input));
                    }
                    else if(i_input == Keys.OemQuestion)
                    {
                        messageText.Append(".");
                    }
                    else if(i_input == Keys.OemPlus)
                    {
                        messageText.Append("=");
                    }
                    else if(i_input == Keys.OemMinus)
                    {
                        messageText.Append("-");
                    }
                    else if (i_input == Keys.OemComma)
                    {
                        messageText.Append(",");
                    }
                    else if (i_input == Keys.OemQuotes)
                    {
                        messageText.Append("'");
                    }
                    else if( i_input == Keys.Enter)
                    {
                        sendMessage();
                    }
                }

                if (isChatSelfUpdated && ChatData != null && ChatData.Count > 0)
                {
                    if (endingMessage > requestedNumberOfVisibleMessages)
                    {
                        startingMessage = endingMessage - requestedNumberOfVisibleMessages;
                    }
                    else
                    {
                        startingMessage = 0;
                    }
                }

                StringBuilder text = new StringBuilder();
                for (; ChatData != null && lastUsedChatDataMessage < ChatData.Count; 
                    lastUsedChatDataMessage++)
                {
                    if (lastUsedChatDataMessage == ChatData.Count)
                    {
                        break;
                    }
                    text.Append(ChatData[lastUsedChatDataMessage].UserName);
                    text.Append(": ");
                    if (ChatData[lastUsedChatDataMessage].UserName.Length + 
                        ChatData[lastUsedChatDataMessage].Body.Length < 31)
                    {
                        text.Append(ChatData[lastUsedChatDataMessage].Body);
                        ChatMessage.Add(text.ToString());
                        text.Clear();
                    }
                    else
                    {
                        string[] words = ChatData[lastUsedChatDataMessage].Body.Split(' ');
                        bool isFirstLine = true;
                        int lineCounter = 0;

                        foreach (string word in words)
                        {
                            if (word.Length > 15)
                            {
                                text.Append("...");
                                ChatMessage.Add(text.ToString());
                                text.Clear();
                                break;
                            }

                            lineCounter += word.Length;

                            if (isSentenceInChatBorder(isFirstLine, lineCounter, lastUsedChatDataMessage))
                            {
                                text.Append(word);
                            }
                            else
                            {
                                ChatMessage.Add(text.ToString());
                                text.Clear();
                                lineCounter = 0;
                                lineCounter += word.Length;
                                isFirstLine = false;
                                text.Append(word);
                            }
                            lineCounter++;
                            text.Append(' ');
                        }
                        ChatMessage.Add(text.ToString());
                        text.Clear();
                    }
                }

            }

            return returnMessage;
        }

        private string shiftKeysToString(Keys i_input)
        {
            string returnString = null;

            switch(i_input)
            {
                case Keys.D1:
                    returnString = "!";
                    break;
                case Keys.D2:
                    returnString = "@";
                    break;
                case Keys.D3:
                    returnString = "#";
                    break;
                case Keys.D4:
                    returnString = "$";
                    break;
                case Keys.D5:
                    returnString = "%";
                    break;
                case Keys.D6:
                    returnString = "^";
                    break;
                case Keys.D7:
                    returnString = "&";
                    break;
                case Keys.D8:
                    returnString = "*";
                    break;
                case Keys.D9:
                    returnString = "(";
                    break;
                case Keys.D0:
                    returnString = ")";
                    break;
                case Keys.OemQuestion:
                    returnString = "?";
                    break;
                case Keys.OemPlus:
                    returnString = "+";
                    break;
                default:
                    break;
            }

            return returnString;
        }

        public void Draw(GameTime i_gameTime)
        {
            if (isChatVisible)
            {
                Vector2 drawingTextStringSize;

                painter.Draw(storage.GreenUI[6], new Rectangle((int)SendMessageButton.Position.X, (int)SendMessageButton.Position.Y - 20, ChatMessagesWidth, 20), Color.White);
                painter.Draw(storage.GreenUI[5], new Rectangle((int)SendMessageButton.Position.X, (int)SendMessageButton.Position.Y - 20 - ChatMessagesHeight, ChatMessagesWidth, ChatMessagesHeight), Color.White);

                drawingTextStringSize = storage.Fonts[1].MeasureString(messageText.ToString());
                if(drawingTextStringSize.X < ChatMessagesWidth - 20)
                {
                    painter.DrawString(storage.Fonts[1], messageText.ToString(), new Vector2((int)SendMessageButton.Position.X + 10, (int)SendMessageButton.Position.Y - 20), Color.Black);
                }
                else
                {
                    for (int i = 0; i < messageText.Length; i++)
                    {
                        string checkString = messageText.ToString().Substring(i, messageText.Length - i);
                        if (storage.Fonts[1].MeasureString(checkString).X < ChatMessagesWidth - 20)
                        {
                            painter.DrawString(storage.Fonts[1], checkString, new Vector2((int)SendMessageButton.Position.X + 10, (int)SendMessageButton.Position.Y - 20), Color.Black);
                            break;
                        }
                    }
                }
                
                MoveChatUpButton.Draw(i_gameTime, painter);
                MoveChatDownButton.Draw(i_gameTime, painter);
                if (lastSeenMessage >= ChatMessage.Count - 1)
                {
                    MoveChatToLastMessage.Draw(i_gameTime, painter);
                }
                else 
                {
                    MoveChatToLastMessage.Draw(i_gameTime, painter, Color.Red);
                }
                SendMessageButton.Draw(i_gameTime, painter);

                for (int currentMessage = startingMessage, index = 0; currentMessage <= endingMessage && ChatMessage.Count > 0 && currentMessage < ChatMessage.Count; currentMessage++, index++)
                {
                    painter.DrawString(storage.Fonts[1], ChatMessage[currentMessage], new Vector2((int)SendMessageButton.Position.X + 10, (int)SendMessageButton.Position.Y - 10 - ChatMessagesHeight + (20 * index)), Color.Black);
                }
                
            }
        }

        private bool isSentenceInChatBorder(bool i_isFirstLine, int i_lineCounter, 
            int i_lastUsedChatDataMessage)
        {
            bool isSentenceInChatBorder = false;

            if (i_isFirstLine)
            {
                isSentenceInChatBorder = i_lineCounter < (30 - ChatData[i_lastUsedChatDataMessage].UserName.Length - 2);
            }
            else 
            {
                isSentenceInChatBorder = i_lineCounter < 30;
            }

            return isSentenceInChatBorder;
        }

        private bool isKeyLetter(Keys i_key)
        {
            return (int)i_key >= (int)Keys.A && (int)i_key <= (int)Keys.Z;
        }

        private bool isKeyNumber(Keys i_key)
        {
            return ((int)i_key >= (int)Keys.D0 && (int)i_key <= (int)Keys.D9) 
                || ((int)i_key >= (int)Keys.NumPad0 && (int)i_key <= (int)Keys.NumPad9);
        }

        private string numKeyToNumString(Keys i_numKey)
        {
            string numberString;
            
            if(i_numKey == Keys.NumPad0 || i_numKey == Keys.D0)
            {
                numberString = "0";
            }
            else if (i_numKey == Keys.NumPad1 || i_numKey == Keys.D1)
            {
                numberString = "1";
            }
            else if (i_numKey == Keys.NumPad2 || i_numKey == Keys.D2)
            {
                numberString = "2";
            }
            else if (i_numKey == Keys.NumPad3 || i_numKey == Keys.D3)
            {
                numberString = "3";
            }
            else if (i_numKey == Keys.NumPad4 || i_numKey == Keys.D4)
            {
                numberString = "4";
            }
            else if (i_numKey == Keys.NumPad5 || i_numKey == Keys.D5)
            {
                numberString = "5";
            }
            else if (i_numKey == Keys.NumPad6 || i_numKey == Keys.D6)
            {
                numberString = "6";
            }
            else if (i_numKey == Keys.NumPad7 || i_numKey == Keys.D7)
            {
                numberString = "7";
            }
            else if (i_numKey == Keys.NumPad8 || i_numKey == Keys.D8)
            {
                numberString = "8";
            }
            else
            {
                numberString = "9";
            }

            return numberString;
        }

        public void UpdateMessageList(List<Message> i_chatData)
        {
            if (ChatData.Count != i_chatData.Count)
            {
                ChatData = i_chatData;
                newMessagesAvialble = true;
            }
        }
    }
}