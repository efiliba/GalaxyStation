using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GalaxyStation
{
    public class Background
    {
        private Texture2D texture;


        //private Tile[,] tiles;
        //private int displayColumns;
        //private int displayRows;
        //private int tileWidth;
        //private int tileHeight;

//        private float horizontalScale;
//        private float verticalScale;
        private int unscaledWidth;
        private int unscaledHeight;
        private Rectangle sourceRectangle;
        private Rectangle destinationRectangle;

        private float inverseGameColumns;                                                           // Number of columns / rows in the game / world
        private float inverseGameRows;

        public Background(int viewportWidth, int viewportHeight)
        {
            //this.tiles = tiles;
            //displayColumns = columns;
            //displayRows = rows;
            //this.tileWidth = tileWidth;
            //this.tileHeight = tileHeight;

//            horizontalScale = 1f;
//            verticalScale = 1f;
            unscaledWidth = viewportWidth;
            unscaledHeight = viewportHeight;
            sourceRectangle = new Rectangle(0, 0, viewportWidth, viewportHeight);
            destinationRectangle = new Rectangle(0, 0, viewportWidth, viewportHeight);
        }

        public void LoadTexture(Texture2D texture)
        {
            this.texture = texture;
        }

        public int GameColumns
        {
            get { return (int)(1 / inverseGameColumns); }
            set { inverseGameColumns = 1f / value; }
        }

        public int GameRows
        {
            get { return (int)(1 / inverseGameRows); }
            set { inverseGameRows = 1f / value; }
        }

        public float HorizontalScale
        {
            set { destinationRectangle.Width = (int)(unscaledWidth * value); }
        }

        public float VerticalScale
        {
            set { destinationRectangle.Height = (int)(unscaledHeight * value); }
        }

        public void Draw(SpriteBatch spriteBatch, int relativeColumn, int relativeRow)
        {
            //int xOffset = (int)(relativeColumn * 15 * inverseGameColumns);
            //int yOffset = (int)(relativeRow * 15 * inverseGameRows);
            //int offsetColumn = xOffset / 64;
            //int offsetRow = yOffset / 64;
            //int offsetWidth = (int)(xOffset % 64 * 1);
            //int offsetHeight = (int)(yOffset % 64 * 1);

            sourceRectangle.X = (int)(relativeColumn * texture.Bounds.Width * inverseGameColumns);
            sourceRectangle.Y = (int)(relativeRow * texture.Bounds.Height * inverseGameRows);
            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, Color.White);
        }
        
        //public void LoadSpriteSheet(GraphicsDevice graphicsDevice, SpriteSheet spriteSheet)
        //{
        //    // Load sprite sheet containig background tiles
        //    using (System.IO.FileStream stream = new System.IO.FileStream(spriteSheet.Path, System.IO.FileMode.Open))
        //        texture = Texture2D.FromStream(graphicsDevice, stream);
        //}

/*       

        public void Draw(SpriteBatch spriteBatch, int relativeColumn, int relativeRow)
        {
            Rectangle sourceRectangle;
            int xOffset = (int)(relativeColumn * tiles.GetLength(0) * inverseGameColumns);
            int yOffset = (int)(relativeRow * tiles.GetLength(1) * inverseGameRows);
            int offsetColumn = xOffset / tileWidth;
            int offsetRow = yOffset / tileHeight;
            int offsetWidth = (int)(xOffset % tileWidth * HorizontalScale);
            int offsetHeight = (int)(yOffset % tileHeight * VerticalScale);

            for (int row = 0; row <= displayRows; row++)
            {
                Rectangle destinationRectangle = new Rectangle(-offsetWidth, row * scaledHeight - offsetHeight, scaledWidth, scaledHeight);
                for (int column = 0; column < displayColumns; column++)
                {
                    sourceRectangle = tiles[row + offsetRow, column + offsetColumn].SourceRectangle;
                    spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, Color.White);
                    destinationRectangle.Offset(scaledWidth, 0);
                }

                sourceRectangle = tiles[row + offsetRow, displayColumns + offsetColumn].SourceRectangle;
                sourceRectangle.Width = offsetWidth * tileWidth / scaledWidth;
                destinationRectangle.Width = offsetWidth;
                spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, Color.White);
            }
        }
*/    }
}