using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GalaxyStation.EventArgs;

namespace GalaxyStation
{
    public class Players
    {
        public event PlayersCollidedEventHandler PlayersCollided;
        public event AttackPlayerEventHandler AttackPlayer;

        private System.Collections.Generic.IList<Player> players;

        private int tileWidth;
        private int tileHeight;
        private int spriteWidth;
        private int spriteHeight;
        private int screenColumn;                                                                   // Column relative to the left of the screen (view port)
        private int screenRow;
        private int scaledWidth;
        private int scaledHeight;

        private Player collidedWith;                                                                // Other player collided with

        public Players(int tileWidth, int tileHeight, int spriteWidth, int spriteHeight, int screenColumn, int screenRow)
        {
            players = new System.Collections.Generic.List<Player>();
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;
            this.spriteWidth = spriteWidth;
            this.spriteHeight = spriteHeight;
            this.screenColumn = screenColumn;
            this.screenRow = screenRow;
            scaledWidth = tileWidth;
            scaledHeight = tileHeight;

            collidedWith = null;
        }

        public void Update(GameTime gameTime)
        {
            if (collidedWith != null && !collidedWith.EffectedUpdate(gameTime))                     // Call collidees' EffectedUpdate method until effect completed
                collidedWith = null;

            ActivePlayer.Update(gameTime);
        }

        public void Add(int startColumn, int startRow, int sprintSpeed, int walkSpeed, Color? tint = null)
        {
            players.Add(new Player(startColumn, startRow, screenColumn, screenRow, sprintSpeed, walkSpeed, tileWidth, tileHeight, spriteWidth, spriteHeight));
            if (tint != null)
                players[players.Count - 1].Tint = (Color)tint;
        }

        public Player ActivePlayer { get; private set; }

        public void SetActivePlayer(int index)
        {
            ActivePlayer = players[index];
        }

        public float HorizontalScale
        {
            set
            {
                scaledWidth = (int)(tileWidth * value);

                foreach (Player player in players)
                    player.HorizontalScale = value;
            }
        }

        public float VerticalScale
        {
            set
            {
                scaledHeight = (int)(tileHeight * value);

                foreach (Player player in players)
                    player.VerticalScale = value;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw all players with respect to the active player's location - if within bounds
            foreach (Player player in players)
            {
                int columnOffset = player.Column - ActivePlayer.Column;
                if (columnOffset <= screenColumn && columnOffset >= -screenColumn)
                {
                    int rowOffset = player.Row - ActivePlayer.Row;
                    if (rowOffset <= screenRow && rowOffset >= -screenRow)
                    {
                        player.Offset = new Point(columnOffset * scaledWidth, rowOffset * scaledHeight);
                        player.Draw(spriteBatch);
                    }
                }
            }
        }

        public bool Collided(Direction direction)
        {

            switch (direction)
            {
                case Direction.Up: return CollisionAt(ActivePlayer.Column, ActivePlayer.Row - 1, direction);
                case Direction.Down: return CollisionAt(ActivePlayer.Column, ActivePlayer.Row + 1, direction);
                case Direction.Left: return CollisionAt(ActivePlayer.Column - 1, ActivePlayer.Row, direction);
                case Direction.Right: return CollisionAt(ActivePlayer.Column + 1, ActivePlayer.Row, direction);
                default: throw new System.Exception("Unexpected direction: " + direction.ToString());


            }


        }

        private bool CollisionAt(int column, int row, Direction direction)
        {
            bool collided = false;
            int index = 0;
            while (!collided && index < players.Count)
            {
                collided = players[index].Column == column && players[index].Row == row;
                index++;
            }

            if (collided)
                OnPlayersCollided(new PlayersCollidedEventArgs(collidedWith = players[index - 1], direction));

            return collided;
        }

        private void OnPlayersCollided(PlayersCollidedEventArgs e)
        {
            if (PlayersCollided != null)
                PlayersCollided(this, e);
        }

        private void OnAttackPlayer(AttackPlayerEventArgs e)
        {
            if (AttackPlayer != null)
                AttackPlayer(this, e);
        }
    }
}
