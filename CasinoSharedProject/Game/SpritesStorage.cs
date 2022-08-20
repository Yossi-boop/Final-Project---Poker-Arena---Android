using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

using MonoGame.Extended.TextureAtlases;

namespace Casino
{
    public class SpritesStorage
    {
        private ContentManager contentManager;

        public List<SpriteFont> Fonts { get; set; } = new List<SpriteFont>();

        public List<Texture2D> Furnitures;

        public Texture2D LoginPage { get; set; }
        public TextureRegion2D LoginPageBackground { get; set; }


        private StringBuilder textureName;

        public Texture2D[][] playersSkinInRest;
        public List<List<List<Texture2D>>> playerSkinInMovement;
        public List<List<Texture2D>> PlayerFaces { get; set; }

        public List<Texture2D> PokerSigns { get; set; } = new List<Texture2D>();

        public List<Texture2D> GreenUI { get; set; } = new List<Texture2D>();

        public List<Texture2D> GreyUI { get; set; } = new List<Texture2D>();

        public List<Texture2D> Coins { get; set; } = new List<Texture2D>();

        public Texture2D HandsRating { get; set; }

        public Texture2D BlueRoomBackground { get; set; }

        public List<Texture2D> PokerChips { get; set; } = new List<Texture2D>();

        public Texture2D Background { get; set; }
        public TextureRegion2D PokerBackGround { get; set; }

        public Texture2D SpeachBubble { get; set; }

        public List<Texture2D> Cards { get; set; } = new List<Texture2D>(53);

        public Texture2D CurrentPlayerMark { get; set; }

        #region Music Storage
        public Song BackgroundMusic { get; set; }
        public Song CoinsMusic { get; set; }
        public Song CoinsMusic2 { get; set; }
        public Song RightFootMusic { get; set; }
        public Song LeftFootMusic { get; set; }
        public Song SnokerSound { get; set; }
        public Song RouletteWheelSound { get; set; }
        public Song SlotMachineSound { get; set; }
        public Song TreesSound { get; set; }
        public Song StatueSound { get; set; }
        public Song ChestSound { get; set; }
        public Song PokerCardSound { get; set; }
        #endregion
        public float heigth { get; set; }
        public float width { get; set; }

        public SpritesStorage(ContentManager i_ContentManager, int i_Heigth, int i_Width)
        {
            int numberOfSkins = Enum.GetNames(typeof(PlayerSkin)).Length;
            int numberOfFurnituers = Enum.GetNames(typeof(FurnitureType)).Length;
            contentManager = i_ContentManager;
            Furnitures = new List<Texture2D>(numberOfFurnituers);
            playersSkinInRest = new Texture2D[numberOfSkins][];
            playerSkinInMovement = new List<List<List<Texture2D>>>(numberOfSkins);
            for (int i = 0; i < numberOfSkins; i++)
            {
                playersSkinInRest[i] = new Texture2D[2];
                playerSkinInMovement.Add(new List<List<Texture2D>>(2));
            }
            for (int i = 0; i < numberOfSkins; i++) 
            {
                for (int j = 0; j < 2; j++)
                {
                    playerSkinInMovement[i].Add(new List<Texture2D>());
                }
            }

            textureName = new StringBuilder();

            heigth = (float)i_Heigth / 720;
            width = (float)i_Width / 1280;
        }

