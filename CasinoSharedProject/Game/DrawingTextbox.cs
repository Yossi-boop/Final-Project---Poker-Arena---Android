using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Casino
{
    public class DrawingTextbox
    {
        private int width;
        private int height;

        private Texture2D texture;
        private SpriteFont font;

        public Vector2 Position { get; set; }

        private StringBuilder messageText;

        public string message;

        public DrawingTextbox(int i_width, int i_height, Texture2D i_texture2D, SpriteFont i_font)
        {
            width = i_width;
            height = i_height;
            texture = i_texture2D;
            font = i_font;

            messageText = new StringBuilder();
        }

        public void Update(Keys i_input, Vector2 i_givenPosition)
        {
            Position = i_givenPosition;

            if (i_input != Keys.None)
            {
                if (isKeyNumber(i_input))
                {
                    messageText.Append(numKeyToNumString(i_input));
                }
                else if (i_input == Keys.Back && messageText.Length > 0)
                {
                    messageText.Remove(messageText.Length - 1, 1);
                }
            }

            message = messageText.ToString();
        }

        private string numKeyToNumString(Keys i_numKey)
        {
            string numberString = null;
            if (i_numKey == Keys.D0 || i_numKey == Keys.NumPad0)
            {
                numberString = "0";
            }
            else if(i_numKey == Keys.D1 || i_numKey == Keys.NumPad1)
            {
                numberString = "1";
            }
            else if (i_numKey == Keys.D2 || i_numKey == Keys.NumPad2)
            {
                numberString = "2";
            }
            else if (i_numKey == Keys.D3 || i_numKey == Keys.NumPad3)
            {
                numberString = "3";
            }
            else if (i_numKey == Keys.D4 || i_numKey == Keys.NumPad4)
            {
                numberString = "4";
            }
            else if (i_numKey == Keys.D5 || i_numKey == Keys.NumPad5)
            {
                numberString = "5";
            }
            else if (i_numKey == Keys.D6 || i_numKey == Keys.NumPad6)
            {
                numberString = "6";
            }
            else if (i_numKey == Keys.D7 || i_numKey == Keys.NumPad7)
            {
                numberString = "7";
            }
            else if (i_numKey == Keys.D8 || i_numKey == Keys.NumPad8)
            {
                numberString = "8";
            }
            else if (i_numKey == Keys.D9 || i_numKey == Keys.NumPad9)
            {
                numberString = "9";
            }

            return numberString;
        }

        private bool isKeyNumber(Keys i_key)
        {
            return ((int)i_key >= (int)Keys.D0 && (int)i_key <= (int)Keys.D9)
                || ((int)i_key >= (int)Keys.NumPad0 && (int)i_key <= (int)Keys.NumPad9);
        }

        public void Draw(SpriteBatch i_painter)
        {
            i_painter.Draw(texture, new Rectangle((int)Position.X, (int)Position.Y, width, height), Color.White);
            i_painter.DrawString(font, message, new Vector2((int)Position.X + 10, (int)Position.Y + 15), Color.Black);
        }

        public void updateMessageExtern(string i_givenInput)
        {
            messageText.Clear();
            messageText.Append(i_givenInput);
            message = i_givenInput;
        }
    }
}
