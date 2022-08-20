using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

using Classes;

namespace Casino
{
    public class Player
    {
        private ContentManager contentManager;
        private SpriteBatch painter;

        public SpritesStorage storage;
        public PlayerSkin playerSkin;

        public Stats stats;
        public string PlayerName { get; set; }
        public string playerEmail { get; set; }

        public int totalCoins { get; set; }

        public List<FurnitureInstance> furnituresList { get; set; }

        private AnimationManager animation;

        public Vector2 position;
        public Vector2 LastPosition;
        public Vector2 drawingPosition;

        private const int playerSpeed = 150;

        public Direction direction;
        private bool isMoving;

        public DateTime LastActionTime { get; set; }
        public string LastMessage { get; set; }

        public bool IsUpdate { get; set; } = false;

        private int footSoundEffetSpeed = 5;
        private int footSoundEffetChoise = 0;

        public Chest updateStuckChest;

        public Player(ContentManager i_Content, int i_XPosition, int i_YPosition,
            PlayerSkin i_playerSkin, SpritesStorage i_storage, string i_playerName, string i_playerEmail, Stats i_stats, List<FurnitureInstance> furnituresList)
        {
            contentManager = i_Content;
            this.furnituresList = furnituresList;
            position = new Vector2(i_XPosition / 2 + 2736, i_YPosition / 2 + 1824);
            drawingPosition = new Vector2(position.X - 47, position.Y - 32);
            direction = Direction.Right;
            playerSkin = i_playerSkin;
            storage = i_storage;
            PlayerName = i_playerName;
            playerEmail = i_playerEmail;
            stats = i_stats;
            totalCoins = stats.Money;
        }

        public void Load(SpriteBatch i_Painter)
        {
            painter = i_Painter;
            animation = new AnimationManager(contentManager, painter, playerSkin, storage);
        }

        public void updatePlayer(GameTime i_gameTime, Keys i_input, object i_lock)
        {
            int playerWidth = Game1.listOfSprites[(int)playerSkin].playerWidth;
            int playerHeigth = Game1.listOfSprites[(int)playerSkin].playerHeight;
            IsUpdate = true;
            float dt = (float)i_gameTime.ElapsedGameTime.TotalSeconds;
            isMoving = true;
            FurnitureInstance furniture;
            if (i_input == Keys.Right)
            {
                if ((furniture = checkIfFurnitureOnTheWay(position.X + playerSpeed * dt, position.Y, Direction.Right, i_lock)) == null)
                {
                    position.X += playerSpeed * dt;
                }
                else
                {
                    position.X = furniture.CurrentXPos - (playerWidth + 1) * storage.width;
                }

                direction = Direction.Right;
            }

            else if (i_input == Keys.Left)
            {
                if ((furniture = checkIfFurnitureOnTheWay(position.X - playerSpeed * dt, position.Y, Direction.Left, i_lock)) == null)
                {
                    position.X -= playerSpeed * dt;
                }
                else
                {
                    position.X = furniture.CurrentXPos + furniture.Width + 1;
                }

                direction = Direction.Left;
            }

            else if (i_input == Keys.Up)
            {
                if ((furniture = checkIfFurnitureOnTheWay(position.X, position.Y - playerSpeed * dt, Direction.Up, i_lock)) == null)
                {
                    position.Y -= playerSpeed * dt;
                }
                else
                {
                    position.Y = furniture.CurrentYPos + furniture.Length + 1;
                }
            }

            else if (i_input == Keys.Down)
            {
                if ((furniture = checkIfFurnitureOnTheWay(position.X, position.Y + playerSpeed * dt, Direction.Down, i_lock)) == null)
                {
                    position.Y += playerSpeed * dt;
                }
                else
                {
                    position.Y = furniture.CurrentYPos - (playerHeigth + 1) * storage.width;
                }
            }

            else if (i_input != Keys.Right &&
                i_input != Keys.Left && i_input != Keys.Up
                && i_input != Keys.Down)
            {
                isMoving = false;
            }

            footSoundEffetSpeed++;
            if (isPlayerFootHeard())
            {
                footSoundEffetChoise++;
                if (footSoundEffetChoise == 0)
                {
                    MediaPlayer.Play(storage.RightFootMusic);
                }
                else
                {
                    MediaPlayer.Play(storage.LeftFootMusic);
                }
                footSoundEffetChoise %= 2;
            }

            footSoundEffetSpeed %= 15;

            animation.UpdateAnimation(direction, isMoving);
        }

