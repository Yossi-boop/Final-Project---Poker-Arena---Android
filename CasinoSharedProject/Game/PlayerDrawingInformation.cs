using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

using Classes;
using System.Diagnostics.CodeAnalysis;

namespace Casino
{
    public class PlayerDrawingInformation:Instance
    {
        private Vector2 position;
        private Vector2 nextPosition;
        public Direction direction;
        private PlayerSkin playerSkin;
        private string lastMessage;
        private AnimationManager animation;
        private const int playerSpeed = 150; //to make player move
        public int CurrentXPos { get; set; }
        public int CurrentYPos { get; set; }

        public PlayerDrawingInformation(ContentManager i_ContentManager, SpriteBatch i_Painter,
           SpritesStorage i_Storage, Vector2 i_Position, Vector2 i_NextPosition,
           Direction i_Direction, PlayerSkin i_PlayerSkin, string i_lastMessage)
        {
            position = i_Position;
            nextPosition = i_NextPosition;
            direction = i_Direction;
            playerSkin = i_PlayerSkin;
            lastMessage = i_lastMessage;
            CurrentXPos = (int)position.X;
            CurrentYPos = (int)position.Y;
            animation = new AnimationManager(i_ContentManager, i_Painter, playerSkin, i_Storage);
        }

        public PlayerDrawingInformation(CharacterInstance i_character, ContentManager i_contentManager, SpriteBatch i_painter, SpritesStorage i_storage)
        {
            position = new Vector2(i_character.LastXPos, i_character.LastYPos);
            nextPosition = new Vector2(i_character.CurrentXPos, i_character.CurrentYPos);
            direction = (Direction)i_character.Direction;
            playerSkin = (PlayerSkin)i_character.Skin;
            lastMessage = i_character.LastMessage;
            CurrentXPos = (int)position.X;
            CurrentYPos = (int)position.Y;
            animation = new AnimationManager(i_contentManager, i_painter, playerSkin, i_storage);
        }

        public void updateOnlinePlayer(GameTime i_GameTime, Direction i_Direction)
        {
            bool isMoving = false;
            float dt = (float)i_GameTime.ElapsedGameTime.TotalSeconds;
            if (nextPosition != position)
            {
                isMoving = true;

                if (position.X + dt * playerSpeed <= nextPosition.X)
                {
                    position.X += dt * playerSpeed;
                }
                else if (position.X - dt * playerSpeed >= nextPosition.X)
                {
                    position.X -= dt * playerSpeed;
                }
                else if (position.Y + dt * playerSpeed <= nextPosition.Y)
                {
                    position.Y += dt * playerSpeed;
                }
                else if (position.Y - dt * playerSpeed >= nextPosition.Y)
                {
                    position.Y -= dt * playerSpeed;
                }
                else
                {
                    position = nextPosition;
                }

            }

            CurrentXPos = (int)position.X;
            CurrentYPos = (int)position.Y;

            animation.UpdateAnimation(i_Direction, isMoving);
        }

        public void drawOnlinePlayer(SpriteBatch i_Painter)
        {
            //animation.DrawAnimation(position, lastMessage, direction, 75, 100);
            animation.DrawAnimation(position, lastMessage, direction);
        }

        public int CompareTo(Instance other)
        {
            if (CurrentYPos == other.CurrentYPos)
            {
                return CurrentXPos - other.CurrentXPos;
            }
            return CurrentYPos - other.CurrentYPos;
        }
    }

    public enum PlayerSkin
    {
        Ninja,
        Jack,
        Zombie
    }

    public enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    public enum FurnitureType
    {
        None,
        PokerTable,
        Flowerpot,
        Picture
    }
}