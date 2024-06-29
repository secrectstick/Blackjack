using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace monoGameBlackjack
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;


        private Rectangle tester;
        private MouseState prev;
        private MouseState currentMouseState;

        Player player;
        public enum GameState
        {
            menu,
            game
        }


        private GameState gState;   

        private Texture2D Testtexture;
        private Texture2D playUp;
        private Texture2D playDown;
        private Texture2D endUp;
        private Texture2D endDown;


        private Rectangle startButton;
        private Rectangle endButton;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 800;
            _graphics.ApplyChanges();

            player = new Player();
            player.intialize();
            tester = new Rectangle(100,100,100,100);
            startButton =new Rectangle(350,150,50,50);
            endButton = new Rectangle(350,225,50,50);
            gState = GameState.menu;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Testtexture = Content.Load<Texture2D>($"{2}_clubs");
            playDown = Content.Load<Texture2D>("PlayClick");
            playUp = Content.Load<Texture2D>("PlayBtn");
            endUp = Content.Load<Texture2D>("ExitClick");
            endDown = Content.Load<Texture2D>("ExitBtn");
            player.LoadContent(Content);
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            currentMouseState = Mouse.GetState();
            // TODO: Add your update logic here

            switch (gState)
            {
                case GameState.menu:

                    if (currentMouseState.LeftButton == ButtonState.Released &&
                    prev.LeftButton == ButtonState.Pressed &&
                    startButton.Contains(currentMouseState.Position))
                    {
                        gState = GameState.game;
                    }

                    if (currentMouseState.LeftButton == ButtonState.Released &&
                    prev.LeftButton == ButtonState.Pressed &&
                    endButton.Contains(currentMouseState.Position))
                    {
                        Exit();
                    }

                    break;
                case GameState.game:
                    player.update();
                    break;
            }


            

            prev = currentMouseState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Green);
            _spriteBatch.Begin();

            switch (gState)
            {
                case GameState.menu:
                    if (startButton.Contains(currentMouseState.Position))
                    {
                        _spriteBatch.Draw(playDown, startButton, Color.White);
                    }
                    else
                    {
                        _spriteBatch.Draw(playUp, startButton, Color.White);
                    }

                    if (endButton.Contains(currentMouseState.Position))
                    {
                        _spriteBatch.Draw(endDown, endButton, Color.White);
                    }
                    else
                    {
                        _spriteBatch.Draw(endUp, endButton, Color.White);
                    }


                    break; 
                case GameState.game:
                    //_spriteBatch.Draw(Testtexture, tester, Color.White);
                    player.draw(_spriteBatch);
                    break;
            }

            
            // TODO: Add your drawing code here

            

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}