using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Server;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Casino
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static int PreferedScreenWidth = 1280;
        public static int PreferedScreenHeight = 720;

        public static int UserScreenWidth { get; set; }
        public static int UserScreenHeight { get; set; }

        private SpritesStorage storage;

        public Proxy server;

        public string mainPlayerEmail;

        public eScreenType ScreenType { get; set; } = eScreenType.LoginPage;

        private LoginPage loginPage;
        private RegisterPage registerPage;
        public CasinoRoom casinoRoom;
        public PokerTable pokerTable;

        public static List<CharcterSprite> listOfSprites = new List<CharcterSprite>();

        public delegate void KeyboardAction();
        public KeyboardAction showKeyBoard;
        public KeyboardAction hideKeyBoard;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            listOfSprites.Add(new CharcterSprite(PlayerSkin.Ninja, 75, 100));
            listOfSprites.Add(new CharcterSprite(PlayerSkin.Jack, 75, 100));
            listOfSprites.Add(new CharcterSprite(PlayerSkin.Knight, 85, 130));
            listOfSprites.Add(new CharcterSprite(PlayerSkin.Zombie, 75, 100));
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            _graphics.PreferredBackBufferWidth = PreferedScreenWidth;
            _graphics.PreferredBackBufferHeight = PreferedScreenHeight;
            _graphics.ApplyChanges();

            UserScreenWidth = GraphicsDevice.PresentationParameters.Bounds.Width;
            UserScreenHeight = GraphicsDevice.PresentationParameters.Bounds.Height;

            storage = new SpritesStorage(Content, UserScreenHeight, UserScreenWidth);

            server = new Proxy();

            loginPage = new LoginPage(this, GraphicsDevice, Content);
            registerPage = new RegisterPage(this, GraphicsDevice, Content);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            storage.Load();
            loginPage.Load(_spriteBatch, storage);
            registerPage.Load(_spriteBatch, storage);

            // TODO: use this.Content to load your game content here
        }

        protected async override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            switch (ScreenType)
            {
                case eScreenType.LoginPage:
                    updateLoginPage(gameTime);
                    break;
                case eScreenType.RegisterPage:
                    registerPage.Update(gameTime);
                    break;
                case eScreenType.CasinoRoom:
                    await updateCasinoRoom(gameTime);
                    break;
                case eScreenType.PokerTable:
                    updatePokerTable(gameTime);
                    break;
                default:
                    break;
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here

            switch (ScreenType)
            {
                case eScreenType.LoginPage:
                    drawLoginPage(gameTime);
                    break;
                case eScreenType.RegisterPage:
                    registerPage.Draw(gameTime);
                    break;
                case eScreenType.CasinoRoom:
                    drawCasinoRoom(gameTime);
                    break;
                case eScreenType.PokerTable:
                    drawPokerTable(gameTime);
                    break;
                default:
                    break;
            }

            base.Draw(gameTime);
        }

        private void drawCasinoRoom(GameTime i_gameTime)
        {
            if (casinoRoom != null)
            {
                _spriteBatch.Begin(transformMatrix: casinoRoom.camera.Transform);

                casinoRoom.Draw(i_gameTime);

                _spriteBatch.End();
            }
        }

        private async Task updateCasinoRoom(GameTime i_GameTime)
        {
            if (casinoRoom == null)
            {
                casinoRoom = new CasinoRoom(this, Content, storage);
                await casinoRoom.Load(_spriteBatch);
            }
            if (casinoRoom.isReEnterToCasino)
            {
                casinoRoom.isReEnterToCasino = false;
                casinoRoom.UpdateMainPlayer(mainPlayerEmail);
            }
            casinoRoom.IsUpdated = true;
            casinoRoom.Update(i_GameTime);
        }

        private void drawPokerTable(GameTime i_GameTime)
        {
            _spriteBatch.Begin();

            pokerTable.Draw(i_GameTime);

            _spriteBatch.End();
        }

        private void updatePokerTable(GameTime i_GameTime)
        {
            pokerTable.Update(i_GameTime);
        }

        private void drawLoginPage(GameTime i_gameTime)
        {
            loginPage.Draw(i_gameTime);
        }

        private void updateLoginPage(GameTime i_gameTime)
        {
            loginPage.Update(i_gameTime);
        }

        public void handleKeyboardInput(string i_givenChar)
        {
            switch (ScreenType)
            {
                case eScreenType.LoginPage:
                    loginPage.handleKeyboardInput(i_givenChar);
                    break;
                case eScreenType.RegisterPage:
                    registerPage.handleKeyboardInput(i_givenChar);
                    break;
                case eScreenType.CasinoRoom:
                    casinoRoom.handleKeyboardInput(i_givenChar);
                    break;
                case eScreenType.PokerTable:
                    pokerTable.handleKeyboardInput(i_givenChar);
                    break;
            }
        }
    }
}
