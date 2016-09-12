using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GalaxyStation
{
    public class Tile
    {
        public int FirstGid { get; set; }
        public int Gid { get; set; }

        public int SpriteSheetNumber { get; set; }                                                  // Index of the sprite sheet to use
        public Rectangle SourceRectangle { get; set; }                                              // Source rectangle from sprite sheet
        public bool Solid { get; set; }
        public bool Roof { get; set; }
    }

    public class Tiles : Scalable
    {
        private Tile[,] floorTiles;
        private Tile[,] wallTiles;
        private Tile[,] roofTiles;
        private Tile[,] doorTiles;

        public Tiles(Tile[,] floorTiles, Tile[,] wallTiles, Tile[,] roofTiles, Tile[,] doorTiles, int totalColumns, int totalRows, int displayColumns, int displayRows, int tileWidth, int tileHeight) :
                base(totalColumns, totalRows, displayColumns, displayRows, tileWidth, tileHeight)
        {
            this.floorTiles = floorTiles;
            this.wallTiles = wallTiles;
            this.roofTiles = roofTiles;
            this.doorTiles = doorTiles;
        }

        public bool Solid(int column, int row)
        {
            bool result;
            try
            {
                result = wallTiles[column, row] != null && wallTiles[column, row].Solid;
                result = result || roofTiles[column, row] != null && roofTiles[column, row].Solid || doorTiles[column, row] != null && doorTiles[column, row].Solid;
            }
            catch (System.IndexOutOfRangeException)
            {
                result = true;
            }
            return result;
        }

        public void Draw(SpriteBatch spriteBatch, int columnOffset, int rowOffset, Atmosphere gas)
        {
            Rectangle destinationRectangle = new Rectangle(0, 0, scaledWidth, scaledHeight);
            int endRow = System.Math.Min(rowOffset + displayRows, totalRows);
            int endColumn = System.Math.Min(columnOffset + displayColumns, totalColumns);
            for (int row = System.Math.Max(rowOffset, 0); row < endRow; row++)
            {
                destinationRectangle.Location = new Point(0, (row - rowOffset) * scaledHeight);     // Reset destination rectangle's start postion
                for (int column = System.Math.Max(columnOffset, 0); column < endColumn; column++)
                {
                    int gasEffect = 255 - (int)(gas.Effect(column, row, 0, true) * 3.5);
                    Color damageColour = new Color(gasEffect, 255, 255);
                    Tile floorTile = floorTiles[column, row];
                    if (floorTile != null)
                        spriteBatch.Draw(spriteSheets[floorTile.SpriteSheetNumber], destinationRectangle, floorTile.SourceRectangle, damageColour);

                    Tile wallTile = wallTiles[column, row];
                    if (wallTile != null)
                        spriteBatch.Draw(spriteSheets[wallTile.SpriteSheetNumber], destinationRectangle, wallTile.SourceRectangle, damageColour);

                    Tile roofTile = roofTiles[column, row];
                    if (roofTile != null)
                        spriteBatch.Draw(spriteSheets[roofTile.SpriteSheetNumber], destinationRectangle, roofTile.SourceRectangle, damageColour);

                    Tile doorTile = doorTiles[column, row];
                    if (doorTile != null)
                        spriteBatch.Draw(spriteSheets[doorTile.SpriteSheetNumber], destinationRectangle, doorTile.SourceRectangle, Color.White);

                    destinationRectangle.Offset(scaledWidth, 0);                                    // Increment the destination rectangle's column
                }
            }
        }

        // Redraw tile at player's position if it is a roof tile
        public void DrawRoofTile(SpriteBatch spriteBatch, int column, int row, int columnOffset, int rowOffset)
        {
            Tile wallTile = wallTiles[column, row];
            if (wallTile != null && wallTile.Roof)
                spriteBatch.Draw(spriteSheets[wallTile.SpriteSheetNumber], new Rectangle(columnOffset * scaledWidth, rowOffset * scaledHeight, scaledWidth, scaledHeight), wallTile.SourceRectangle, Color.White);

            Tile roofTile = roofTiles[column, row];
            if (roofTile != null && roofTile.Roof)
                spriteBatch.Draw(spriteSheets[roofTile.SpriteSheetNumber], new Rectangle(columnOffset * scaledWidth, rowOffset * scaledHeight, scaledWidth, scaledHeight), roofTile.SourceRectangle, Color.White);
        }
    }
}