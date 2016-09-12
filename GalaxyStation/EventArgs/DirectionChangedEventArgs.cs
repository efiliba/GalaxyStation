namespace GalaxyStation.EventArgs
{
    public delegate void DirectionChangedEventHandler(object sender, DirectionChangedEventArgs e);

    public class DirectionChangedEventArgs
    {
        public Direction Direction { get; set; }
        public DirectionChangedEventArgs(Direction direction)
        {
            Direction = direction;
        }
    }
}