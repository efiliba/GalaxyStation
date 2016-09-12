using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GalaxyStation
{
    public class MapItems : Items
    {
        public MapItems(System.Collections.Generic.List<Item> items, int totalColumns, int totalRows, int displayColumns, int displayRows, int tileWidth, int tileHeight) :
                base(items, totalColumns, totalRows, displayColumns, displayRows, tileWidth, tileHeight)
        {
        }

        public void Draw(SpriteBatch spriteBatch, int columnOffset, int rowOffset)
        {
            foreach (Item item in items)
                if (!item.Held)
                {
                    destinationRectangle.X = (item.Column - columnOffset) * scaledWidth;
                    destinationRectangle.Y = (item.Row - rowOffset) * scaledHeight;

                    spriteBatch.Draw(spriteSheets[item.SpriteSheetNumber], destinationRectangle, item.SourceRectangle, Color.White);
                }
        }
    }
}
