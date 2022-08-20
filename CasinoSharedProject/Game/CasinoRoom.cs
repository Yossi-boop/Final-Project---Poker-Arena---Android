using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System.Threading.Tasks;

using Classes;

using System.Timers;
using Microsoft.Xna.Framework.Input.Touch;

namespace Casino
{

    public class CasinoRoom
    {
        private readonly Game1 gameManager;

        private readonly ContentManager contentManager;
        private SpriteBatch painter;

        private readonly SpritesStorage storage;

        private TouchCollection touchState;

        private Timer casinoTimer;
        private Timer pokerTableTimer;

        private DateTime cameraCounter;
        private bool cameraMoved = true;

        private Player mainPlayer;
        public Camera camera;

        public static readonly object updateobjlock = new object();
        public static readonly object drawobjlock = new object();
        public static readonly object instanceobjlock = new object();

        private List<CharacterInstance> playersInTheCasinoInformation;
        private readonly List<PlayerDrawingInformation> playersInTheCasino = new List<PlayerDrawingInformation>();

        public List<FurnitureInstance> furnituresList { get; set; }
        public List<Instance> instancesList { get; set; } = new List<Instance>();
        public Chest winningChest;

        public bool isReEnterToCasino = false;
        public bool IsUpdated { get; set; } = false;

        #region UpperBar
        private Vector2 coinPosition;
        private AnimationManager coinAnimationManager;
        private NewChat casinoRoomNewChat;
        private DrawingButton exitButton;

        #region Settings Panel
        private DrawingButton settingsButton;
        private Rectangle settingPanelRectangle;
        private DrawingButton volumeOnOffButton;
        private DrawingButton ninjaSkin;
        private DrawingButton jackSkin;
        private DrawingButton zombieSkin;
        private bool isSettingPanelVisible = false;
        #endregion

        #endregion

        #region Enter Table Panel
        private Rectangle enterTablePanelRectangle;
        private DrawingButton confirmEnterTable;
        private DrawingButton exitEnterTable;
        private bool isEnterTablePanelVisible = false;
        private string givenTableId = null;
        #endregion

        #region Explain Enter Table Panel
        private DrawingButton explainButton;
        private int speedOfChangedSpaceBar = 0;
        private bool isSpaceBarClicked = false;
        #endregion

        #region Winning Amount Of Chest Panel
        private Rectangle winningAmountOfChestPanelRectangle;
        private bool isWinningAmountOfChestPanelVisible = false;
        private DrawingButton confirmWinButton;
        #endregion

        private KeyboardInput keyboard;
        private JoysStick joystick;
        private Keys currentInput;

        private string lastMessage;

        private bool mainPlayerDraw = false;

        private bool isUpdateStatsAfterPokerTable = true;

        private Table currentTable;

        public CasinoRoom(Game1 i_gameManager, ContentManager i_Content, SpritesStorage i_Storage)
        {
            try
            {
                gameManager = i_gameManager;
                contentManager = i_Content;
                storage = i_Storage;

                camera = new Camera();
            }
            catch (Exception e)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                {
                    file.WriteLine("CasinoRoom.Constructor " + e.Message);
                }
                throw e;
            }
            
        }

        public async Task Load(SpriteBatch i_Painter)
        {
            try
            {
                painter = i_Painter;

                winningChest = await gameManager.server.getChest("1234");
                furnituresList = await gameManager.server.GetCasinoFurnitureInstances("1234");
                instancesList.Add(winningChest);
                instancesList.AddRange(furnituresList);

                User user = await gameManager.server.GetUserDetails(gameManager.mainPlayerEmail);
                mainPlayer = new Player(contentManager, Game1.UserScreenWidth, Game1.UserScreenHeight,
                  (PlayerSkin)user.Figure, storage, user.Name, gameManager.mainPlayerEmail,
                  await gameManager.server.GetStats(gameManager.mainPlayerEmail), furnituresList)
                {
                    updateStuckChest = winningChest
                };

                mainPlayer.Load(painter);
                camera.Follow(mainPlayer);

                casinoRoomNewChat = new NewChat(storage, 250, 250);
                casinoRoomNewChat.UpdateMessageList(await gameManager.server.getCasinoMessages("1234"));
                casinoRoomNewChat.Load(painter);

                #region Input From User
                keyboard = new KeyboardInput();
                joystick = new JoysStick(storage, painter);
                joystick.Load();
                #endregion

                explainButton = new DrawingButton(storage.GreenUI[0], storage.Fonts[0])
                {
                    Text = "SPACE"
                };
                explainButton.Click += explainButton_Click;

                #region Enter Table Buttons
                confirmEnterTable = new DrawingButton(storage.GreenUI[0], storage.Fonts[0])
                {
                    Text = "Confirm"
                };
                confirmEnterTable.Click += ConfirmEnterTable_Click;

                exitEnterTable = new DrawingButton(storage.GreenUI[0], storage.Fonts[0])
                {
                    Text = "Exit"
                };
                exitEnterTable.Click += ExitEnterTable_Click;
                #endregion

                #region UpperBar Buttons And Animations
                #region Settings Buttons
                settingsButton = new DrawingButton(storage.GreenUI[0], storage.Fonts[0])
                {
                    Text = "Settings"
                };
                settingsButton.Click += SettingsButton_Click;

                volumeOnOffButton = new DrawingButton(storage.GreenUI[0], storage.Fonts[0])
                {
                    Text = "Sound On"
                };
                volumeOnOffButton.Click += VolumeOnOffButton_Click;

                ninjaSkin = new DrawingButton(storage.GreenUI[0], storage.Fonts[0])
                {
                    Text = "NINJA"
                };
                ninjaSkin.Click += NinjaSkin_Click;

                jackSkin = new DrawingButton(storage.GreenUI[0], storage.Fonts[0])
                {
                    Text = "JACK"
                };
                jackSkin.Click += JackSkin_Click;

                zombieSkin = new DrawingButton(storage.GreenUI[0], storage.Fonts[0])
                {
                    Text = "ZOMBIE"
                };
                zombieSkin.Click += ZombieSkin_Click;
                #endregion

                exitButton = new DrawingButton(storage.GreenUI[0], storage.Fonts[0])
                {
                    Text = "Exit"
                };
                exitButton.Click += ExitButton_Click;

                coinAnimationManager = new AnimationManager(painter, storage);
                #endregion

                #region Winning Chest Button
                confirmWinButton = new DrawingButton(storage.GreenUI[0], storage.Fonts[0])
                {
                    Text = "Confirm"
                };
                confirmWinButton.Click += ConfirmWinButton_Click;
                #endregion

                await gameManager.server.UpdatePosition("1234", mainPlayer.playerEmail, mainPlayer.PlayerName, (int)mainPlayer.LastPosition.X,
                (int)mainPlayer.LastPosition.Y, (int)mainPlayer.drawingPosition.X,
                (int)mainPlayer.drawingPosition.Y, (int)mainPlayer.direction, (int)mainPlayer.playerSkin);
            }
            catch (Exception)
            {
            }
            
        }