        private bool isPlayerFootHeard()
        {
            bool isPLayerFootHeard = isMoving && footSoundEffetSpeed % 15 == 0;
            if (MediaPlayer.State == MediaState.Playing)
            {
                if (MediaPlayer.Queue.ActiveSong != storage.LeftFootMusic && MediaPlayer.Queue.ActiveSong != storage.RightFootMusic)
                {
                    isPLayerFootHeard &= false;
                }
                else
                {
                    isPLayerFootHeard &= true;
                }
            }

            return isPLayerFootHeard;
        }

        private FurnitureInstance checkIfFurnitureOnTheWay(float i_RelevantPositionX, float i_RelevantPositionY, Direction i_Direction, object i_lock)
        {
            FurnitureInstance furniture = new FurnitureInstance();
            foreach (FurnitureInstance Furniture in furnituresList)
            {
                furniture = checkIfSpecificFurnitureOnTheWay(Furniture, i_RelevantPositionX, i_RelevantPositionY, i_Direction);
                if (furniture != null)
                {
                    return furniture;
                }
            }
            furniture = checkIfWallAhead(i_RelevantPositionX, i_RelevantPositionY, i_Direction);
            if (furniture == null && !updateStuckChest.Collected)
            {
                furniture = checkIfSpecificFurnitureOnTheWay(updateStuckChest, i_RelevantPositionX, i_RelevantPositionY, i_Direction);
            }
            return furniture;
        }

