using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Text;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.ViewportAdapters;
using Microsoft.Xna.Framework.Content;

namespace Casino
{
    class LoginPage
    {
        private readonly Game1 gameManager;

        private readonly GraphicsDevice _graphics;
        private SpriteBatch _spriteBatch;
        private readonly ContentManager contentManager;
        private SpritesStorage storage;

        private GuiSystem _guiSystem;

        private bool isUserNameTextBoxFocused = false;
        private bool isPassWordTextBoxFocused = false;

        private TextBox userNameTextBox;
        private TextBox passWordTextBox;
        private Button loginButton;
        private Button registerButton;
        private Label errorMessage;
        private Label tryAgainMessage;

        private readonly StringBuilder password;
        private readonly StringBuilder starsPassword;

        private const string enterUsernameAndPassword = "Please insert Username And Password.";
        private const string wrongUserNameOrPassWord = "Wrong User Name Or Password.";
        private const string userAlreadyLogin = "This user is already login.";
        private const string userAlreadyLoginTryAgain = "Try again in 30 seconds."; 
        private const string unknownError = "Something went wrong.";

        public LoginPage(Game1 i_gameManager, GraphicsDevice i_grapics, ContentManager i_contentManager)
        {
            gameManager = i_gameManager;
            _graphics = i_grapics;
            contentManager = i_contentManager;
            password = new StringBuilder();
            starsPassword = new StringBuilder();
        }

        public void Load(SpriteBatch i_spriteBatch, SpritesStorage i_storage)
        {
            float width = 1280 / Game1.UserScreenWidth;
            float hight = 720 / Game1.UserScreenHeight;

            _spriteBatch = i_spriteBatch;
            storage = i_storage;

            DefaultViewportAdapter viewportAdapter = new DefaultViewportAdapter(_graphics);
            GuiSpriteBatchRenderer guiRenderer = new GuiSpriteBatchRenderer(_graphics, () => Matrix.Identity);
            var font = contentManager.Load<BitmapFont>("Sensation");
            BitmapFont.UseKernings = false;
            Skin.CreateDefault(font);

            userNameTextBox = new TextBox()
            {

                Size = new Size(200, 50),
                HorizontalAlignment = HorizontalAlignment.Centre,
                VerticalAlignment = VerticalAlignment.Centre
            };

            passWordTextBox = new TextBox()
            {

                Size = new Size(200, 50),
                HorizontalAlignment = HorizontalAlignment.Centre,
                VerticalAlignment = VerticalAlignment.Centre
            };
            passWordTextBox.TextChanged += PassWordTextBox_TextChanged;



            loginButton = new Button
            {
                Size = new Size(288, 50),
                Content = "Login",
                VerticalAlignment = VerticalAlignment.Centre,
                HorizontalAlignment = HorizontalAlignment.Centre
            };
            loginButton.Clicked += LoginButton_Clicked;

            registerButton = new Button
            {
                Size = new Size(288, 50),
                Content = "Register Now!",
                VerticalAlignment = VerticalAlignment.Centre,
                HorizontalAlignment = HorizontalAlignment.Centre
            };
            registerButton.Clicked += RegisterButton_Clicked;

            errorMessage = new Label(unknownError)
            {
                IsVisible = false,
                TextColor = Color.Red
            };

            tryAgainMessage = new Label(userAlreadyLoginTryAgain)
            {
                IsVisible = false,
                TextColor = Color.Red
            };

            Screen loginScreen = new Screen
            {
                Content = new DockPanel
                {
                    BackgroundRegion = i_storage.LoginPageBackground,
                    Items =
                    {

                        new StackPanel
                        {
                            AttachedProperties = { { DockPanel.DockProperty, Dock.Top } },
                            Size = new Size(100, 100)
                        },
                        new StackPanel
                        {
                            AttachedProperties = { { DockPanel.DockProperty, Dock.Bottom } },
                            Size = new Size(100, 100)
                        },
                        new StackPanel
                        {
                            AttachedProperties = { { DockPanel.DockProperty, Dock.Left } },
                            Size = new Size(100, 100)
                        },
                        new StackPanel
                        {
                            AttachedProperties = { { DockPanel.DockProperty, Dock.Right } },
                            Size = new Size(100, 100)
                        },
                        new StackPanel
                        {
                            Spacing = 5,
                            Orientation = Orientation.Vertical,
                            HorizontalAlignment = HorizontalAlignment.Centre,
                            VerticalAlignment = VerticalAlignment.Centre,
                            Items =
                            {
                                new StackPanel
                                {
                                    Orientation = Orientation.Horizontal,
                                    Size = new Size(70, 70)
                                },
                                new StackPanel
                                {
                                    Margin = 6,
                                    Orientation = Orientation.Horizontal,
                                    Items =
                                    {
                                        new Label("User Name") { Margin = 4 , TextColor = Color.Black, HorizontalAlignment = HorizontalAlignment.Centre, VerticalAlignment = VerticalAlignment.Centre},
                                        userNameTextBox
                                    }
                                },
                                new StackPanel
                                {
                                    Margin = 6,
                                    Orientation = Orientation.Horizontal,
                                    Items =
                                    {
                                        new Label("Password   ") { Margin = 5 , TextColor = Color.Black, HorizontalAlignment = HorizontalAlignment.Centre, VerticalAlignment = VerticalAlignment.Centre},
                                        passWordTextBox

                                    }
                                },
                                new StackPanel
                                {
                                    Margin = 6,
                                    Orientation = Orientation.Horizontal,
                                    Items =
                                    {

                                        loginButton
                                    }
                                },
                                new StackPanel
                                {
                                    Margin = 6,
                                    Orientation = Orientation.Horizontal,
                                    Items =
                                    {
                                        registerButton
                                    }
                                },
                                new StackPanel
                                {
                                    Margin = 6,
                                    Items =
                                    {
                                        errorMessage,
                                        tryAgainMessage
                                    }
                                }
                            }

                        }

                    }
                }
            };

            _guiSystem = new GuiSystem(viewportAdapter, guiRenderer) { ActiveScreen = loginScreen };
        }

