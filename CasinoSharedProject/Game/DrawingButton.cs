using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Input.Touch;

namespace Casino
{
    public class DrawingButton
    {
        #region Fields

        private bool _currentTouch;

        private readonly SpriteFont _font;

        private bool _isHovering;

        private bool _previousTouch;

        private readonly Texture2D _texture;

        #endregion

        #region Properties

        public bool IsVisible { get; set; } = true;

        public bool IsEnabled { get; set; } = true;

        public event EventHandler Click;

        public bool Clicked { get; private set; }

        public Color PenColour { get; set; }

        public Vector2 Position { get; set; }

        public string Name { get; set; }

        public Size Size { get; set; } = new Size(-1, -1);

        public Rectangle Rectangle
        {
            get
            {
                if (Size.Width == -1 && Size.Height == -1)
                {
                    return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);
                }
                else
                {
                    return new Rectangle((int)Position.X, (int)Position.Y, Size.Width, Size.Height);
                }
            }
        }

        public string Text { get; set; }

        #endregion

        #region Methods

        public DrawingButton(Texture2D texture, SpriteFont font)
        {
            _texture = texture;

            _font = font;

            PenColour = Color.Black;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (IsVisible)
            {
                var colour = Color.White;

                if (_isHovering)
                    colour = Color.Gray;

                spriteBatch.Draw(_texture, Rectangle, colour);

                if (!string.IsNullOrEmpty(Text))
                {
                    var x = (Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(Text).X / 2);
                    var y = (Rectangle.Y + (Rectangle.Height / 2)) - (_font.MeasureString(Text).Y / 2);

                    spriteBatch.DrawString(_font, Text, new Vector2(x, y), PenColour);
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Color i_buttonColor)
        {
            if (IsVisible)
            {
                var colour = i_buttonColor;

                if (_isHovering)
                    colour = Color.Gray;

                spriteBatch.Draw(_texture, Rectangle, colour);

                if (!string.IsNullOrEmpty(Text))
                {
                    var x = (Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(Text).X / 2);
                    var y = (Rectangle.Y + (Rectangle.Height / 2)) - (_font.MeasureString(Text).Y / 2);

                    spriteBatch.DrawString(_font, Text, new Vector2(x, y), PenColour);
                }
            }
        }

        public void Update(GameTime gameTime, int width, int height)
        {
        }

        public void Update(GameTime gameTime, int width, int height, int xPostion, int yPostion)
        {
            if (IsEnabled)
            {
                _previousTouch = _currentTouch;

                var touchRectangle = new Rectangle(xPostion + width, yPostion + height,
                    1, 1);
                if (touchRectangle.Intersects(Rectangle))
                {
                    _isHovering = true;
                    Clicked = true;
                    _currentTouch = true;
                }
                else
                {
                    _currentTouch = false;
                    Clicked = false;
                    _isHovering = false;
                }
                if (_currentTouch == false && _previousTouch == true)
                {
                    Click?.Invoke(this, new EventArgs());
                }
            }
        }

        #endregion
    }
}
