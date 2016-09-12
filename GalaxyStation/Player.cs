using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using GalaxyStation.EventArgs;

namespace GalaxyStation
{
    public enum Direction { Up, Down, Left, Right };

    public class Player
    {
        public event PlayerClickedEventHandler PlayerClicked;
        public event DirectionChangedEventHandler DirectionChanged;

        private static Texture2D[] images;
        private int animationStep;

        private float sprintSpeed;
        private float walkSpeed;
        private float speed;                                                                        // Current movement speed - may be set to the walk or sprint speed
        private bool sprint;
        private int tileWidth;
        private int tileHeight;
        private int spriteWidth;                                                                    // Width of each animated player sprite in the sprite sheet
        private int spriteHeight;
        private float horizontalScale;
        private float verticalScale;

        private Rectangle scaledDestination;                                                        // Player to draw's base location rectangle (with scale applied)
        private Rectangle offsetDestination;                                                        // Offset applied to the player's location rectangle
        private Rectangle animatedDestination;
        private Point offset;                                                                       // Offset to player's location rectangle
        private Direction direction;                                                                // Direction player is facing
        private System.TimeSpan previousTime;                                                       // Calculate duration between calls to Update(..) to control Player's speed 
        private long elapsedTime;

        private bool startAnimation;
        private Direction animationDirection;
        private int animationMovement;

        public int Column { get; set; }                                                             // Player's current 'game' column (world column location)
        public int Row { get; set; }
        public int ColumnOffset { get; set; }                                                       // Column relative to the left of the screen (view port)
        public int RowOffset { get; set; }
        public Color Tint { get; set; }

        public Player(int startColumn, int startRow, int columnOffset, int rowOffset, int sprintSpeed, int walkSpeed, int tileWidth, int tileHeight, int spriteWidth, int spriteHeight)
        {
            Column = startColumn;
            Row = startRow;
            ColumnOffset = columnOffset;
            RowOffset = rowOffset;
            Tint = Color.White;

            sprint = true;
            this.sprintSpeed = sprintSpeed;
            this.walkSpeed = walkSpeed;
            this.speed = 1f / sprintSpeed;
            direction = Direction.Down;

            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;
            this.spriteWidth = spriteWidth;
            this.spriteHeight = spriteHeight;
            horizontalScale = 1f;
            verticalScale = 1f;

            offset = new Point(0, 0);
            scaledDestination = new Rectangle(columnOffset * tileWidth, rowOffset * tileHeight - spriteHeight + (tileHeight >> 1), tileWidth, spriteHeight);
            animatedDestination = offsetDestination = scaledDestination;
            images = new Texture2D[4];
            animationStep = 0;
            previousTime = new System.TimeSpan();
            elapsedTime = 0;
        }

        public float GetSpeed()
        {
            return speed;
        }

        public void SetSpeed(float value)
        {
            speed = value;
        }

        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        public void Update(GameTime gameTime)
        {
            // MouseState mouse = Mouse2;
        }

        public float HorizontalScale
        {
            set
            {
                horizontalScale = value;
                SetDestinationRectangle();
            }
        }

        public float VerticalScale
        {
            set
            {
                verticalScale = value;
                SetDestinationRectangle();
            }
        }

        public Point Offset
        {
            get { return offsetDestination.Location; }
            set
            {
                offsetDestination = scaledDestination;                                              // Reset to base scaled destination rectangle
                offsetDestination.Offset(offset = value);
            }
        }

        public void StartAnimation(Direction direction)
        {
            animatedDestination = offsetDestination;                                                // Reset animation destination rectangle
            animationDirection = direction;
            animationMovement = 0;                                                                  // Amount moved since animation started
            startAnimation = true;

            Move(direction);                                                                        // Set location to move to
        }

        private void SetDestinationRectangle()
        {
            int scaledTileWidth = (int)(tileWidth * horizontalScale);
            int scaledTileHeight = (int)(tileHeight * verticalScale);
            int scaledSpriteWidth = (int)(spriteWidth * horizontalScale);
            int scaledSpriteHeight = (int)(spriteHeight * verticalScale);

            scaledDestination = new Rectangle(ColumnOffset * scaledTileWidth, RowOffset * scaledTileHeight - scaledSpriteHeight + (scaledTileHeight >> 1), scaledTileWidth, scaledSpriteHeight);
            offsetDestination = scaledDestination;
            offsetDestination.Offset(offset);
            animatedDestination = offsetDestination;
        }

        public int RelativeColumn                                                                   // Screen left most column 
        {
            get { return Column - ColumnOffset; }
        }

        public int RelativeRow
        {
            get { return Row - RowOffset; }
        }

        public static void Load(ContentManager contentManager)
        {
            foreach (Direction direction in System.Enum.GetValues(typeof(Direction)))
                images[(int)direction] = contentManager.Load<Texture2D>("Walking" + direction);
        }

        public bool UpdateState(GameTime gameTime, Direction direction)
        {
            bool updated = false;
            elapsedTime += gameTime.TotalGameTime.Subtract(previousTime).Ticks;
            previousTime = gameTime.TotalGameTime;
            if (elapsedTime > 10000000 * speed)
            {
                updated = true;
                elapsedTime = 0;
                if (this.direction != direction)
                {
                    this.direction = direction;
                    animationStep = 0;
                    OnDirectionChanged(new DirectionChangedEventArgs(direction));
                }
                else
                    animationStep = (animationStep + 1) % 1;
            }

            return updated;
        }

        public bool EffectedUpdate(GameTime gameTime)
        {
            switch (animationDirection)
            {
                case Direction.Up:
                    animatedDestination.Offset(0, -1);
                    startAnimation = ++animationMovement < tileHeight * verticalScale;
                    break;
                case Direction.Down:
                    animatedDestination.Offset(0, 1);
                    startAnimation = ++animationMovement < tileHeight * verticalScale;
                    break;
                case Direction.Left:
                    animatedDestination.Offset(-1, 0);
                    startAnimation = ++animationMovement < tileWidth * horizontalScale;
                    break;
                case Direction.Right:
                    animatedDestination.Offset(1, 0);
                    startAnimation = ++animationMovement < tileWidth * horizontalScale;
                    break;
            }

            return startAnimation;
        }

        public void Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up: Row--; break;
                case Direction.Down: Row++; break;
                case Direction.Left: Column--; break;
                case Direction.Right: Column++; break;
            }
        }

        public void SetLocation(int column, int row)
        {
            Column = column;
            Row = row;
        }

        public void ToggleSprint()
        {
            sprint = !sprint;
            speed = 1f / (sprint ? sprintSpeed : walkSpeed);

            OnPlayerClicked(new PlayerClickedEventArgs());
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle source = new Rectangle(spriteWidth * animationStep, 0, spriteWidth, spriteHeight);
            Rectangle destination = startAnimation ? animatedDestination : offsetDestination;
            spriteBatch.Draw(images[(int)direction], destination, source, Tint);
        }

        private void OnPlayerClicked(PlayerClickedEventArgs e)
        {
            if (PlayerClicked != null)
                PlayerClicked(this, e);
        }

        private void OnDirectionChanged(DirectionChangedEventArgs e)
        {
            if (DirectionChanged != null)
                DirectionChanged(this, e);
        }
    }
}
