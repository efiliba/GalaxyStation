namespace GalaxyStation
{
    public class SpriteSheet
    {
        public string Path { get; set; }
        public int Index { get; set; }
        public int FirstGid { get; set; }
        public int Columns { get; private set; }
        public Microsoft.Xna.Framework.Rectangle[] SourceRectangles { get; private set; }

        public SpriteSheet(int imageWidth, int imageHeight, int imageTileWidth, int imageTileHeight, int tileWidth, int tileHeight)
        {
            Columns = (int)System.Math.Ceiling(imageWidth / (double)imageTileWidth);
            int rows = (int)System.Math.Ceiling(imageHeight / (double)imageTileHeight);

            SourceRectangles = new Microsoft.Xna.Framework.Rectangle[Columns * rows];
            int index = 0;
            for (int row = 0, yPos = 0; row < rows; row++, yPos += tileHeight)
                for (int column = 0, xPos = 0; column < Columns; column++, xPos += tileWidth)
                    SourceRectangles[index++] = new Microsoft.Xna.Framework.Rectangle(xPos, yPos, tileWidth, tileHeight);
        }

        public Microsoft.Xna.Framework.Rectangle SourceRectangleByGid(int gid)
        {
            return SourceRectangles[gid - FirstGid];
        }
    }
}