        private FurnitureInstance checkIfSpecificFurnitureOnTheWay(FurnitureInstance i_Furniture, float i_RelevantPositionX, float i_RelevantPositionY, Direction i_Direction)
        {
            FurnitureInstance furniture = new FurnitureInstance();
            int leftSide;
            int upSide;
            int rightSide;
            int downSide;

            int playerWidth = Game1.listOfSprites[(int)playerSkin].playerWidth;
            int playerHeigth = Game1.listOfSprites[(int)playerSkin].playerHeight;

            calculateFurnitureSize(i_Furniture, out leftSide, out rightSide, out upSide, out downSide);

            furniture.CurrentXPos = leftSide;
            furniture.CurrentYPos = upSide;
            furniture.Width = rightSide - leftSide;
            furniture.Length = downSide - upSide;


            switch (i_Direction)
            {
                case Direction.Up:
                    {
                        if ((i_RelevantPositionY >= upSide && i_RelevantPositionY <= downSide && i_RelevantPositionX >= leftSide && i_RelevantPositionX <= rightSide)
                            || (i_RelevantPositionY >= upSide && i_RelevantPositionY <= downSide && i_RelevantPositionX + 0.25 * playerWidth * storage.width >= leftSide && i_RelevantPositionX + 0.25 * playerWidth * storage.width <= rightSide)
                            || (i_RelevantPositionY >= upSide && i_RelevantPositionY <= downSide && i_RelevantPositionX + 0.50 * playerWidth * storage.width >= leftSide && i_RelevantPositionX + 0.50 * playerWidth * storage.width <= rightSide)
                            || (i_RelevantPositionY >= upSide && i_RelevantPositionY <= downSide && i_RelevantPositionX + 0.75 * playerWidth * storage.width >= leftSide && i_RelevantPositionX + 0.75 * playerWidth * storage.width <= rightSide)
                            || (i_RelevantPositionY >= upSide && i_RelevantPositionY <= downSide && i_RelevantPositionX + 0.75 * playerWidth * storage.width >= leftSide && i_RelevantPositionX + playerWidth * storage.width <= rightSide))
                        {
                            return furniture;
                        }
                        break;
                    }
                case Direction.Down:
                    {
                        if ((i_RelevantPositionY + playerHeigth * storage.heigth >= upSide && i_RelevantPositionY + playerHeigth * storage.heigth <= downSide && i_RelevantPositionX >= leftSide && i_RelevantPositionX <= rightSide)
                        || (i_RelevantPositionY + playerHeigth * storage.heigth >= upSide && i_RelevantPositionY + playerHeigth * storage.heigth <= downSide && i_RelevantPositionX + 0.25 * playerWidth * storage.width >= leftSide && i_RelevantPositionX + 0.25 * playerWidth * storage.width <= rightSide)
                        || (i_RelevantPositionY + playerHeigth * storage.heigth >= upSide && i_RelevantPositionY + playerHeigth * storage.heigth <= downSide && i_RelevantPositionX + 0.50 * playerWidth * storage.width >= leftSide && i_RelevantPositionX + 0.50 * playerWidth * storage.width <= rightSide)
                        || (i_RelevantPositionY + playerHeigth * storage.heigth >= upSide && i_RelevantPositionY + playerHeigth * storage.heigth <= downSide && i_RelevantPositionX + 0.75 * playerWidth * storage.width >= leftSide && i_RelevantPositionX + 0.75 * playerWidth * storage.width <= rightSide)
                        || (i_RelevantPositionY + playerHeigth * storage.heigth >= upSide && i_RelevantPositionY + playerHeigth * storage.heigth <= downSide && i_RelevantPositionX + 0.25 * playerWidth * storage.width >= leftSide && i_RelevantPositionX + playerWidth * storage.width <= rightSide))
                        {
                            return furniture;
                        }
                        break;
                    }
                case Direction.Left:
                    {
                        if ((i_RelevantPositionY >= upSide && i_RelevantPositionY <= downSide && i_RelevantPositionX >= leftSide && i_RelevantPositionX <= rightSide)
                            || (i_RelevantPositionY + 0.125 * playerHeigth * storage.heigth >= upSide && i_RelevantPositionY + 0.125 * playerHeigth * storage.heigth <= downSide && i_RelevantPositionX >= leftSide && i_RelevantPositionX <= rightSide)
                            || (i_RelevantPositionY + 0.375 * playerHeigth * storage.heigth >= upSide && i_RelevantPositionY + 0.375 * playerHeigth * storage.heigth <= downSide && i_RelevantPositionX >= leftSide && i_RelevantPositionX <= rightSide)
                            || (i_RelevantPositionY + 0.625 * playerHeigth * storage.heigth >= upSide && i_RelevantPositionY + 0.625 * playerHeigth * storage.heigth <= downSide && i_RelevantPositionX >= leftSide && i_RelevantPositionX <= rightSide)
                            || (i_RelevantPositionY + 0.25 * playerHeigth * storage.heigth >= upSide && i_RelevantPositionY + 0.25 * playerHeigth * storage.heigth <= downSide && i_RelevantPositionX >= leftSide && i_RelevantPositionX <= rightSide)
                            || (i_RelevantPositionY + 0.5 * playerHeigth * storage.heigth >= upSide && i_RelevantPositionY + 0.5 * playerHeigth * storage.heigth <= downSide && i_RelevantPositionX >= leftSide && i_RelevantPositionX <= rightSide)
                            || (i_RelevantPositionY + 0.75 * playerHeigth * storage.heigth >= upSide && i_RelevantPositionY + 0.75 * playerHeigth * storage.heigth <= downSide && i_RelevantPositionX >= leftSide && i_RelevantPositionX <= rightSide)
                             || (i_RelevantPositionY + playerHeigth * storage.heigth >= upSide && i_RelevantPositionY + playerHeigth * storage.heigth <= downSide && i_RelevantPositionX >= leftSide && i_RelevantPositionX <= rightSide))
                        {
                            return furniture;
                        }
                        break;
                    }
                case Direction.Right:
                    {
                        if ((i_RelevantPositionY >= upSide && i_RelevantPositionY <= downSide && i_RelevantPositionX + playerWidth * storage.width >= leftSide && i_RelevantPositionX + playerWidth * storage.width <= rightSide)
                            || (i_RelevantPositionY + 0.125 * playerHeigth * storage.heigth >= upSide && i_RelevantPositionY + 0.125 * playerHeigth * storage.heigth <= downSide && i_RelevantPositionX + playerWidth * storage.width >= leftSide && i_RelevantPositionX + 75 * storage.width <= rightSide)
                            || (i_RelevantPositionY + 0.375 * playerHeigth * storage.heigth >= upSide && i_RelevantPositionY + 0.375 * playerHeigth * storage.heigth <= downSide && i_RelevantPositionX + playerWidth * storage.width >= leftSide && i_RelevantPositionX + 75 * storage.width <= rightSide)
                            || (i_RelevantPositionY + 0.625 * playerHeigth * storage.heigth >= upSide && i_RelevantPositionY + 0.625 * playerHeigth * storage.heigth <= downSide && i_RelevantPositionX + playerWidth * storage.width >= leftSide && i_RelevantPositionX + 75 * storage.width <= rightSide)
                            || (i_RelevantPositionY + 0.875 * playerHeigth * storage.heigth >= upSide && i_RelevantPositionY + 0.875 * playerHeigth * storage.heigth <= downSide && i_RelevantPositionX + playerWidth * storage.width >= leftSide && i_RelevantPositionX + 75 * storage.width <= rightSide)
                            || (i_RelevantPositionY + 0.25 * playerHeigth * storage.heigth >= upSide && i_RelevantPositionY + 0.25 * playerHeigth * storage.heigth <= downSide && i_RelevantPositionX + playerWidth * storage.width >= leftSide && i_RelevantPositionX + 75 * storage.width <= rightSide)
                            || (i_RelevantPositionY + 0.5 * playerHeigth * storage.heigth >= upSide && i_RelevantPositionY + 0.5 * playerHeigth * storage.heigth <= downSide && i_RelevantPositionX + playerWidth * storage.width >= leftSide && i_RelevantPositionX + 75 * storage.width <= rightSide)
                            || (i_RelevantPositionY + 0.75 * playerHeigth * storage.heigth >= upSide && i_RelevantPositionY + 0.75 * playerHeigth * storage.heigth <= downSide && i_RelevantPositionX + playerWidth * storage.width >= leftSide && i_RelevantPositionX + 75 * storage.width <= rightSide)

                            || (i_RelevantPositionY + playerHeigth * storage.heigth >= upSide && i_RelevantPositionY + playerHeigth * storage.heigth <= downSide && i_RelevantPositionX + playerWidth * storage.width >= leftSide && i_RelevantPositionX + 75 * storage.width <= rightSide))
                        {
                            return furniture;
                        }
                        break;
                    }
            }
            return null;
        }

