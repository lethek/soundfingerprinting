namespace SoundFingerprinting.Command
{
    using SoundFingerprinting.Query;

    /// <summary>
    ///  Contract for realtime result entry filter.
    /// </summary>
    public interface IRealtimeResultEntryFilter<T>
    {
        /// <summary>
        ///  Checks if provided result entry passes the filter.
        /// </summary>
        /// <param name="entry">Instance of <see cref="ResultEntry"/> to check.</param>
        /// <param name="canContinueInTheNextQuery">Flag indicating whether the result entry can continue in the next query.</param>
        /// <returns>True if the result entry passes the filter, otherwise false.</returns>
        bool Pass(T entry, bool canContinueInTheNextQuery);
    }
}