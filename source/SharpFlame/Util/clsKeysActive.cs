namespace SharpFlame.Util
{
    public class clsKeysActive
    {
        public bool[] Keys = new bool[256];

        public void Deactivate()
        {
            for( int i = 0; i <= 255; i++ )
            {
                Keys[i] = false;
            }
        }
    }
}