        private void explainButton_Click(object sender, EventArgs e)
        {
            currentInput = Keys.Space;
        }

        public async void UpdateMainPlayer(string i_mainPlayerEmail)
        {
            try
            {
                User user = gameManager.server.GetUserDetails(gameManager.mainPlayerEmail).Result;

                mainPlayer = new Player(contentManager, Game1.UserScreenWidth, Game1.UserScreenHeight,
                  (PlayerSkin)user.Figure, storage, user.Name, gameManager.mainPlayerEmail, await gameManager.server.GetStats(gameManager.mainPlayerEmail), furnituresList)
                {
                    updateStuckChest = winningChest
                };
                mainPlayer.Load(painter);
                camera.Follow(mainPlayer);
            }
            catch (Exception)
            {
            }
            
        }

        #region Winning Chest Money Methods
        private void ConfirmWinButton_Click(object sender, EventArgs e)
        {
            turnConfirmWinChestPanelOff();
        }

        private void turnConfirmWinChestPanelOff()
        {
            isWinningAmountOfChestPanelVisible = false;
        }
        #endregion

        #region Enter Table Methods
        private void ExitEnterTable_Click(object sender, EventArgs e)
        {
            isEnterTablePanelVisible = false;
        }

        private void ConfirmEnterTable_Click(object sender, EventArgs e)
        {
            isEnterTablePanelVisible = false;
            if (givenTableId != null)
            {
                OpenPokerTable(givenTableId, mainPlayer.PlayerName, mainPlayer.playerEmail);
            }
            isUpdateStatsAfterPokerTable = false;
        }
        #endregion

        #region UpperBar Methods
        private void ExitButton_Click(object sender, EventArgs e)
        {
            isReEnterToCasino = true;
            MediaPlayer.Stop();
            gameManager.ScreenType = eScreenType.LoginPage;
            casinoTimer.Enabled = false;
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            switchSettingPanelVisible();
        }

        private void switchSettingPanelVisible()
        {
            isSettingPanelVisible = !isSettingPanelVisible;
        }

        #region Setting Methods
        private void VolumeOnOffButton_Click(object sender, EventArgs e)
        {
            if (MediaPlayer.Volume == 1f)
            {
                MediaPlayer.Volume = 0f;
                volumeOnOffButton.Text = "Sound Off";
            }
            else
            {
                MediaPlayer.Volume = 1f;
                volumeOnOffButton.Text = "Sound On";
            }
        }

        private void ZombieSkin_Click(object sender, EventArgs e)
        {
            changeSkinAndCloseSettingPanel((sender as DrawingButton).Text);
        }

        private void JackSkin_Click(object sender, EventArgs e)
        {
            changeSkinAndCloseSettingPanel((sender as DrawingButton).Text);
        }

        private void NinjaSkin_Click(object sender, EventArgs e)
        {
            changeSkinAndCloseSettingPanel((sender as DrawingButton).Text);
        }

        private void changeSkinAndCloseSettingPanel(string i_buttonText)
        {
            switch (i_buttonText)
            {
                case "NINJA":
                    mainPlayer.UpdatePlayerSkin(PlayerSkin.Ninja);
                    break;
                case "JACK":
                    mainPlayer.UpdatePlayerSkin(PlayerSkin.Jack);
                    break;
                case "ZOMBIE":
                    mainPlayer.UpdatePlayerSkin(PlayerSkin.Zombie);
                    break;
                default:
                    break;
            }
            gameManager.server.ChangeUserDetails(mainPlayer.playerEmail, new User(mainPlayer.PlayerName, mainPlayer.playerEmail, (int)mainPlayer.playerSkin));
            switchSettingPanelVisible();
        }
        #endregion
        #endregion

        private void initializeIntervalsForPokerTimer()
        {
            try
            {
                if (pokerTableTimer == null)
                {
                    pokerTableTimer = new Timer
                    {
                        Interval = 1000
                    };
                    pokerTableTimer.Elapsed += pokerTableTimer_Elapsed;
                    pokerTableTimer.AutoReset = true;
                    pokerTableTimer.Enabled = true;
                }
                else if (pokerTableTimer.Enabled == false)
                {
                    pokerTableTimer.Enabled = true;
                }
            }
            catch (Exception)
            {
            }
        }