        private FurnitureInstance checkIfWallAhead(float i_RelevantPositionX, float i_RelevantPositionY, Direction i_Direction)
        {
            FurnitureInstance furniture = new FurnitureInstance();
            int left = 2096 + storage.Furnitures[5].Width;
            int up = 1464 + storage.Furnitures[5].Height;
            int right = furniture.Width = left + 2560 - storage.Furnitures[5].Width;
            int down = furniture.Length = up + 1440 - storage.Furnitures[5].Height;
            int playerWidth = Game1.listOfSprites[(int)playerSkin].playerWidth;
            int playerHeigth = Game1.listOfSprites[(int)playerSkin].playerHeight;
            switch (i_Direction)
            {
                case Direction.Up:
                    {
                        if (i_RelevantPositionY <= up - 30)
                        {
                            furniture.CurrentXPos = 0;
                            furniture.CurrentYPos = up - 30;
                            furniture.Width = 0;
                            furniture.Length = 0;
                            return furniture;
                        }
                        break;
                    }
                case Direction.Down:
                    {
                        if (i_RelevantPositionY + playerHeigth >= down + 30)
                        {
                            furniture.CurrentXPos = 0;
                            furniture.CurrentYPos = down + 30;
                            furniture.Width = 0;
                            furniture.Length = 0;
                            return furniture;
                        }
                        break;
                    }
                case Direction.Left:
                    {
                        if (i_RelevantPositionX <= left + 15)
                        {
                            furniture.CurrentXPos = left + 15;
                            furniture.CurrentYPos = 0;
                            furniture.Width = 0;
                            furniture.Length = 0;
                            return furniture;
                        }
                        break;
                    }
                case Direction.Right:
                    {
                        if (i_RelevantPositionX >= right)
                        {
                            furniture.CurrentXPos = right + playerWidth;
                            furniture.CurrentYPos = 0;
                            furniture.Width = 0;
                            furniture.Length = 0;
                            return furniture;
                        }
                        break;
                    }
            }
            return null;
        }

