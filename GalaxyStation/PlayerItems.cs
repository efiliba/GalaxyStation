using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Linq;

namespace GalaxyStation
{
    public class PlayerItems : Items
    {
        public struct DirectionalImage
        {
            public Texture2D Image { get; set; }
            public Rectangle[] SourceRectangles { get; set; }
            public Rectangle DestinationRectangle { get; set; }
        }

        private int heldColumn;
        private int heldRow;
        private Direction direction;
        private static System.Collections.Generic.Dictionary<string, DirectionalImage> directionalImages;

        public PlayerItems(System.Collections.Generic.List<Item> items, int totalColumns, int totalRows, int displayColumns, int displayRows, int tileWidth, int tileHeight) :
                base(items, totalColumns, totalRows, displayColumns, displayRows, tileWidth, tileHeight)
        {
            directionalImages = new System.Collections.Generic.Dictionary<string, DirectionalImage>();
        }

        public void Load(ContentManager contentManager)
        {
            foreach (Map.TileProperty property in items.Select(x => x.Property))
            {
                int width = scaledWidth * property.HorizontalTiles;
                int height = scaledHeight * property.VerticalTiles;
                Rectangle[] sourceRectangles = new Rectangle[4];
                foreach (Direction direction in System.Enum.GetValues(typeof(Direction)))
                    sourceRectangles[(int)direction] = new Rectangle(width * (int)direction, 0, width, height);

                directionalImages[property.Name] = new DirectionalImage
                {
                    Image = contentManager.Load<Texture2D>("Items\\" + property.Name),
                    SourceRectangles = sourceRectangles,
                    DestinationRectangle = new Rectangle(heldColumn * scaledWidth + scaledWidth * property.HorizontalOffset, heldRow * scaledHeight + scaledHeight * property.VerticalOffset, width, height)
                };
            }
        }

        public int HeldColumn
        {
            get { return heldColumn; }
            set
            {
                heldColumn = value;
            }
        }
        public int HeldRow
        {
            get { return heldRow; }
            set
            {
                heldRow = value;
            }
        }
        public Direction Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        public void Draw(SpriteBatch spriteBatch, int columnOffset, int rowOffset)
        {
            foreach (Item item in items)
                if (item.Held)
                {
                    DirectionalImage directionalImage = directionalImages[item.Property.Name];
                    spriteBatch.Draw(directionalImage.Image, directionalImage.DestinationRectangle, directionalImage.SourceRectangles[(int)Direction], Color.White);
                }
        }
    }
}
