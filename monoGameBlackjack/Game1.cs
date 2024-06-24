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


        Player player;
        public enum GameState
        {
            menu,
            game
        }


        private GameState gState;   

        private Texture2D Testtexture;




        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            player = new Player();
            player.intialize();
            tester = new Rectangle(100,100,100,100);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Testtexture = Content.Load<Texture2D>($"{2}_clubs");
            player.LoadContent(Content);
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            MouseState currentMouseState = Mouse.GetState();
            // TODO: Add your update logic here


            if (currentMouseState.LeftButton == ButtonState.Released &&
                prev.LeftButton == ButtonState.Pressed &&
                    tester.Contains(currentMouseState.Position))
            {
                tester.X += 80;
            }

            prev = currentMouseState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Green);
            _spriteBatch.Begin();

            _spriteBatch.Draw(Testtexture, tester,Color.White);
            // TODO: Add your drawing code here

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}