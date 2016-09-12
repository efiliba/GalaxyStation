using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace GalaxyStation
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game, IGalaxyStationServiceCallback
    {
        private const int SCREEN_COLUMNS = 30;//17;
        private const int SCREEN_ROWS = 16;//12;
        private const int CHAT_WIDTH = 0;
        private const int TILE_WIDTH = 64;
        private const int TILE_HEIGHT = 64;
        private const int SPRITE_WIDTH = 64;
        private const int SPRITE_HEIGHT = 80;
        private const float HORIZONTAL_SCALE = 1f;
        private const float VERTICAL_SCALE = 1f;

        private const int SPAWN_X = 49;//150;
        private const int SPAWN_Y = 49;//100;
        private const int SPEED = 15;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Map map;
        private Tiles tiles;
        private MapItems mapItems;                                                                  // Items placed on the map
        private PlayerItems playerItems;                                                            // Items held/equipped on the player
        private Background background;
        private Player activePlayer;

        private KeyboardState previousKey;

        private cButton btnPlay;
        private enum GameState { MainMenu, InGame }
        private GameState CurrentGameState = GameState.InGame; // GameState.MainMenu;
        private Texture2D mainMenuTexture;
        private Rectangle mainMenuRectangle;

        private int screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 16 - 100;
        private int screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 58 - 100;

//        private GalaxyStationServiceClient galaxyStationClient;
        private int playerID = 0;
        private Data_Contracts.PlayerLocation[] playerLocations;
        private Texture2D otherPlayerTexture;

//        private AtmosphereTest atmosphereTest;
        private Atmosphere gas;

        public enum BuildCycle { Floor, Floor2, Floor3, Floor4, Floor5, Floor6, Floor7, Floor8, Floor9 }
        private BuildCycle buildCycle;

        private int leftBound;
        private int topBound;
        private int rightBound;
        private int bottomBound;

        protected Song song;
        protected Song spacework;
 
        public Game1()
        {
/*            atmosphereTest = new AtmosphereTest();
            atmosphereTest.TestGas0AtStart();

//            atmosphereTest.TestGas100AtTime5Distance0();
//            atmosphereTest.TestGas0AtTime5Distance100();

            atmosphereTest.TestGas0AtTime100Distance0();

            atmosphereTest.TestGas100AtTime1Distance0();
            atmosphereTest.TestGas0AtTime1Distance1();
            atmosphereTest.TestGas0AtTime1Distance2();
*/
            System.ServiceModel.InstanceContext context = new System.ServiceModel.InstanceContext(this);
//            galaxyStationClient = new GalaxyStationServiceClient(context);
//            galaxyStationClient.ClearPlayers();
//            playerID = galaxyStationClient.RegisterPlayer(SPAWN_X, SPAWN_Y);

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            IsMouseVisible = true;
            Window.AllowUserResizing = true;

            // Change window size
            graphics.PreferredBackBufferWidth = SCREEN_COLUMNS * (int)(TILE_WIDTH * HORIZONTAL_SCALE) + CHAT_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_ROWS * (int)(TILE_HEIGHT * VERTICAL_SCALE);
            graphics.ApplyChanges();

//            Window.ClientSizeChanged += new System.EventHandler<System.EventArgs>(Window_ClientSizeChanged);

            map = new Map(@"GalaxyStationContent\Mapdoors.json", TILE_WIDTH, TILE_HEIGHT);
            tiles = new Tiles(map.FloorTiles, map.WallTiles, map.RoofTiles, map.DoorTiles, map.Columns, map.Rows, SCREEN_COLUMNS, SCREEN_ROWS, TILE_WIDTH, TILE_HEIGHT)
            {
                HorizontalScale = HORIZONTAL_SCALE,
                VerticalScale = VERTICAL_SCALE
            };

            activePlayer = new Player(SPAWN_X, SPAWN_Y, SCREEN_COLUMNS / 2, SCREEN_ROWS / 2, SPEED, SPEED >> 1, TILE_WIDTH, TILE_HEIGHT, SPRITE_WIDTH, SPRITE_HEIGHT);
            activePlayer.HorizontalScale = HORIZONTAL_SCALE;
            activePlayer.VerticalScale = VERTICAL_SCALE;
            activePlayer.PlayerClicked += new EventArgs.PlayerClickedEventHandler(activePlayer_PlayerClicked);
            activePlayer.DirectionChanged += new EventArgs.DirectionChangedEventHandler(activePlayer_DirectionChanged);

            mapItems = new MapItems(map.ItemTiles, map.Columns, map.Rows, SCREEN_COLUMNS, SCREEN_ROWS, TILE_WIDTH, TILE_HEIGHT)
            {
                HorizontalScale = HORIZONTAL_SCALE,
                VerticalScale = VERTICAL_SCALE
            };

            playerItems = new PlayerItems(map.ItemTiles, map.Columns, map.Rows, SCREEN_COLUMNS, SCREEN_ROWS, TILE_WIDTH, TILE_HEIGHT)
            {
                HorizontalScale = HORIZONTAL_SCALE,
                VerticalScale = VERTICAL_SCALE,
                HeldColumn = activePlayer.ColumnOffset,
                HeldRow = activePlayer.RowOffset - 1
            };

            background = new Background(SCREEN_COLUMNS * TILE_WIDTH, SCREEN_ROWS * TILE_HEIGHT)
            {
                GameColumns = map.Columns,
                GameRows = map.Rows,
                HorizontalScale = HORIZONTAL_SCALE,
                VerticalScale = VERTICAL_SCALE
            };

            leftBound = SCREEN_COLUMNS / 2 + 1;
            topBound = SCREEN_ROWS / 2 + 1;
            rightBound = 100 - SCREEN_COLUMNS * 3 / 2 - 2;
            bottomBound = 100 - SCREEN_ROWS * 2;

            gas = new Atmosphere(200);
            buildCycle = BuildCycle.Floor;

            mainMenuRectangle = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
        }

        private void Window_ClientSizeChanged(object sender, System.EventArgs e)
        {
            float horizontalScale = tiles.HorizontalScale;
            if (Window.ClientBounds.Width > 0)                                                      // Make sure window not minimised
                horizontalScale *= (Window.ClientBounds.Width - CHAT_WIDTH) / (float)(graphics.PreferredBackBufferWidth - CHAT_WIDTH);
 
            tiles.HorizontalScale = horizontalScale;
            mapItems.HorizontalScale = horizontalScale;
            playerItems.HorizontalScale = horizontalScale;
            background.HorizontalScale = horizontalScale;
            activePlayer.HorizontalScale = horizontalScale;

            float verticalScale = tiles.VerticalScale;
            if (Window.ClientBounds.Height > 0)
                verticalScale *= Window.ClientBounds.Height / (float)graphics.PreferredBackBufferHeight;

            tiles.VerticalScale = verticalScale;
            mapItems.VerticalScale = verticalScale;
            playerItems.VerticalScale = verticalScale;
            background.VerticalScale = verticalScale;
            activePlayer.VerticalScale = verticalScale;

            graphics.PreferredBackBufferWidth = tiles.Width + CHAT_WIDTH;
            graphics.PreferredBackBufferHeight = tiles.Height;
            graphics.ApplyChanges();

            mainMenuRectangle.Width = tiles.Width + CHAT_WIDTH;
            mainMenuRectangle.Height = tiles.Height;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content. Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            //// Set window start position
            //graphics.PreferredBackBufferWidth = screenWidth;
            //graphics.PreferredBackBufferHeight = screenHeight;
            ////System.Windows.Forms.Form.FromHandle(this.Window.Handle).Location = new System.Drawing.Point(10, 10);
            //graphics.ApplyChanges();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            mainMenuTexture = Content.Load<Texture2D>("MainMenu");
            btnPlay = new cButton(Content.Load<Texture2D>("button"), graphics.GraphicsDevice);
            btnPlay.setPosition(new Vector2((graphics.PreferredBackBufferWidth /2 -55), 810));

            otherPlayerTexture = Content.Load<Texture2D>("WalkingUp");

            background.LoadTexture(Content.Load<Texture2D>("background"));
            tiles.Load(GraphicsDevice, map.SpriteSheets);
            mapItems.Load(GraphicsDevice, map.SpriteSheets);
            Player.Load(Content);
            playerItems.Load(Content);

            //Content.Load<Song>("beauty"); 
            song = Content.Load<Song>("beauty");
            spacework = Content.Load<Song>("spacework2");
            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = true;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
//            galaxyStationClient.RemovePlayer(playerID);
//            galaxyStationClient.Close();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            KeyboardState keys = Keyboard.GetState();
            //foreach (Keys key in keys.GetPressedKeys())
            //    switch (key)
            //    {
            //        case Keys.Left: break;
            //        ;
            //    }

            if (keys.IsKeyDown(Keys.Escape))
                Exit();

            MouseState mouse = Mouse.GetState();
            switch (CurrentGameState)
            {
                case GameState.MainMenu:

                    if (Keyboard.GetState().GetPressedKeys().Length > 0)
                    {
                        CurrentGameState = GameState.InGame;
                        MediaPlayer.Stop();
                    }
                        
  
                    btnPlay.Update(mouse);
                    break;

                case GameState.InGame:

                    if (keys.IsKeyDown(Keys.P) && !previousKey.IsKeyDown(Keys.P))
                        activePlayer.ToggleSprint();

                    //activePlayer.Speed = 20;
                    //var s = activePlayer.Speed;

                    Direction? direction = null;
                    if (keys.IsKeyDown(Keys.Up))
                        direction = Direction.Up;
                    else if (keys.IsKeyDown(Keys.Down))
                        direction = Direction.Down;
                    else if (keys.IsKeyDown(Keys.Left))
                        direction = Direction.Left;
                    else if (keys.IsKeyDown(Keys.Right))
                        direction = Direction.Right;

                    else if (keys.IsKeyDown(Keys.G) && !previousKey.IsKeyDown(Keys.G))
                    {
                        gas.Start(activePlayer.Column, activePlayer.Row);
                    }
                    else if (keys.IsKeyDown(Keys.S) && !previousKey.IsKeyDown(Keys.S))
                    {
                        gas.Stop();
                    }

                    else if (keys.IsKeyDown(Keys.M) && !previousKey.IsKeyDown(Keys.M))
                    {
                        MediaPlayer.Play(spacework);
                    }

                    else if (keys.IsKeyDown(Keys.N) && !previousKey.IsKeyDown(Keys.N))
                    {
                        MediaPlayer.Stop();
                    }

                    else if (keys.IsKeyDown(Keys.B) && !previousKey.IsKeyDown(Keys.B))
                    {
                        buildCycle++;
                        if ((int)buildCycle >= Enum.GetNames(typeof(BuildCycle)).Length)
                            buildCycle = BuildCycle.Floor;
                    }

                    // Movement key pressed and player's state updated and no tiles nor other players at perspective location
                    if (direction != null && activePlayer.UpdateState(gameTime, (Direction)direction) &&
                            WithinBounds(activePlayer.Column, activePlayer.Row, (Direction)direction) &&
                            !Obstructed(activePlayer.Column, activePlayer.Row, (Direction)direction))// && !players.Collided((Direction)direction))
                    {
                        activePlayer.Move((Direction)direction);
                    }

                    Window.Title = "Effect: " + gas.Effect(activePlayer.Column, activePlayer.Row, 0, true).ToString();

//                    playerLocations = galaxyStationClient.GetPlayerLocations(playerID);


                    if (ClickOnScreen(mouse) && mouse.LeftButton == ButtonState.Pressed)
                    {
                        //Window.Title = "pressed X: " + mouse.X + "  Y: " + mouse.Y;
//                        if (direction != null)
//                            map.DestroyTile(activePlayer.Column, activePlayer.Row, (Direction)direction);
                        try
                        {
                            //map.ToggleDoor(activePlayer.RelativeColumn + mouse.X / TILE_WIDTH, activePlayer.RelativeRow + mouse.Y / TILE_HEIGHT);

                            map.DestroyTile(activePlayer.RelativeColumn + mouse.X / TILE_WIDTH, activePlayer.RelativeRow + mouse.Y / TILE_HEIGHT, Keyboard.GetState());
                            playerItems[1].Held = true;
                        }
                        catch
                        {
                            // Log error
                        }
                    }
                    else
                        Window.Title = "Effect: " + gas.Effect(activePlayer.Column, activePlayer.Row, 0, true).ToString();

                    Window.Title = buildCycle.ToString() + "                                                           Effect:" + gas.Effect(activePlayer.Column, activePlayer.Row, 0, true).ToString();

                    if (ClickOnScreen(mouse) && mouse.RightButton == ButtonState.Pressed)
                    {
                        try
                        {
                            map.BuildTile(activePlayer.RelativeColumn + mouse.X / TILE_WIDTH, activePlayer.RelativeRow + mouse.Y / TILE_HEIGHT, Keyboard.GetState(), buildCycle);
                        }
                        catch
                        {
                            // Log error
                        }
                    }

                    previousKey = keys;

                    //players2.Update(gameTime);
                    break;
            }

            base.Update(gameTime);
        }

        private bool ClickOnScreen(MouseState mouse)
        {
            return this.IsActive && mouse.X > 0 && mouse.Y > 0 && mouse.X < graphics.PreferredBackBufferWidth && mouse.Y < graphics.PreferredBackBufferHeight;
        }

        void activePlayer_PlayerClicked(object sender, EventArgs.PlayerClickedEventArgs e)
        {
           
        }

        void activePlayer_DirectionChanged(object sender, EventArgs.DirectionChangedEventArgs e)
        {
            playerItems.Direction = e.Direction;
        }

        void IGalaxyStationServiceCallback.PlayerLocationsCallback(Data_Contracts.PlayerLocation[] locations)
        {
            playerLocations = locations;
        }

        private bool WithinBounds(int column, int row, Direction direction)
        {
            return (column > leftBound || direction != Direction.Left) &&
                (row > topBound || direction != Direction.Up) &&
                (column < rightBound || direction != Direction.Right) &&
                (row < bottomBound || direction != Direction.Down);
        }

        public bool Obstructed(int column, int row, Direction direction)
        {
            switch (direction)
            {
                case Direction.Up: return tiles.Solid(column, row - 1);
                case Direction.Down: return tiles.Solid(column, row + 1);
                case Direction.Left: return tiles.Solid(column - 1, row);
                case Direction.Right: return tiles.Solid(column + 1, row);
                default: throw new System.Exception("Unexpected direction: " + direction.ToString());
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            if (CurrentGameState == GameState.InGame)
            {
                spriteBatch.Begin();
                GraphicsDevice.Clear(new Color(195, 195, 195));

                background.Draw(spriteBatch, activePlayer.Column, activePlayer.Row);
                tiles.Draw(spriteBatch, activePlayer.RelativeColumn, activePlayer.RelativeRow, gas);
                mapItems.Draw(spriteBatch, activePlayer.RelativeColumn, activePlayer.RelativeRow);
                activePlayer.Draw(spriteBatch);
                playerItems.Draw(spriteBatch, activePlayer.RelativeColumn, activePlayer.RelativeRow);
                // Redraw tile at player's position if it is a roof tile - i.e. draw player under tile 
                tiles.DrawRoofTile(spriteBatch, activePlayer.Column, activePlayer.Row, activePlayer.ColumnOffset, activePlayer.RowOffset);

                if (playerLocations != null)
                    foreach (var player in playerLocations)
                        if (player.ID != playerID)
                        {
                            Rectangle source = new Rectangle(0, 0, SPRITE_WIDTH, SPRITE_HEIGHT);
                            Rectangle destination = new Rectangle((player.Column - activePlayer.Column + SCREEN_COLUMNS / 2) * TILE_WIDTH,
                                    (player.Row - activePlayer.Row + SCREEN_ROWS / 2) * TILE_HEIGHT - SPRITE_HEIGHT + (TILE_HEIGHT >> 1), TILE_WIDTH, SPRITE_WIDTH);
                            spriteBatch.Draw(otherPlayerTexture, destination, source, Color.Red);

                            // Redraw tile at player's position if it is a roof tile - i.e. draw player under tile 
                            tiles.DrawRoofTile(spriteBatch, player.Column, player.Row, player.Column - activePlayer.Column + activePlayer.ColumnOffset, player.Row - activePlayer.Row + activePlayer.RowOffset);
                        }


             //   spriteBatch.DrawString(new SpriteFont, "Cannon power: 100", new Vector2(graphics.PreferredBackBufferWidth / 2 - 50, 40), Color.White);

                spriteBatch.End();
            }
            else
            {
                spriteBatch.Begin();
                spriteBatch.Draw(mainMenuTexture, mainMenuRectangle, Color.White);
                btnPlay.Draw(spriteBatch);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        private void players_PlayersCollided(object sender, EventArgs.PlayersCollidedEventArgs e)
        {
            if (!Obstructed(e.Player.Column, e.Player.Row, e.Direction))
            {
                e.Player.StartAnimation(e.Direction);
                //                e.Player.Move(e.Direction);
            }
            else
            {
                e.Player.StartAnimation(Opposite(e.Direction));
                //                players.Active.Move(e.Direction);
            }

            //            players.Active.Move(e.Direction);
        }

        private void players_AttackPlayer(object sender, EventArgs.AttackPlayerEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private Direction Opposite(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up: return Direction.Down;
                case Direction.Down: return Direction.Up;
                case Direction.Left: return Direction.Right;
                case Direction.Right: return Direction.Left;
                default: throw new System.Exception("Unexpected direction: " + direction.ToString());
            }
        }
    }
}