        private void PassWordTextBox_OnFocus(object sender, EventArgs e)
        {
            passWordTextBox.BackgroundColor = Color.Green;
        }

        private void PassWordTextBox_TextChanged(object sender, EventArgs e)
        {
            if (passWordTextBox.Text.Length < starsPassword.Length)
            {
                password.Remove(password.Length - 1, 1);
                starsPassword.Remove(starsPassword.Length - 1, 1);
            }
            else if (!passWordTextBox.Text.EndsWith("*") && passWordTextBox.Text.Length > 0)
            {
                password.Append(passWordTextBox.Text[passWordTextBox.Text.Length - 1]);
                starsPassword.Append('*');
                passWordTextBox.Text = starsPassword.ToString();
            }
        }

        private async void LoginButton_Clicked(object sender, EventArgs e)
        {
            if (userNameTextBox.Text.Length == 0 || passWordTextBox.Text.Length == 0)
            {
                errorMessage.Content = enterUsernameAndPassword;
                errorMessage.IsVisible = true;
            }
            else
            {
                errorMessage.IsVisible = false;
                tryAgainMessage.IsVisible = false;

                string loginResponse = await gameManager.server.Login(userNameTextBox.Text, password.ToString());

                if (loginResponse.Contains("loggedIn complete")) // Successful login
                {
                    gameManager.casinoRoom = new CasinoRoom(gameManager, contentManager, storage);
                    gameManager.mainPlayerEmail = userNameTextBox.Text;
                    await gameManager.casinoRoom.Load(_spriteBatch);
                    
                    //gameManager.mainPlayerEmail = userNameTextBox.Text;
                    
                    gameManager.ScreenType = eScreenType.CasinoRoom;
                    userNameTextBox.Text = string.Empty;
                    passWordTextBox.Text = string.Empty;
                    password.Clear();
                    starsPassword.Clear();
                }
                else if (loginResponse.Contains("Incorrect password")) // wrong username or password
                {
                    errorMessage.Content = wrongUserNameOrPassWord;
                    errorMessage.IsVisible = true;
                }
                else if (loginResponse.Contains("User Allready playing")) // user already login
                {
                    errorMessage.Content = userAlreadyLogin;
                    errorMessage.IsVisible = true;
                    tryAgainMessage.IsVisible = true;
                }
                else // general error message
                {
                    errorMessage.Content = unknownError;
                    errorMessage.IsVisible = true;
                }
            }  
        }

        private void RegisterButton_Clicked(object sender, EventArgs e)
        {
            gameManager.ScreenType = eScreenType.RegisterPage;
        }

        public void Update(GameTime i_gameTime)
        {
            _guiSystem.Update(i_gameTime);
            if (userNameTextBox.IsFocused && !isUserNameTextBoxFocused)
            {
                gameManager.showKeyBoard();
            }
            isUserNameTextBoxFocused = userNameTextBox.IsFocused;

            if(passWordTextBox.IsFocused && !isPassWordTextBoxFocused)
            {
                gameManager.showKeyBoard();
            }
            isPassWordTextBoxFocused = passWordTextBox.IsFocused;
        }

        public void Draw(GameTime i_gameTime)
        {
            _guiSystem.Draw(i_gameTime);   
        }

        public void handleKeyboardInput(string i_givenChar)
        {
            if(userNameTextBox.IsFocused)
            {
                if(i_givenChar.Equals("Del"))
                {
                    if(userNameTextBox.Text.Length > 1)
                    {
                        userNameTextBox.Text = userNameTextBox.Text.Remove(userNameTextBox.Text.Length - 1, 1);
                    }
                }
                else
                {
                    userNameTextBox.Text += i_givenChar.ToLower();
                }
            }
            else if(passWordTextBox.IsFocused)
            {
                if(i_givenChar.Equals("Del"))
                {
                    if( passWordTextBox.Text.Length > 1)
                    {
                        passWordTextBox.Text = passWordTextBox.Text.Remove(passWordTextBox.Text.Length - 1, 1);
                    }
                }
                else
                {
                    passWordTextBox.Text += i_givenChar.ToLower();
                }                
            }
        }
    }
}