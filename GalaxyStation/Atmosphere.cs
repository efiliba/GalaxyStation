namespace GalaxyStation
{
    public class Atmosphere
    {
        private float[] gas;
        private System.DateTime startTime;
        private System.DateTime stopTime;
        private int column;
        private int row;
        private bool stopped;

        public Atmosphere(int pressure)
        {
            int maxDistance = 11;
            gas = new float[maxDistance];
            for (int distance = 0; distance < maxDistance; distance++)
                gas[distance] = pressure / (float)System.Math.Pow(distance * 2 + 1, 2);

            stopped = false;
        }

        public void Start(int column, int row)
        {
            startTime = System.DateTime.Now;

            this.column = column;
            this.row = row;
            stopped = false;
        }

        public void Stop()
        {
            stopTime = System.DateTime.Now;
            stopped = true;
        }

        public float Effect(int playerColumn, int playerRow, int obstacleDistance, bool compound = false)
        {
            int time = (int)System.DateTime.Now.Subtract(startTime).TotalSeconds;
            int distance = obstacleDistance + (int)System.Math.Max(System.Math.Abs(column - playerColumn), System.Math.Abs(row - playerRow));

            float totalEffect = 0;
            if (distance <= time)
            {
                int effectStrength = time < gas.Length ? time : gas.Length - 1;
                totalEffect = gas[effectStrength];

                if (compound)
                {
                    int effectTime = stopped ? (int)(stopTime - startTime).TotalSeconds : time;

                    int startEffectTime = time > effectTime ? time - effectTime + 1 : distance;
                    if (distance > startEffectTime)
                        startEffectTime = distance;


                    if (time > gas.Length)
                    {
                        int delta = System.Math.Min(time - gas.Length + 1, effectTime);
                        totalEffect *= delta;
                        time -= delta;
                    }

                    while (startEffectTime < time)
                        totalEffect += gas[startEffectTime++];
                }
            }

            return totalEffect;
        }
    }
}
