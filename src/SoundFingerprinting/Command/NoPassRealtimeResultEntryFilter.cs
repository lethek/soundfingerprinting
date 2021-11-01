namespace SoundFingerprinting.Command
{
    public class NoPassRealtimeResultEntryFilter<T> : IRealtimeResultEntryFilter<T>
    {
        public bool Pass(T entry, bool canContinueInTheNextQuery)
        {
            return false;
        }
    }
}