using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GalaxyStation
{
    public class Item
    {
        public Map.TileProperty Property { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public int SpriteSheetNumber { get; set; }                                                  // Index of the sprite sheet to use
        public Rectangle SourceRectangle { get; set; }                                              // Source rectangle from sprite sheet
        public bool Solid { get; set; }
        public bool Held { get; set; }
    }

    public class Items : Scalable
    {
        protected System.Collections.Generic.List<Item> items;

        protected Rectangle destinationRectangle;

        public Items(System.Collections.Generic.List<Item> items, int totalColumns, int totalRows, int displayColumns, int displayRows, int tileWidth, int tileHeight) :
                base(totalColumns, totalRows, displayColumns, displayRows, tileWidth, tileHeight)
        {
            this.items = items;
            destinationRectangle = new Rectangle
            {
                Width = scaledWidth,
                Height = scaledHeight
            };
        }

        public Item this[int index]
        {
            get { return items[index]; }
        }

        public bool ItemAt(int column, int row)
        {
            return false;
        }
    }
}