        private bool twoPartFurniture(int type)
        {
            return type == 0 || type == 4 || (type >= 9 && type <= 12);
        }

        public void calculateFurnitureSize(FurnitureInstance furniture, out int leftSide, out int rightSide, out int upSide, out int downSide)
        {
            leftSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 0.05);
            rightSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 0.95);
            upSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.125);
            downSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.875);
            switch (furniture.Type)
            {
                case 0:
                    {
                        leftSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 0.3);
                        rightSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 0.525);
                        upSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.5);
                        downSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.75);
                        break;
                    }
                case 1:
                    {
                        leftSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 0.6);
                        rightSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 0.65);
                        upSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.75);
                        downSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.9);
                        break;
                    }
                case 2:
                    {
                        leftSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 0.87);
                        rightSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 1.07);
                        upSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.55);
                        downSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.75);
                        break;
                    }
                case 3:
                    {
                        leftSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 0.35);
                        rightSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 1.05);
                        upSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.35);
                        downSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 1.1);
                        break;
                    }
                case 4:
                    {
                        leftSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 0.25);
                        rightSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 0.525);
                        upSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.6);
                        downSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.85);
                        break;
                    }
                case 5:
                    {
                        leftSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 0.35);
                        rightSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 1.05);
                        upSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.35);
                        downSide = (int)(furniture.CurrentYPos + (float)furniture.Length);
                        break;
                    }
                case 6:
                    {
                        leftSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 0.35);
                        rightSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 1.05);
                        upSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.35);
                        downSide = (int)(furniture.CurrentYPos + (float)furniture.Length);
                        break;
                    }
                case 7:
                    {
                        leftSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 0.35);
                        rightSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 1.05);
                        upSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.35);
                        downSide = (int)(furniture.CurrentYPos + (float)furniture.Length);
                        break;
                    }
                case 8:
                    {
                        leftSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 0.75);
                        rightSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 1.1);
                        upSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.45);
                        downSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.85);
                        break;
                    }
                case 9:
                    {
                        leftSide = (int)(furniture.CurrentXPos);
                        rightSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 0.52);
                        upSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.2);
                        downSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.5);
                        break;
                    }
                case 10:
                    {
                        leftSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 0.35);
                        rightSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 1.05);
                        upSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.95);
                        downSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 1.3);
                        break;
                    }
                case 11:
                    {
                        leftSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 0.9);
                        rightSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 1.55);
                        upSide = (int)(furniture.CurrentYPos - (float)furniture.Length * 0.2);
                        downSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.45);
                        break;
                    }
                case 12:
                    {
                        leftSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 0.35);
                        rightSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 1.05);
                        upSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.85);
                        downSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 1.2);
                        break;
                    }
                case 13:
                    {
                        leftSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 0.65);
                        rightSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 1.2);
                        upSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.18);
                        downSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.8);
                        break;
                    }
                case 14:
                    {
                        leftSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 0.65);
                        rightSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 1.2);
                        upSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.18);
                        downSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.8);
                        break;
                    }
                case 15:
                    {
                        leftSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 0.35);
                        rightSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 1.1);
                        upSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.25);
                        downSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.6);
                        break;
                    }
                case 16:
                    {
                        leftSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 0.38);
                        rightSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 1.09);
                        upSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.52);
                        downSide = (int)(furniture.CurrentYPos + (float)furniture.Length);
                        break;
                    }
                case 17:
                    {
                        leftSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 0.3);
                        rightSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 1.1);
                        upSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.58);
                        downSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.85);
                        break;
                    }
                case 18:
                    {
                        leftSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 0.35);
                        rightSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 1.1);
                        upSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.35);
                        downSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.65);
                        break;
                    }
                case 19:
                    {
                        leftSide = (int)(furniture.CurrentXPos);
                        rightSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 1.56);
                        upSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.35);
                        downSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.7);
                        break;
                    }
                case 20:
                    {
                        leftSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 0.35);
                        rightSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 1.05);
                        upSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.35);
                        downSide = (int)(furniture.CurrentYPos + (float)furniture.Length);
                        break;
                    }
                case 21:
                    {
                        leftSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 0.35);
                        rightSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 1.05);
                        upSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.35);
                        downSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.57);
                        break;
                    }
                case 22:
                    {
                        leftSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 0.95);
                        rightSide = (int)(furniture.CurrentXPos + (float)furniture.Width * 1.75);
                        upSide = (int)(furniture.CurrentYPos + (float)furniture.Length * 0.35);
                        downSide = (int)(furniture.CurrentYPos + (float)furniture.Length);
                        break;
                    }

            }
        }

        public void DrawBubble(string i_lastMessage)
        {
            LastMessage = i_lastMessage;
        }

        public void drawPlayer()
        {
            if (IsUpdate)
            {
                drawingPosition.X = position.X - 47;
                drawingPosition.Y = position.Y - 32;
                //animation.DrawAnimation(drawingPosition, direction, 75, 100);

                if (DateTime.Now.Subtract(LastActionTime).TotalSeconds <= 3)
                {
                    animation.DrawAnimation(drawingPosition, LastMessage, direction);
                    //Vector2 bubblePosition = new Vector2(-210, -240) + drawingPosition; // acording to defualt bubble size(300,300)

                    ////if(LastMessage.Length < 20)
                    //if (lengthOfLastMessage(LastMessage) < 20)
                    //{
                    //    painter.Draw(storage.SpeachBubble, new Rectangle((int)bubblePosition.X + 80, (int)bubblePosition.Y + 80, 200, 200), Color.White);
                    //    painter.DrawString(storage.Fonts[2], createBubbleString(LastMessage, 1), new Vector2(120, 115) + bubblePosition, Color.Black);
                    //}
                    //else if (lengthOfLastMessage(LastMessage) < 50)
                    //{
                    //    painter.Draw(storage.SpeachBubble, new Rectangle((int)bubblePosition.X + 80, (int)bubblePosition.Y + 80, 200, 200), Color.White);
                    //    painter.DrawString(storage.Fonts[3], createBubbleString(LastMessage, 2), new Vector2(120, 115) + bubblePosition, Color.Black);
                    //}
                    //else
                    //{
                    //    painter.Draw(storage.SpeachBubble, new Rectangle((int)bubblePosition.X + 80, (int)bubblePosition.Y + 80, 200, 200), Color.White);
                    //    painter.DrawString(storage.Fonts[4], createBubbleString(LastMessage, 3), new Vector2(110, 105) + bubblePosition, Color.Black);
                    //}
                }
                else
                {
                    animation.DrawAnimation(drawingPosition, null, direction);
                }
            }
            IsUpdate = false;
        }

        //private int lengthOfLastMessage(string i_lastMessage)
        //{
        //    int lengthOfLastMessage = i_lastMessage.Length;
        //    int countBigSymbols = 0;

        //    foreach (char letter in i_lastMessage)
        //    {
        //        if (letter == '@' || letter == '#' || letter == '$' || letter == '%' || letter == '&')
        //        {
        //            countBigSymbols++;
        //        }
        //    }

        //    lengthOfLastMessage += countBigSymbols;

        //    return lengthOfLastMessage;
        //}

        //private string createBubbleString(string i_message, int i_fontSize)
        //{
        //    StringBuilder bubbleString = new StringBuilder();

        //    string[] words = i_message.Split(' ');

        //    int lineCounter = 0;
        //    foreach (string word in words)
        //    {
        //        bubbleString.Append(word);
        //        lineCounter += word.Length;

        //        if (lineCounter > 7 && i_fontSize == 1)
        //        {
        //            bubbleString.Append("\n");
        //            lineCounter = 0;
        //        }
        //        else if (lineCounter > 9 && i_fontSize == 2)
        //        {
        //            bubbleString.Append("\n");
        //            lineCounter = 0;
        //        }
        //        else if (lineCounter > 15 && i_fontSize == 3)
        //        {
        //            bubbleString.Append("\n");
        //            lineCounter = 0;
        //        }
        //        else
        //        {
        //            lineCounter++;
        //            bubbleString.Append(' ');
        //        }

        //    }

        //    return bubbleString.ToString();
        //}

        public void UpdatePlayerSkin(PlayerSkin i_PlayerSkin)
        {
            playerSkin = i_PlayerSkin;
            animation.UpdateSkinType(i_PlayerSkin);
        }
    }
}