        private void pokerTableTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                gameManager.server.UpdatePosition("1234", mainPlayer.playerEmail, mainPlayer.PlayerName,
                (int)mainPlayer.LastPosition.X, (int)mainPlayer.LastPosition.Y,
                (int)mainPlayer.drawingPosition.X, (int)mainPlayer.drawingPosition.Y,
                (int)mainPlayer.direction, (int)mainPlayer.playerSkin);
            }
            catch (Exception)
            {
            }
        }

        private void initializeIntervals()
        {
            try
            {
                if (casinoTimer == null)
                {
                    casinoTimer = new Timer
                    {
                        Interval = 100
                    };
                    casinoTimer.Elapsed += CasinoTimer_Elapsed;
                    casinoTimer.AutoReset = true;
                    casinoTimer.Enabled = true;
                    if(pokerTableTimer != null)
                        pokerTableTimer.Enabled = false;
                }
                else if (casinoTimer.Enabled == false)
                {
                    if(pokerTableTimer != null)
                        pokerTableTimer.Enabled = false;
                    casinoTimer.Enabled = true;
                }
            }
            catch (Exception)
            {
            }   
        }

        private async void CasinoTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                await gameManager.server.UpdatePosition("1234", mainPlayer.playerEmail, mainPlayer.PlayerName,
                (int)mainPlayer.LastPosition.X, (int)mainPlayer.LastPosition.Y, 
                (int)mainPlayer.drawingPosition.X, (int)mainPlayer.drawingPosition.Y, 
                (int)mainPlayer.direction, (int)mainPlayer.playerSkin);
                mainPlayer.LastPosition.X = mainPlayer.position.X;
                mainPlayer.LastPosition.Y = mainPlayer.position.Y;
                List<CharacterInstance> testPlayersInTheCasinoInformation;
                testPlayersInTheCasinoInformation = 
                    await gameManager.server.GetPosition("1234", mainPlayer.playerEmail);
                if (testPlayersInTheCasinoInformation != null)
                {
                    playersInTheCasinoInformation = testPlayersInTheCasinoInformation;
                }

                List<Message> testChatData =await gameManager.server.getCasinoMessages("1234");
                if (testChatData != null)
                {
                    casinoRoomNewChat.UpdateMessageList(testChatData);
                }

                winningChest = await gameManager.server.getChest("1234");
                mainPlayer.updateStuckChest = winningChest;

                lock (updateobjlock)
                {
                    if (playersInTheCasino != null && playersInTheCasino.Count > 0)
                    {
                        playersInTheCasino.Clear();
                    }


                    foreach (CharacterInstance player in playersInTheCasinoInformation)
                    {
                        playersInTheCasino.Add(new PlayerDrawingInformation
                            (player, contentManager, painter, storage));
                    }
                }

                lock (instanceobjlock)
                {
                    instancesList.Clear();
                    instancesList.Add(winningChest);
                    instancesList.AddRange(furnituresList);
                    instancesList.AddRange(playersInTheCasino);
                    instancesList.Sort();
                }
                
            }
            catch (Exception)
            {
            }
            
        }

        public void casinoPeopleUpdate(GameTime i_GameTime)
        {
            try
            {
                if (playersInTheCasino != null) //The list exist
                {
                    lock (updateobjlock)
                    {
                        foreach (PlayerDrawingInformation player in playersInTheCasino)
                        {
                            player.updateOnlinePlayer(i_GameTime, player.direction);
                        }
                    }   
                }
            }
            catch (Exception)
            {
            }
        }

        public async void Update(GameTime i_gameTime)
        {
            try
            {
                IsUpdated = true;
                touchState = TouchPanel.GetState();
                Vector2 touchLocation = new Vector2(0, 0);
                if (touchState.Count > 0)
                    touchLocation = new Vector2(touchState[0].Position.X, touchState[0].Position.Y);
                updateStatsAfterPokerTable();
                initializeIntervals();
                updateEnterTablePanel(i_gameTime, camera.Position, touchLocation);
                casinoPeopleUpdate(i_gameTime);
                updateWinningAmountOfChest(i_gameTime, camera.Position, touchLocation);
                updateCurrentInput(i_gameTime, camera.Position, touchLocation);

                if (cameraMoved && (currentInput == Keys.Up || currentInput == Keys.Down || 
                    currentInput == Keys.Right || currentInput == Keys.Left))
                {
                    cameraCounter = DateTime.Now;
                    cameraMoved = false;
                }

                FurnitureInstance furniture = nearToPokerTable();
                if (furniture != null)
                {
                    if ((furniture.Type == 0 || furniture.Type == 4 || furniture.Type == 18 ||
                        furniture.Type == 2 || furniture.Type == 17 || furniture.Type == 1) &&
                        !casinoRoomNewChat.IsChatVisible)
                    {
                        explainButton.IsVisible = true;
                    }
                    else if (furniture.Type == 8)
                    {
                        if (!winningChest.Collected)
                        {
                            explainButton.IsVisible = true;
                        }
                        else
                        {
                            explainButton.IsVisible = false;
                        }
                    }
                    else
                    {
                        explainButton.IsVisible = false;
                    }

                    if (currentInput == Keys.Space && !casinoRoomNewChat.IsChatVisible)
                    {
                        switch (furniture.Type)
                        {
                            case 0:
                                {
                                    isEnterTablePanelVisible = true;
                                    givenTableId = furniture.Id;
                                    currentInput = Keys.None;
                                    break;
                                }
                            case 1:
                                {
                                    MediaPlayer.Play(storage.TreesSound);
                                    currentInput = Keys.None;
                                    break;
                                }
                            case 2:
                                {
                                    MediaPlayer.Play(storage.StatueSound);
                                    currentInput = Keys.None;
                                    break;
                                }
                            case 4:
                                {
                                    MediaPlayer.Play(storage.RouletteWheelSound);
                                    currentInput = Keys.None;
                                    break;
                                }
                            case 8:
                                {
                                    if (!winningChest.Collected)
                                    {
                                        await gameManager.server.CollectChest("1234", mainPlayer.playerEmail);
                                        mainPlayer.stats = await gameManager.server.GetStats(mainPlayer.playerEmail);
                                        MediaPlayer.Play(storage.ChestSound);
                                        isWinningAmountOfChestPanelVisible = true;
                                    }
                                    currentInput = Keys.None;
                                    break;
                                }
                            case 17:
                                {
                                    isEnterTablePanelVisible = true;
                                    givenTableId = furniture.Id;
                                    currentInput = Keys.None;
                                    updateEnterTablePanel(i_gameTime, camera.Position, touchLocation);
                                    break;
                                }
                            case 18:
                                {
                                    MediaPlayer.Play(storage.RouletteWheelSound);
                                    currentInput = Keys.None;
                                    break;
                                }
                        }
                    }
                }
                else
                {
                    explainButton.IsVisible = false;
                }

                mainPlayer.updatePlayer(i_gameTime, currentInput, instanceobjlock);
                lastMessage = casinoRoomNewChat.Update(i_gameTime, mainPlayer.position, 
                    currentInput, keyboard.isCapsLockOn, keyboard.isShiftOn);

                if (lastMessage != null)
                {
                    mainPlayer.LastActionTime = DateTime.Now;
                    mainPlayer.DrawBubble(lastMessage);
                    string serverAnswer;
                    do
                    {
                        serverAnswer = await gameManager.server.SendMessageToCasinoChat
                            ("1234", mainPlayer.playerEmail, null, mainPlayer.PlayerName, lastMessage);
                    }
                    while (serverAnswer == null);
                    lastMessage = null;
                }

                updateSettingPanel(i_gameTime, camera.Position, touchLocation);

                if (!cameraMoved && DateTime.Now.Subtract(cameraCounter).TotalMilliseconds >= 1000)
                {
                    float xDifrance = camera.Position.X - mainPlayer.position.X;
                    float yDifrance = camera.Position.Y - mainPlayer.position.Y;
                    Vector2 temp = camera.Position;
                    if (xDifrance > 0)
                    {
                        if (xDifrance - (float)i_gameTime.ElapsedGameTime.TotalSeconds * 150 > 0)
                        {
                            temp.X -= (float)i_gameTime.ElapsedGameTime.TotalSeconds * 150;
                        }
                        else
                        {
                            temp.X = mainPlayer.position.X;
                        }
                    }
                    else if (xDifrance < 0)
                    {
                        if (xDifrance + (float)i_gameTime.ElapsedGameTime.TotalSeconds * 150 < 0)
                        {
                            temp.X += (float)i_gameTime.ElapsedGameTime.TotalSeconds * 150;
                        }
                        else
                        {
                            temp.X = mainPlayer.position.X;
                        }

                    }

                    if (yDifrance > 0)
                    {
                        if (yDifrance - (float)i_gameTime.ElapsedGameTime.TotalSeconds * 150 > 0)
                        {
                            temp.Y -= (float)i_gameTime.ElapsedGameTime.TotalSeconds * 150;
                        }
                        else
                        {
                            temp.Y = mainPlayer.position.Y;
                        }
                    }
                    else if (yDifrance < 0)
                    {
                        if (yDifrance + (float)i_gameTime.ElapsedGameTime.TotalSeconds * 150 < 0)
                        {
                            temp.Y += (float)i_gameTime.ElapsedGameTime.TotalSeconds * 150;
                        }
                        else
                        {
                            temp.Y = mainPlayer.position.Y;
                        }

                    }

                    camera.Position = temp;

                    if (camera.Position == mainPlayer.position)
                    {
                        cameraMoved = true;
                    }
                }

                camera.Follow(mainPlayer);
                updateExplainToPlayerHowToEnterPokerTable(i_gameTime, camera.Position, touchLocation);
                updateUpperBar(i_gameTime, touchLocation);
                updateChat(i_gameTime, touchLocation);
            }
            catch (Exception)
            {
            }
            
        }

        private void updateCurrentInput(GameTime i_gameTime, Vector2 i_cameraPosition, Vector2 screenTouch)
        {
            try
            {
                if(currentInput != Keys.Space)
                    currentInput = joystick.Update(i_gameTime, i_cameraPosition, screenTouch);
            }
            catch (Exception)
            {
            }
            
        }

        private void updateWinningAmountOfChest(GameTime i_gameTime, Vector2 i_mainPosition, 
            Vector2 screenTouch)
        {
            try
            {
                if (isWinningAmountOfChestPanelVisible)
                {
                    Vector2 panelLocation = new Vector2(-100, -100) + i_mainPosition;
                    confirmWinButton.Position = new Vector2(80, 90) + panelLocation;
                    confirmWinButton.Update(i_gameTime, (int)i_mainPosition.X - Game1.UserScreenWidth / 2, 
                        (int)i_mainPosition.Y - Game1.UserScreenHeight / 2, (int)screenTouch.X,
                        (int)screenTouch.Y);
                    winningAmountOfChestPanelRectangle = new Rectangle((int)panelLocation.X, 
                        (int)panelLocation.Y, 350, 150);
                }
            }
            catch (Exception)
            {
            }
        }

        private async void updateStatsAfterPokerTable()
        {
            try
            {
                if (!isUpdateStatsAfterPokerTable)
                {
                    mainPlayer.stats = await gameManager.server.GetStats(mainPlayer.playerEmail);
                    isUpdateStatsAfterPokerTable = true;
                }
            }
            catch (Exception)
            {
            }
            
        }

        private void updateExplainToPlayerHowToEnterPokerTable(GameTime i_gameTime, Vector2 i_mainPosition,
            Vector2 screenTouch)
        {
            try
            {
                if (explainButton.IsVisible && !casinoRoomNewChat.IsChatVisible)
                {
                    Vector2 buttonLocation = new Vector2(390, 154) + i_mainPosition;
                    explainButton.Position = buttonLocation;
                    explainButton.Update(i_gameTime, (int)i_mainPosition.X - Game1.UserScreenWidth / 2,
                        (int)i_mainPosition.Y - Game1.UserScreenHeight / 2, (int)screenTouch.X,
                        (int)screenTouch.Y);
                    speedOfChangedSpaceBar++;
                    speedOfChangedSpaceBar %= 20;
                    if (speedOfChangedSpaceBar % 20 == 0)
                    {
                        isSpaceBarClicked = !isSpaceBarClicked;
                    }
                }
            }
            catch (Exception)
            {
            }
            
        }

        private void updateEnterTablePanel(GameTime i_gameTime, Vector2 i_mainPosition, Vector2 screenTouch)
        {
            try
            {
                if (isEnterTablePanelVisible)
                {
                    Vector2 enterTablePanelLocation = new Vector2(-200, -200) + i_mainPosition;

                    confirmEnterTable.Position = new Vector2(50, 135) + enterTablePanelLocation;
                    confirmEnterTable.Update(i_gameTime, (int)i_mainPosition.X - Game1.UserScreenWidth / 2, 
                        (int)i_mainPosition.Y - Game1.UserScreenHeight / 2, (int)screenTouch.X,
                (int)screenTouch.Y);
                    exitEnterTable.Position = new Vector2((int)confirmEnterTable.Position.X + 
                        confirmEnterTable.Rectangle.Width + 30, (int)confirmEnterTable.Position.Y);
                    exitEnterTable.Update(i_gameTime, (int)i_mainPosition.X - Game1.UserScreenWidth / 2, 
                        (int)i_mainPosition.Y - Game1.UserScreenHeight / 2, (int)screenTouch.X,
                (int)screenTouch.Y);

                    enterTablePanelRectangle = new Rectangle((int)enterTablePanelLocation.X - 25, 
                        (int)enterTablePanelLocation.Y, 165 + (confirmEnterTable.Rectangle.Width * 2), 200);
                }
            }
            catch (Exception)
            {
            }
            
        }

        private FurnitureInstance nearToPokerTable()
        {
            try
            {
                foreach (FurnitureInstance furniture in furnituresList)
                {
                    if (isNearToPlayer(furniture))
                    {
                        return furniture;
                    }

                }
                if (isNearToPlayer(winningChest))
                {
                    return winningChest;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private bool isNearToPlayer(FurnitureInstance i_Furniture)
        {
            try
            {
                mainPlayer.calculateFurnitureSize(i_Furniture, out int leftSide, out int rightSide, 
                    out int upSide, out int downSide);
                float playerRightSide = mainPlayer.position.X + storage.width * 75 - 1;
                float playerLeftSide = mainPlayer.position.X + 1;
                float playerUpSide = mainPlayer.position.Y + 1;
                float PlayerDownSide = mainPlayer.position.Y + storage.width * 100 - 1;

                if ((inRange(playerRightSide, leftSide) || inRange(playerLeftSide, rightSide)) &&
                    ((playerUpSide >= upSide && playerUpSide <= downSide) || (PlayerDownSide >= upSide && PlayerDownSide <= downSide) ||
                    (playerUpSide <= upSide && PlayerDownSide >= upSide) || (playerUpSide <= downSide && PlayerDownSide >= downSide)))
                {
                    return true;
                }

                if ((inRange(playerUpSide, downSide) || inRange(PlayerDownSide, upSide)) &&
                    ((playerLeftSide >= leftSide && playerLeftSide <= rightSide) || (playerRightSide >= leftSide && playerRightSide <= rightSide) ||
                    (playerLeftSide <= leftSide && playerRightSide >= leftSide) || (playerLeftSide <= rightSide && playerRightSide >= rightSide)))
                {
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool inRange(float playerRightSide, int leftSide)
        {
            try
            {
                return playerRightSide + 5 >= leftSide && playerRightSide - 5 <= leftSide;
            }
            catch (Exception)
            {
                return false;
            }   
        }

        private void updateSettingPanel(GameTime i_gameTime, Vector2 i_mainPosition, Vector2 screenTouch)
        {
            try
            {
                if (isSettingPanelVisible)
                {
                    Vector2 settingPanelLocation = new Vector2(-200, -200) + i_mainPosition;

                    ninjaSkin.Position = new Vector2(5, 60) + settingPanelLocation;
                    ninjaSkin.Update(i_gameTime, (int)i_mainPosition.X - Game1.UserScreenWidth / 2, 
                        (int)i_mainPosition.Y - Game1.UserScreenHeight / 2, (int)screenTouch.X,
                        (int)screenTouch.Y);
                    jackSkin.Position = new Vector2((int)ninjaSkin.Position.X + ninjaSkin.Rectangle.Width + 30, 
                        (int)ninjaSkin.Position.Y);
                    jackSkin.Update(i_gameTime, (int)i_mainPosition.X - Game1.UserScreenWidth / 2, 
                        (int)i_mainPosition.Y - Game1.UserScreenHeight / 2, (int)screenTouch.X,
                        (int)screenTouch.Y);
                    zombieSkin.Position = new Vector2(ninjaSkin.Position.X + 
                        (ninjaSkin.Rectangle.Width / 2) + 30, (int)jackSkin.Position.Y + 
                        jackSkin.Rectangle.Height + 30);
                    zombieSkin.Update(i_gameTime, (int)i_mainPosition.X - Game1.UserScreenWidth / 2, 
                        (int)i_mainPosition.Y - Game1.UserScreenHeight / 2, (int)screenTouch.X,
                        (int)screenTouch.Y);
                    volumeOnOffButton.Position = new Vector2(ninjaSkin.Position.X + 
                        (ninjaSkin.Rectangle.Width / 2) + 30, zombieSkin.Position.Y + 
                        ninjaSkin.Rectangle.Height + 30);
                    volumeOnOffButton.Update(i_gameTime, (int)i_mainPosition.X - Game1.UserScreenWidth / 2, 
                        (int)i_mainPosition.Y - Game1.UserScreenHeight / 2, (int)screenTouch.X,
                        (int)screenTouch.Y);
                    if (MediaPlayer.Volume == 1f)
                        volumeOnOffButton.Text = "Sound On";
                    else
                        volumeOnOffButton.Text = "Sound Off";

                    settingPanelRectangle = new Rectangle((int)settingPanelLocation.X - 25, (int)settingPanelLocation.Y, 100 + (ninjaSkin.Rectangle.Width * 2), 100 + (ninjaSkin.Rectangle.Height * 2) + ninjaSkin.Rectangle.Height * 2);
                }
            }
            catch (Exception)
            {
            }
        }

        private void updateChat(GameTime i_gameTime, Vector2 screenTouch)
        {
            try
            {
                casinoRoomNewChat.ChatButton.Position = new Vector2(450, 313) + camera.Position;
                casinoRoomNewChat.ChatButton.Update(i_gameTime, (int)camera.Position.X - Game1.UserScreenWidth / 2, 
                    (int)camera.Position.Y - Game1.UserScreenHeight / 2, (int)screenTouch.X,
                    (int)screenTouch.Y);
                casinoRoomNewChat.MoveChatUpButton.Position = new Vector2(-380, 50) + camera.Position;
                casinoRoomNewChat.MoveChatUpButton.Update(i_gameTime, (int)camera.Position.X - Game1.UserScreenWidth / 2, 
                    (int)camera.Position.Y - Game1.UserScreenHeight / 2, (int)screenTouch.X,
                    (int)screenTouch.Y);
                casinoRoomNewChat.MoveChatDownButton.Position = new Vector2(-380, 250) + camera.Position;
                casinoRoomNewChat.MoveChatDownButton.Update(i_gameTime, (int)camera.Position.X - Game1.UserScreenWidth / 2, 
                    (int)camera.Position.Y - Game1.UserScreenHeight / 2, (int)screenTouch.X,
                    (int)screenTouch.Y);
                casinoRoomNewChat.SendMessageButton.Position = new Vector2(-640, 313) + camera.Position;
                casinoRoomNewChat.SendMessageButton.Update(i_gameTime, (int)camera.Position.X - Game1.UserScreenWidth / 2, 
                    (int)camera.Position.Y - Game1.UserScreenHeight / 2, (int)screenTouch.X,
                    (int)screenTouch.Y);
                casinoRoomNewChat.MoveChatToLastMessage.Position = new Vector2(-380, 300) + camera.Position;
                casinoRoomNewChat.MoveChatToLastMessage.Update(i_gameTime, (int)camera.Position.X - Game1.UserScreenWidth / 2, 
                    (int)camera.Position.Y - Game1.UserScreenHeight / 2, (int)screenTouch.X,
                    (int)screenTouch.Y);
            }
            catch (Exception)
            {
            }
        }

        private void updateUpperBar(GameTime i_gameTime, Vector2 screenTouch)
        {
            try
            {
                coinPosition = new Vector2(-35, -(Game1.UserScreenHeight / 2 - 5)) + camera.Position;
                exitButton.Position = new Vector2(485, -5) + coinPosition;
                exitButton.Update(i_gameTime, (int)camera.Position.X - Game1.UserScreenWidth / 2, 
                    (int)camera.Position.Y - Game1.UserScreenHeight / 2, (int)screenTouch.X,
                    (int)screenTouch.Y);
                settingsButton.Position = new Vector2(-exitButton.Rectangle.Width, 0) + exitButton.Position;
                settingsButton.Update(i_gameTime, (int)camera.Position.X - Game1.UserScreenWidth / 2, 
                    (int)camera.Position.Y - Game1.UserScreenHeight / 2, (int)screenTouch.X,
                    (int)screenTouch.Y);
                coinAnimationManager.UpdateAnimation();
            }
            catch (Exception)
            {
            }
        }

        private void drawCasinoWalls()
        {
            try
            {
                for (int i = 0; i < 22; i++)
                {
                    for (int j = 0; j < 72; j++)
                    {
                        if ((816 + j * storage.Furnitures[5].Width <= 2096 || 816 + j * storage.Furnitures[5].Width >= 4656) ||
                            (744 + i * storage.Furnitures[5].Height <= 1464 || 744 + i * storage.Furnitures[5].Height >= 2904))
                        {
                            painter.Draw(storage.Furnitures[5], new Vector2(831 + j * storage.Furnitures[5].Width, 804 + i * storage.Furnitures[5].Height), Color.White);
                        }
                    }
                }

                for (int i = 0; i < 72; i++)
                {
                    painter.Draw(storage.Furnitures[5], new Rectangle(2096 + i * storage.Furnitures[5].Width, 2904, storage.Furnitures[5].Width, storage.Furnitures[5].Height + 20), Color.White);
                }

                for (int i = 0; i < 10; i++)
                {
                    if (i == 9)
                    {
                        painter.Draw(storage.Furnitures[5], new Rectangle(4655, 1596 + i * storage.Furnitures[5].Height, storage.Furnitures[5].Width + 10, storage.Furnitures[5].Height - 10), Color.White);
                    }
                    else
                    {
                        painter.Draw(storage.Furnitures[5], new Rectangle(4655, 1596 + i * storage.Furnitures[5].Height, storage.Furnitures[5].Width + 15, storage.Furnitures[5].Height), Color.White);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public void Draw(GameTime i_gameTime)
        {
            try
            {
                casinoRoomDraw();
                mainPlayerDraw = false;
                drawCasinoWalls();
                drawCasinoInstances(i_gameTime);
                drawExplainToPlayerHowToEnterPokerTable(i_gameTime);
                drawEnterTablePanel(i_gameTime);
                drawWinningAmountOfChest(i_gameTime);
                drawUpperBar(i_gameTime);
                joystick.Draw(i_gameTime);
                casinoRoomNewChat.Draw(i_gameTime);
                drawSettingPanel(i_gameTime);
                IsUpdated = false;
            }
            catch (Exception)
            {
            }
            
        }

        private void drawWinningAmountOfChest(GameTime i_gameTime)
        {
            try
            {
                if (isWinningAmountOfChestPanelVisible)
                {
                    painter.Draw(storage.GreenUI[5], winningAmountOfChestPanelRectangle, Color.White);
                    painter.DrawString(storage.Fonts[0],
                        string.Format(@"Enjoy Your {0} 
    New Coins!"
    , winningChest.GoldAmount), new Vector2(winningAmountOfChestPanelRectangle.X + 80, winningAmountOfChestPanelRectangle.Y + 8), Color.Black);
                    confirmWinButton.Draw(i_gameTime, painter);
                }
            }
            catch (Exception)
            {
            }
        }

        private void drawCasinoInstances(GameTime i_gameTime)
        {
            try
            {
                lock (instanceobjlock)
                {
                    foreach (Instance casinoInstance in instancesList)
                    {
                        if (!mainPlayerDraw && mainPlayer.position.Y < casinoInstance.CurrentYPos)
                        {
                            mainPlayer.drawPlayer();
                            mainPlayerDraw = true;
                        }
                        if ((casinoInstance as FurnitureInstance) != null)
                        {
                            drawFurniture(casinoInstance as FurnitureInstance);
                        }
                        else
                        {
                            (casinoInstance as PlayerDrawingInformation).drawOnlinePlayer(painter);
                        }
                    }
                    if (!mainPlayerDraw)
                    {
                        mainPlayer.drawPlayer();
                        mainPlayerDraw = true;
                    }
                }
            }
            catch (Exception)
            {
            }   
        }

        private void drawExplainToPlayerHowToEnterPokerTable(GameTime i_gameTime)
        {
            try
            {
                if (explainButton.IsVisible)
                {
                    if (isSpaceBarClicked)
                        explainButton.Draw(i_gameTime, painter, Color.White);
                    else
                        explainButton.Draw(i_gameTime, painter, Color.Gray);
                }
            }
            catch (Exception)
            {
            }
        }

        private async void drawEnterTablePanel(GameTime i_gameTime)
        {
            try
            {
                if (isEnterTablePanelVisible)
                {
                    if (currentTable == null || !currentTable.Id.Equals(givenTableId))
                    {
                        currentTable = await gameManager.server.GetTableById(givenTableId, 
                            "1234", mainPlayer.playerEmail);
                    }
                    painter.Draw(storage.GreenUI[5], enterTablePanelRectangle, Color.White);
                    painter.DrawString(storage.Fonts[0], "Are You Sure You Want To Enter?", 
                        new Vector2(enterTablePanelRectangle.X + 25, enterTablePanelRectangle.Y + 40), 
                        Color.Black);
                    painter.DrawString(storage.Fonts[0], "Small Blind: " + 
                        currentTable.GameSetting.SmallBlind.ToString(), 
                        new Vector2(enterTablePanelRectangle.X + 30, enterTablePanelRectangle.Y + 90), 
                        Color.Black);
                    painter.DrawString(storage.Fonts[0], "Big Blind: " + 
                        currentTable.GameSetting.BigBlind.ToString(), 
                        new Vector2(enterTablePanelRectangle.X + 280, enterTablePanelRectangle.Y + 90), 
                        Color.Black);
                    confirmEnterTable.Draw(i_gameTime, painter);
                    exitEnterTable.Draw(i_gameTime, painter);
                }
            }
            catch (Exception)
            {
            }   
        }

        private void drawSettingPanel(GameTime i_gameTime)
        {
            try
            {
                if (isSettingPanelVisible)
                {
                    painter.Draw(storage.GreenUI[5], settingPanelRectangle, Color.White);
                    painter.DrawString(storage.Fonts[0], "Please Choose Your Skin:", new Vector2(55, 20) + 
                        new Vector2((int)settingPanelRectangle.X, (int)settingPanelRectangle.Y), Color.Black);
                    ninjaSkin.Draw(i_gameTime, painter);
                    jackSkin.Draw(i_gameTime, painter);
                    zombieSkin.Draw(i_gameTime, painter);
                    volumeOnOffButton.Draw(i_gameTime, painter);
                }
            }
            catch (Exception)
            {
            }
        }

        private void drawUpperBar(GameTime i_gameTime)
        {
            try
            {
                coinAnimationManager.DrawAnimation(coinPosition, storage.Coins[0].Width, 
                    storage.Coins[0].Height);
                painter.DrawString(storage.Fonts[0], mainPlayer.stats.Money.ToString(), 
                    new Vector2(50, 10) + coinPosition, Color.Black);
                if (!casinoRoomNewChat.IsChatVisible && casinoRoomNewChat.newMessagesAvialble)
                {
                    casinoRoomNewChat.ChatButton.Draw(i_gameTime, painter, Color.Red);
                }
                else
                {
                    casinoRoomNewChat.ChatButton.Draw(i_gameTime, painter);
                }
                
                exitButton.Draw(i_gameTime, painter);
                settingsButton.Draw(i_gameTime, painter);
            }
            catch (Exception)
            {
            }
        }

        public void casinoRoomDraw()
        {
            try
            {
                painter.Draw(storage.BlueRoomBackground, new Rectangle(1500, 1000, 4000, 3000), Color.White);

            }
            catch (Exception)
            {
            }
        }

        public void casinoFurnitureDraw()
        {
            try
            {
                foreach (FurnitureInstance furniture in furnituresList)
                {
                    drawFurniture(furniture);
                }
            }
            catch (Exception)
            {
            }
        }

        private void drawFurniture(FurnitureInstance i_furniture)
        {
            try
            {
                if (i_furniture.Type >= 0 && i_furniture.Type <= 16)
                {
                    if (i_furniture.Type == 9)
                    {
                        painter.Draw(storage.Furnitures[i_furniture.Type], 
                            new Rectangle(i_furniture.CurrentXPos, i_furniture.CurrentYPos, 
                            i_furniture.Width, i_furniture.Length), 
                            new Rectangle(430, 242, 100, 95), Color.White);
                    }
                    else if (i_furniture.Type == 10)
                    {
                        painter.Draw(storage.Furnitures[9], 
                            new Rectangle(i_furniture.CurrentXPos, i_furniture.CurrentYPos, 
                            i_furniture.Width, i_furniture.Length), 
                            new Rectangle(578, 285, 100, 95), Color.White);
                    }
                    else if (i_furniture.Type == 11)
                    {
                        painter.Draw(storage.Furnitures[9], 
                            new Rectangle(i_furniture.CurrentXPos, i_furniture.CurrentYPos, 
                            i_furniture.Width, i_furniture.Length), 
                            new Rectangle(625, 242, 100, 95), Color.White);
                    }
                    else if (i_furniture.Type == 12)
                    {
                        painter.Draw(storage.Furnitures[9], 
                            new Rectangle(i_furniture.CurrentXPos, i_furniture.CurrentYPos, 
                            i_furniture.Width, i_furniture.Length), 
                            new Rectangle(475, 292, 100, 95), Color.White);
                    }
                    else if (i_furniture.Type == 13)
                    {
                        painter.Draw(storage.Furnitures[9], 
                            new Rectangle(i_furniture.CurrentXPos, i_furniture.CurrentYPos, 
                            i_furniture.Width, i_furniture.Length), 
                            new Rectangle(586, 49, 67, 140), Color.White);
                    }
                    else if (i_furniture.Type == 14)
                    {
                        painter.Draw(storage.Furnitures[9], 
                            new Rectangle(i_furniture.CurrentXPos, i_furniture.CurrentYPos, 
                            i_furniture.Width, i_furniture.Length), 
                            new Rectangle(682, 49, 67, 140), Color.White);
                    }
                    else if (i_furniture.Type == 15)
                    {
                        painter.Draw(storage.Furnitures[9], 
                            new Rectangle(i_furniture.CurrentXPos, i_furniture.CurrentYPos, 
                            i_furniture.Width, i_furniture.Length), 
                            new Rectangle(432, 50, 140, 79), Color.White);
                    }
                    else if (i_furniture.Type == 16)
                    {
                        painter.Draw(storage.Furnitures[9], 
                            new Rectangle(i_furniture.CurrentXPos, i_furniture.CurrentYPos, 
                            i_furniture.Width, i_furniture.Length), 
                            new Rectangle(432, 165, 140, 79), Color.White);
                    }
                    else
                    {
                        if (i_furniture.Type != 8 || 
                            ((i_furniture as Chest) != null && !(i_furniture as Chest).Collected))
                        {
                            painter.Draw(storage.Furnitures[i_furniture.Type], 
                                new Rectangle(i_furniture.CurrentXPos, i_furniture.CurrentYPos, 
                                i_furniture.Width, i_furniture.Length), Color.White);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public async void OpenPokerTable(string i_tableID, string i_playerName, string i_playerEmail)
        {
            try
            {
                MediaPlayer.Play(storage.CoinsMusic);
                gameManager.pokerTable = new PokerTable(gameManager, gameManager.GraphicsDevice, painter, storage,
                    contentManager, "1234", i_tableID, i_playerEmail, i_playerName);
                casinoTimer.Enabled = false;
                initializeIntervalsForPokerTimer();
                await gameManager.pokerTable.Load();
                gameManager.ScreenType = eScreenType.PokerTable;
            }
            catch (Exception)
            {
            }
        }
    }
}