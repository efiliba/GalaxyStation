namespace GalaxyStation
{
    public partial class Map
    {
        private struct MapData
        {
            public int Height { get; set; }
            public Layer[] Layers { get; set; }
            public string Orientation { get; set; }
            public Properties Properties { get; set; }
            public int TileHeight { get; set; }
            public TileSet[] TileSets { get; set; }
            public int TileWidth { get; set; }
            public int Version { get; set; }
            public int Width { get; set; }
        }

        private struct Layer
        {
            public int[] Data { get; set; }
            public int Height { get; set; }
            public string Name { get; set; }
            public Object[] Objects { get; set; }
            public float Opacity { get; set; }
            public string Type { get; set; }
            public bool Visible { get; set; }
            public int Width { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
        }

        private struct Properties
        {
            public bool Roof { get; set; }
        }

        private struct Object
        {
            public int Gid { get; set; }
            public int Height { get; set; }
            public string Name { get; set; }
            public Properties Properties { get; set; }
            public string Type { get; set; }
            public int Width { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
        }

        private struct TileSet
        {
            public int FirstGid { get; set; }
            public string Image { get; set; }
            public int ImageHeight { get; set; }
            public int ImageWidth { get; set; }
            public int Margin { get; set; }
            public string Name { get; set; }
            public Properties Properties { get; set; }
            public int Spacing { get; set; }
            public int TileHeight { get; set; }
            public Newtonsoft.Json.Linq.JObject TileProperties { get; set; }
            public int TileWidth { get; set; }
        }

        public struct TileProperty
        {
            public string Name { get; set; }
            public int HorizontalTiles { get; set; }
            public int VerticalTiles { get; set; }
            public int HorizontalOffset { get; set; }
            public int VerticalOffset { get; set; }
        }
    }
}