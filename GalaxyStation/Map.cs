using System;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace GalaxyStation
{
    partial class Map
    {
        private MapData map;

        public SpriteSheet[] SpriteSheets { get; private set; }
        public Tile[,] FloorTiles { get; private set; }
        public Tile[,] WallTiles { get; private set; }
        public Tile[,] RoofTiles { get; private set; }
        public Tile[,] DoorTiles { get; private set; }
        public System.Collections.Generic.List<Item> ItemTiles { get; private set; }

        private RoofTileLookup roofTileLookup;
        private WallTileLookup wallTileLookup;
        private BuildRoofTileLookup buildRoofTileLookup;
        private ModifyWallTileLookup modifyWallTileLookup;
        private System.Collections.Generic.IEnumerable<int> wallTops = new int[] { 114, 115, 119, 120, 121, 123 };   // Roofs with walls under them - roofs not considered solid
//        private System.Collections.Generic.IEnumerable<int> wallTops = new int[] { 112, 114, 115, 116, 119, 120 };   // Roofs with walls under them

        public Map(string filePath, int tileWidth, int tileHeight)
        {
            System.IO.StreamReader reader = new System.IO.StreamReader(filePath);
            map = Newtonsoft.Json.JsonConvert.DeserializeObject<MapData>(reader.ReadToEnd());

            // Set the sprite sheets' names and indices
            SpriteSheets = new SpriteSheet[map.TileSets.Length];
            for (int index = 0; index < map.TileSets.Length; index++)
            {
                TileSet tileSet = map.TileSets[index];
                SpriteSheets[index] = new SpriteSheet(tileSet.ImageWidth, tileSet.ImageHeight, tileSet.TileWidth, tileSet.TileHeight, tileWidth, tileHeight)
                {
                    Path = tileSet.Image,
                    Index = index,
                    FirstGid = tileSet.FirstGid
                };
            }

            // Setup the floor tiles
            FloorTiles = new Tile[map.Width, map.Height];
            for (int row = 0; row < map.Height; row++)
                for (int column = 0; column < map.Width; column++)
                {
                    int tileID = map.Layers[0].Data[row * map.Width + column];
                    if (tileID > 0)
                    {
                        SpriteSheet spriteSheet = GetSpriteSheet(tileID);
                        FloorTiles[column, row] = new Tile
                        {
                            SpriteSheetNumber = spriteSheet.Index,
                            SourceRectangle = spriteSheet.SourceRectangles[tileID],
                            Solid = false
                        };
                    }
                }

            // Setup wall tiles, from the second layer
            WallTiles = new Tile[map.Height + 1, map.Width];                                        // Add buffer row for when redrawing roof tiles
            foreach (Object layerObject in map.Layers[1].Objects)
            {
                int column = layerObject.X / tileWidth;
                int row = layerObject.Y / tileHeight - 1;
                SpriteSheet spriteSheet = GetSpriteSheet(layerObject.Gid);

                WallTiles[column, row] = new Tile
                {
                    Gid = layerObject.Gid,
                    SpriteSheetNumber = spriteSheet.Index,
                    SourceRectangle = spriteSheet.SourceRectangleByGid(layerObject.Gid),
                    Solid = true,
                    Roof = layerObject.Properties.Roof
                };
            }

            RoofTiles = new Tile[map.Height + 1, map.Width];                                        // Add buffer row for when redrawing roof tiles
            foreach (Object layerObject in map.Layers[2].Objects)
            {
                int column = layerObject.X / tileWidth;
                int row = layerObject.Y / tileHeight - 1;
                SpriteSheet spriteSheet = GetSpriteSheet(layerObject.Gid);

                RoofTiles[column, row] = new Tile
                {
                    Gid = layerObject.Gid,
                    SpriteSheetNumber = spriteSheet.Index,
                    SourceRectangle = spriteSheet.SourceRectangleByGid(layerObject.Gid),
                    Solid = !wallTops.Contains(layerObject.Gid),                                    // Roofs with walls under them - roof only i.e. non blocking                                                                  // Roofs are not considered solid
                    Roof = true
                };
            }

            // Setup door tiles
            DoorTiles = new Tile[map.Height + 1, map.Width];                                        // Add buffer row for when redrawing roof tiles
            foreach (Object layerObject in map.Layers[3].Objects)
            {
                int column = layerObject.X / tileWidth;
                int row = layerObject.Y / tileHeight - 1;
                SpriteSheet spriteSheet = GetSpriteSheet(layerObject.Gid);

                //DoorTiles[column, row] = new Tile
                //{
                //    Gid = layerObject.Gid,
                //    SpriteSheetNumber = spriteSheet.Index,
                //    SourceRectangle = spriteSheet.SourceRectangleByGid(layerObject.Gid),
                //    Solid = false,

                //};
            }

            // Add the item tiles, i.e. after the second layer
            TileProperty[] tileProperties = GetItemsProperties(map.TileSets[3].TileProperties);     // Get custom properties for each item in 'images' TileSet
            ItemTiles = new System.Collections.Generic.List<Item>();
            for (int layer = 3; layer < map.Layers.Length; layer++)
                foreach (Object layerObject in map.Layers[layer].Objects)
                {
                    SpriteSheet spriteSheet = GetSpriteSheet(layerObject.Gid);
                    ItemTiles.Add(new Item
                    {
                        Property = tileProperties[layerObject.Gid - spriteSheet.FirstGid],
                        Column = layerObject.X / tileWidth,
                        Row = layerObject.Y / tileHeight - 1,
                        SpriteSheetNumber = spriteSheet.Index,
                        SourceRectangle = new Microsoft.Xna.Framework.Rectangle(((layerObject.Gid - spriteSheet.FirstGid) % spriteSheet.Columns) * tileWidth,
                                ((layerObject.Gid - spriteSheet.FirstGid) / spriteSheet.Columns) * tileHeight, tileWidth, tileHeight)
                    });
                }

            wallTileLookup = new WallTileLookup(map.TileSets[1].FirstGid);                          // Modify wall tiles when adjacent tiles destroyed or built based on SideType
            roofTileLookup = new RoofTileLookup(map.TileSets[2].FirstGid);
            buildRoofTileLookup = new BuildRoofTileLookup(map.TileSets[2].FirstGid);
            modifyWallTileLookup = new ModifyWallTileLookup(map.TileSets[1].FirstGid);
        }

        private TileProperty[] GetItemsProperties(Newtonsoft.Json.Linq.JObject properties)
        {
            var tileProperties = new TileProperty[properties.Count];
            for (int index = 0; index < tileProperties.Length; index++)
            {
//                tileProperties[index] = ((Newtonsoft.Json.Linq.JToken)properties[index.ToString()]).ToObject<TileProperty>();
                var property = (Newtonsoft.Json.Linq.JToken)properties[index.ToString()];
                tileProperties[index] = new TileProperty
                {
                    Name = property["Name"].ToString(),
                    HorizontalTiles = GetIntProperty(property["HorizontalTiles"], 1),
                    VerticalTiles = GetIntProperty(property["VerticalTiles"], 1),
                    HorizontalOffset = GetIntProperty(property["HorizontalOffset"]),
                    VerticalOffset = GetIntProperty(property["VerticalOffset"])
                };
            }

            return tileProperties;
        }

        private int GetIntProperty(Newtonsoft.Json.Linq.JToken property, int defaultValue = 0)
        {
            if (property != null)
                int.TryParse(property.ToString(), out defaultValue);

            return defaultValue;
        }

        private SpriteSheet GetSpriteSheet(int gid)
        {
            int index = 1;
            while (index < SpriteSheets.Length && gid >= SpriteSheets[index].FirstGid)
                index++;

            return SpriteSheets[index - 1];
        }

        public int Columns
        {
            get { return map.Width; }
        }

        public int Rows
        {
            get { return map.Height; }
        }

        public Tile[,] GetBackgroundTiles(int columns, int rows, int tileWidth, int tileHeight)
        {
            Tile[,] backgroundTiles = new Tile[columns, rows];  // check column / row order

            // Create the back ground tiles
            System.Random random = new System.Random(System.DateTime.Now.Millisecond);
            for (int row = 0; row < rows; row++)
                for (int column = 0; column < columns; column++)
                {
                    int spriteIndex = 56 + random.Next(26); // 467 - no breaks                      // [457, 482]
                    backgroundTiles[column, row] = new Tile
                    {
                        SpriteSheetNumber = 4,
                        SourceRectangle = new Microsoft.Xna.Framework.Rectangle((spriteIndex % 10) * tileWidth, (spriteIndex / 10) * tileHeight, tileWidth, tileHeight),
                        Solid = false
                    };
                }

            return backgroundTiles;
        }

        public void ToggleDoor(int column, int row)
        {
            if (DoorTiles[column, row] != null)
                DoorTiles[column, row].Solid = !DoorTiles[column, row].Solid;
        }

        public void BuildTile(int column, int row, KeyboardState keyboardState, Game1.BuildCycle buildCycle)    // Add wall with associated roof and modify neighbours
        {
            if (keyboardState.IsKeyDown(Keys.LeftControl))
            {
                BuildWallTile(column, row);
                BuildRoofTile(column, row);
            }
            else
                BuildFloorTile(column, row, buildCycle);
        }

        private void BuildFloorTile(int column, int row, Game1.BuildCycle buildCycle)
        {
            SpriteSheet floorSpriteSheet = SpriteSheets[0];
            FloorTiles[column, row] = new Tile
            {
                Gid = (int)buildCycle,
                SpriteSheetNumber = 0,
                SourceRectangle = floorSpriteSheet.SourceRectangles[(int)buildCycle],
                Solid = false
            };
        }

        private void BuildWallTile(int column, int row)
        {
            SpriteSheet wallSpriteSheet = SpriteSheets[1];
            int bitFlag = ModifyAdjacentWalls(column, row + 1);                                     // Get bit flag of horizontally adjacent tiles
            WallTiles[column, row + 1] = new Tile                                                   // Add wall tile bellow the roof tile
            {
                Gid = wallSpriteSheet.FirstGid + bitFlag,
                SpriteSheetNumber = wallSpriteSheet.Index,
                SourceRectangle = wallSpriteSheet.SourceRectangles[bitFlag],
                Solid = true,                                                                       // Roofs are not considered solid
                Roof = false
            };
        }

        private void BuildRoofTile(int column, int row)
        {
            SpriteSheet roofSpriteSheet = SpriteSheets[2];
            int tileIndex = ModifyAdjacentRoofs(column, row) + roofSpriteSheet.FirstGid;            // Add sprite sheet offset index
            int newGid = buildRoofTileLookup[ExamineType.Current, tileIndex];

            RoofTiles[column, row] = new Tile
            {
                Gid = newGid,                                                                       // Added roof tile based on existing neighbours
                SpriteSheetNumber = roofSpriteSheet.Index,
                SourceRectangle = roofSpriteSheet.SourceRectangleByGid(newGid),
                Solid = false,                                                                      // Roofs are not considered solid
                Roof = true
            };
        }

        private int ModifyAdjacentWalls(int column, int row)                                        // Get bit flag of horizontally adjacent tiles
        {
            int bitFlag = 0;                                                                        // Bit flag of existing horizontal neighbours

            if (RoofTiles[column - 1, row] != null)                                                 // Is left side a roof?
                bitFlag += 1;
            else if (WallTiles[column - 1, row] != null)                                            // Is left side a wall?
            {
                bitFlag += 2;
                ModifyWallTile(WallTiles[column - 1, row], ExamineType.Right);
            }

            if (RoofTiles[column + 1, row] != null)                                                 // Is right side a roof?
                bitFlag += 3;
            else if (WallTiles[column + 1, row] != null)                                            // Is right side a wall?
            {
                bitFlag += 6;
                ModifyWallTile(WallTiles[column + 1, row], ExamineType.Left);
            }
        
            return bitFlag;
        }

        private void ModifyWallTile(Tile tile, ExamineType examineType)
        {
            tile.Gid = modifyWallTileLookup[examineType, tile.Gid];
            tile.SourceRectangle = SpriteSheets[1].SourceRectangleByGid(tile.Gid);                  // SpriteSheets[1] contains Wall tiles
        }

        private int ModifyAdjacentRoofs(int column, int row)
        {
            int bitFlag = 0;                                                                        // Bit flag of existing neighbours

            if (RoofTiles[column, row - 1] != null)                                                 // Top
            {
                bitFlag += 1;
                BuildModifyRoofTile(RoofTiles[column, row - 1], ExamineType.Bottom);
            }
            if (RoofTiles[column - 1, row] != null)                                                 // Left
            {
                bitFlag += 2;
                BuildModifyRoofTile(RoofTiles[column - 1, row], ExamineType.Right);
            }
            if (RoofTiles[column, row + 1] != null)                                                 // Bottom
            {
                bitFlag += 4;
                BuildModifyRoofTile(RoofTiles[column, row + 1], ExamineType.Top);
            }
            if (RoofTiles[column + 1, row] != null)                                                 // Right
            {
                bitFlag += 8;
                BuildModifyRoofTile(RoofTiles[column + 1, row], ExamineType.Left);
            }

            return bitFlag;
        }

        private void BuildModifyRoofTile(Tile roofTile, ExamineType examineType)
        {
            roofTile.Gid = buildRoofTileLookup[examineType, roofTile.Gid];
            roofTile.SourceRectangle = SpriteSheets[2].SourceRectangleByGid(roofTile.Gid);          // SpriteSheets[2] contains Roof tiles
        }

        public void DestroyTile(int column, int row, KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.LeftControl))
            {
                if (WallTiles[column, row] != null)                                                 // Remove wall and associated roof (above wall)
                {
                    ModifyWallTiles(column, row);
                    ModifyRoofTiles(column, row - 1);                                               // Modify roof above wall
                }
                else if (RoofTiles[column, row] != null && RoofTiles[column, row - 1] != null)      // Check for virtual wall i.e. roof has another roof above it
                {
                    ModifyRoofTiles(column, row - 1);
                    RoofTiles[column, row].Solid = false;
                }
            }
            else
                FloorTiles[column, row] = null;
        }

        private void ModifyWallTiles(int column, int row)                                           // Remove current wall and modify adjacent walls
        { 
            WallTiles[column, row] = null;

            ModifyWallTile(column + 1, row, SideType.LeftClosed);
            ModifyWallTile(column - 1, row, SideType.RightClosed);
        }

        private void ModifyWallTile(int column, int row, SideType sideType)
        {
            var wallTile = WallTiles[column, row];
            if (wallTile != null)
            {
                wallTile.Gid = wallTileLookup[sideType, wallTile.Gid];
                wallTile.SourceRectangle = SpriteSheets[1].SourceRectangleByGid(wallTile.Gid);      // SpriteSheets[1] contains Wall tiles
            }
        }

        private void ModifyRoofTiles(int column, int row)                                           // Remove current roof and modify surrounding roofs
        {
            RoofTiles[column, row] = null;

            ModifyRoofTile(column + 1, row, SideType.LeftClosed, true);
            ModifyRoofTile(column - 1, row, SideType.RightClosed, true);
            ModifyRoofTile(column, row + 1, SideType.TopClosed, false);
            ModifyRoofTile(column, row - 1, SideType.BottomClosed, true);

            // There should be a wall underneath the removed roof (if there is another roof above it), if not it had a virtual wall and so the wall must be re-added  
            if (RoofTiles[column, row - 1] != null)
                BuildWallTile(column, row - 1);
        }

        private void ModifyRoofTile(int column, int row, SideType sideType, bool solid)
        {
            var roofTile = RoofTiles[column, row];
            if (roofTile != null)
            {
                roofTile.Gid = roofTileLookup[sideType, roofTile.Gid];
                roofTile.SourceRectangle = SpriteSheets[2].SourceRectangleByGid(roofTile.Gid);      // SpriteSheets[2] contains Roof tiles
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////// 
        public void DestroyTile2(int column, int row, Direction direction)
        {
            switch (direction)
            {
                case Direction.Up: row--; break;
                case Direction.Down: row++; break;
                case Direction.Left: column--; break;
                case Direction.Right: column++; break;
            }

            if (WallTiles[column, row] != null)
            {
                ModifyWallTiles(column, row);
                ModifyRoofTiles(column, row - 1);
            }

            if (RoofTiles[column, row] != null)
            {
                ModifyRoofTiles(column, row);

                if ((direction == Direction.Left || direction == Direction.Right) && RoofTiles[column, row - 1] != null)
                {
                    WallTiles[column, row] = new Tile
                    {
                        SpriteSheetNumber = 1,
                        SourceRectangle = SpriteSheets[1].SourceRectangles[6],
                        Solid = true
                    };
                }
            }
        }
    }
}