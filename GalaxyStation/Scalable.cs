using Microsoft.Xna.Framework.Graphics;

namespace GalaxyStation
{
    public class Scalable
    {
        protected Texture2D[] spriteSheets;
        protected int totalColumns;                                                                 // Number of horizontal tiles
        protected int totalRows;                                                                    // Number of vertical tiles
        protected int displayColumns;                                                               // Number of horizontal tiles to display
        protected int displayRows;                                                                  // Number of vertical tiles to display
        private int tileWidth;
        private int tileHeight;

        protected float horizontalScale;
        protected float verticalScale;
        protected int scaledWidth;
        protected int scaledHeight;

        public Scalable(int totalColumns, int totalRows, int displayColumns, int displayRows, int tileWidth, int tileHeight)
        {
            this.totalColumns = totalColumns;
            this.totalRows = totalRows;
            this.displayColumns = displayColumns;
            this.displayRows = displayRows;
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;

            horizontalScale = 1f;
            verticalScale = 1f;
            scaledWidth = tileWidth;
            scaledHeight = tileHeight;
        }

        public int Width
        {
            get { return displayColumns * scaledWidth; }
        }

        public int Height
        {
            get { return displayRows * scaledHeight; }
        }

        public float HorizontalScale
        {
            get { return horizontalScale; }
            set
            {
                horizontalScale = value;
                scaledWidth = (int)(tileWidth * value);
            }
        }

        public float VerticalScale
        {
            get { return verticalScale; }
            set
            {
                verticalScale = value;
                scaledHeight = (int)(tileHeight * value);
            }
        }

        public void Load(GraphicsDevice graphicsDevice, SpriteSheet[] spriteSheetInfos)
        {
            // Load all the sprite sheets
            spriteSheets = new Texture2D[spriteSheetInfos.Length];
            foreach (SpriteSheet spriteSheetInfo in spriteSheetInfos)
                using (System.IO.FileStream stream = new System.IO.FileStream(spriteSheetInfo.Path, System.IO.FileMode.Open))
                    spriteSheets[spriteSheetInfo.Index] = Texture2D.FromStream(graphicsDevice, stream);
        }
    }
}
