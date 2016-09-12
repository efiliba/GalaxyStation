namespace GalaxyStation.EventArgs
{
    public delegate void PlayersCollidedEventHandler(object sender, PlayersCollidedEventArgs e);

    public class PlayersCollidedEventArgs : System.EventArgs
    {
        public Player Player { get; set; }
        public Direction Direction { get; set; }

        public PlayersCollidedEventArgs(Player player, Direction direction)
        {
            Player = player;
            Direction = direction;
        }
    }
}
