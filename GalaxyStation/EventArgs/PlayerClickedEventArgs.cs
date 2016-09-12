namespace GalaxyStation.EventArgs
{
    public delegate void PlayerClickedEventHandler(object sender, PlayerClickedEventArgs e);

    public class PlayerClickedEventArgs : System.EventArgs
    {
        public PlayerClickedEventArgs()
        {
        }
    }
}