        public void Load()
        {
            #region Music Loads
            TreesSound = contentManager.Load<Song>("Sounds/TreesMusic2new");
            StatueSound = contentManager.Load<Song>("Sounds/Statue");
            SlotMachineSound = contentManager.Load<Song>("Sounds/SlotMachine");
            RouletteWheelSound = contentManager.Load<Song>("Sounds/RouletteWheel2new");
            SnokerSound = contentManager.Load<Song>("Sounds/Billiard");
            RightFootMusic = contentManager.Load<Song>("Sounds/RightFootMusic");
            LeftFootMusic = contentManager.Load<Song>("Sounds/LeftFootMusic");
            CoinsMusic = contentManager.Load<Song>("Sounds/CoinsMusic2new");
            CoinsMusic2 = contentManager.Load<Song>("Sounds/CoinsMusic3new");
            ChestSound = contentManager.Load<Song>("Sounds/ChestMusicnew");
            PokerCardSound = contentManager.Load<Song>("Sounds/PokerCardMusicnew");
            BackgroundMusic = contentManager.Load<Song>("Sounds/BackgroundMusic");
            #endregion
            var d = contentManager.RootDirectory;
            CurrentPlayerMark = contentManager.Load<Texture2D>("CurrentPlayerMark");

            Coins.Add(contentManager.Load<Texture2D>("Coins/Coin1"));//0
            Coins.Add(contentManager.Load<Texture2D>("Coins/Coin2"));//1
            Coins.Add(contentManager.Load<Texture2D>("Coins/Coin3"));//2
            Coins.Add(contentManager.Load<Texture2D>("Coins/Coin4"));//3
            Coins.Add(contentManager.Load<Texture2D>("Coins/Coin5"));//4

            HandsRating = contentManager.Load<Texture2D>("HandsRating");

            PokerSigns.Add(contentManager.Load<Texture2D>("PokerSigns/Check"));//0
            PokerSigns.Add(contentManager.Load<Texture2D>("PokerSigns/Call"));//1
            PokerSigns.Add(contentManager.Load<Texture2D>("PokerSigns/Raise"));//2
            PokerSigns.Add(contentManager.Load<Texture2D>("PokerSigns/Fold"));//3
            PokerSigns.Add(contentManager.Load<Texture2D>("PokerChips/SmallBlind"));//4
            PokerSigns.Add(contentManager.Load<Texture2D>("PokerChips/BigBlind"));//5

            GreenUI.Add(contentManager.Load<Texture2D>("GreenUI/GreenButtonGeneral"));//0
            GreenUI.Add(contentManager.Load<Texture2D>("GreenUI/GreenButtonUp"));//1
            GreenUI.Add(contentManager.Load<Texture2D>("GreenUI/GreenButtonDown"));//2
            GreenUI.Add(contentManager.Load<Texture2D>("GreenUI/GreenButtonRight"));//3
            GreenUI.Add(contentManager.Load<Texture2D>("GreenUI/GreenButtonLeft"));//4
            GreenUI.Add(contentManager.Load<Texture2D>("GreenUI/GreenBackground"));//5
            GreenUI.Add(contentManager.Load<Texture2D>("GreenUI/GreenBackground"));//6
            GreenUI.Add(contentManager.Load<Texture2D>("GreenUI/GreenX"));//7

            GreyUI.Add(contentManager.Load<Texture2D>("GreyUI/GreyButtonGeneral"));//0
            GreyUI.Add(contentManager.Load<Texture2D>("GreyUI/GreyButtonUp"));//1
            GreyUI.Add(contentManager.Load<Texture2D>("GreyUI/GreyButtonDown"));//2
            GreyUI.Add(contentManager.Load<Texture2D>("GreyUI/GreyButtonRight"));//3
            GreyUI.Add(contentManager.Load<Texture2D>("GreyUI/GreyButtonLeft"));//4
            GreyUI.Add(contentManager.Load<Texture2D>("GreyUI/GreyBackground"));//5
            GreyUI.Add(contentManager.Load<Texture2D>("GreyUI/GreyBackground2"));//6
            GreyUI.Add(contentManager.Load<Texture2D>("GreyUI/GreyX"));//7

            BlueRoomBackground = contentManager.Load<Texture2D>("BlueFloor");

            PokerChips.Add(contentManager.Load<Texture2D>("PokerChips/Dealer"));
            PokerChips.Add(contentManager.Load<Texture2D>("PokerChips/SmallBlind"));
            PokerChips.Add(contentManager.Load<Texture2D>("PokerChips/BigBlind"));
            PokerChips.Add(contentManager.Load<Texture2D>("PokerChips/chip1"));
            PokerChips.Add(contentManager.Load<Texture2D>("PokerChips/chip5"));
            PokerChips.Add(contentManager.Load<Texture2D>("PokerChips/chip25"));
            PokerChips.Add(contentManager.Load<Texture2D>("PokerChips/chip100"));
            PokerChips.Add(contentManager.Load<Texture2D>("PokerChips/chip500"));
            PokerChips.Add(contentManager.Load<Texture2D>("PokerChips/chip1000"));

            Fonts.Add(contentManager.Load<SpriteFont>("SpriteFont/GameFont"));//0
            Fonts.Add(contentManager.Load<SpriteFont>("SpriteFont/ChatFont"));//1
            Fonts.Add(contentManager.Load<SpriteFont>("SpriteFont/BubbleFont1"));//2
            Fonts.Add(contentManager.Load<SpriteFont>("SpriteFont/BubbleFont2"));//3
            Fonts.Add(contentManager.Load<SpriteFont>("SpriteFont/BubbleFont3"));//4

            SpeachBubble = contentManager.Load<Texture2D>("SpeachBubble");

            Background = contentManager.Load<Texture2D>("dark-green-texture");
            PokerBackGround = new TextureRegion2D(Background);

            LoginPage = contentManager.Load<Texture2D>("login");
            LoginPageBackground = new TextureRegion2D(LoginPage);

            PlayerFaces = new List<List<Texture2D>>();

            int i = 0;
            foreach (PlayerSkin skin in (PlayerSkin[])Enum.GetValues(typeof(PlayerSkin)))
            {
                textureName.Append("Player");
                textureName.Append(skin.ToString());
                textureName.Append('/');

                textureName.Append(skin.ToString());
                textureName.Append("FaceRight");

                PlayerFaces.Add(new List<Texture2D>());
                PlayerFaces[(int)skin].Add(contentManager.Load<Texture2D>(textureName.ToString()));
                textureName.Replace("Right", "Left");
                PlayerFaces[(int)skin].Add(contentManager.Load<Texture2D>(textureName.ToString()));

                textureName.Replace(skin.ToString(), "Stand", 7 + skin.ToString().Length, skin.ToString().Length);
                textureName.Replace("Left", "Right");

                playersSkinInRest[i][0] = contentManager.Load<Texture2D>(textureName.ToString());

                textureName.Replace("Right", "Left");
                playersSkinInRest[i][1] = contentManager.Load<Texture2D>(textureName.ToString());

                textureName.Replace("Left", "Right");
                textureName.Replace("Stand", "Walk");
                textureName.Append('0');
                for (int j = 1; j <= 1000; j++)
                {
                    textureName.Replace((j - 1).ToString(), j.ToString());
                    try
                    {
                        playerSkinInMovement[i][0].Add(contentManager.Load<Texture2D>(textureName.ToString()));
                    }
                    catch
                    {
                        textureName.Replace(j.ToString(), (j - 1).ToString());
                        break;
                    }
                }
                textureName.Replace("Right", "Left");
                textureName.Replace(playerSkinInMovement[i][0].Count.ToString(), "0");
                for (int j = 1; j <= 1000; j++)
                {
                    textureName.Replace((j - 1).ToString(), j.ToString());
                    try
                    {
                        playerSkinInMovement[i][1].Add(contentManager.Load<Texture2D>(textureName.ToString()));
                    }
                    catch
                    {
                        break;
                    }
                }
                i++;
                textureName.Clear();
            }

            Furnitures.Add(contentManager.Load<Texture2D>("Furnitures/CasinoRoomPokerTable"));//0
            Furnitures.Add(contentManager.Load<Texture2D>("Furnitures/Tree"));//1
            Furnitures.Add(contentManager.Load<Texture2D>("Furnitures/Statue"));//2
            Furnitures.Add(contentManager.Load<Texture2D>("Furnitures/BrownCouch"));//3
            Furnitures.Add(contentManager.Load<Texture2D>("Furnitures/Roulette Table"));//4
            Furnitures.Add(contentManager.Load<Texture2D>("Furnitures/Slot Machine"));//5
            Furnitures.Add(contentManager.Load<Texture2D>("Furnitures/TopPokerTable"));//6
            Furnitures.Add(contentManager.Load<Texture2D>("Furnitures/CasinoRoomSnokerTable"));//7
            Furnitures.Add(contentManager.Load<Texture2D>("Furnitures/CasinoRoomTreasureBox"));//8
            Furnitures.Add(contentManager.Load<Texture2D>("Furnitures/Couch"));//9 many couches


            textureName.Clear();
            for (i = 1; i <= 4; i++)
            {
                string ch;
                switch (i)
                {
                    case 1:
                        {
                            ch = "H";
                            break;
                        }
                    case 2:
                        {
                            ch = "D";
                            break;
                        }
                    case 3:
                        {
                            ch = "C";
                            break;
                        }
                    case 4:
                        {
                            ch = "S";
                            break;
                        }
                    default:
                        {
                            ch = "H";
                            break;
                        }
                }

                for (int j = 2; j <= 14; j++)
                {
                    textureName.Append("GameCards/");
                    if (j == 14)
                    {
                        textureName.Append("1");
                    }
                    else
                    {
                        textureName.Append(j.ToString());
                    }
                    textureName.Append(ch);
                    Cards.Add(contentManager.Load<Texture2D>(textureName.ToString()));
                    textureName.Clear();
                }
            }
            Cards.Add(contentManager.Load<Texture2D>("GameCards/backR"));
        }
    }
}
