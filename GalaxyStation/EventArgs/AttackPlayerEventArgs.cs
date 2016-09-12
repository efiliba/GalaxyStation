namespace GalaxyStation.EventArgs
{
    public delegate void AttackPlayerEventHandler(object sender, AttackPlayerEventArgs e);
 
    public class AttackPlayerEventArgs : System.EventArgs
    {
        public AttackPlayerEventArgs()
        {
        }
    }
}
