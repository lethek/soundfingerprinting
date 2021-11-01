namespace SoundFingerprinting.Configuration
{
    using System;
    using System.Collections.Generic;
    using SoundFingerprinting.Command;
    using SoundFingerprinting.Data;
    using SoundFingerprinting.Query;
    using SoundFingerprinting.Strides;

    /// <summary>
    ///   Configuration options used when querying the data source in realtime.
    /// </summary>
    public class RealtimeQueryConfiguration
    {
        public RealtimeQueryConfiguration(int thresholdVotes,
            IRealtimeResultEntryFilter<ResultEntry> resultEntryFilter,
            Action<QueryResult> successCallback,
            Action<QueryResult> didNotPassFilterCallback,
            IRealtimeResultEntryFilter<ResultEntry> ongoingResultEntryFilter,
            Action<ResultEntry> ongoingSuccessCallback,
            Action<Exception, Hashes?> errorCallback,
            Action restoredAfterErrorCallback,
            IOfflineStorage offlineStorage,
            IStride stride,
            double permittedGap,
            double downtimeCapturePeriod,
            IDictionary<string, string> yesMetaFieldFilters,
            IDictionary<string, string> noMetaFieldsFilters, 
            IBackoffPolicy errorBackoffPolicy)
        {
            QueryConfiguration = new DefaultQueryConfiguration
            {
                ThresholdVotes = thresholdVotes,
                FingerprintConfiguration = new DefaultFingerprintConfiguration
                {
                    SpectrogramConfig = new DefaultSpectrogramConfig
                    {
                        Stride = stride
                    }
                },
                YesMetaFieldsFilters = yesMetaFieldFilters,
                NoMetaFieldsFilters = noMetaFieldsFilters,
                PermittedGap = permittedGap
            };
                
            ResultEntryFilter = resultEntryFilter;
            SuccessCallback = successCallback;
            DidNotPassFilterCallback = didNotPassFilterCallback;
            OngoingResultEntryFilter = ongoingResultEntryFilter;
            OngoingSuccessCallback = ongoingSuccessCallback;
            ErrorCallback = errorCallback;
            RestoredAfterErrorCallback = restoredAfterErrorCallback;
            OfflineStorage = offlineStorage;
            DowntimeCapturePeriod = downtimeCapturePeriod;
            ErrorBackoffPolicy = errorBackoffPolicy;
        }

        /// <summary>
        ///   Gets or sets vote count for a track to be considered a potential match (i.e. [1; 25]).
        /// </summary>
        public int ThresholdVotes
        {
            get => QueryConfiguration.ThresholdVotes;
            set => QueryConfiguration.ThresholdVotes = value;
        }

        /// <summary>
        ///  Gets or sets result entry filter.
        /// </summary>
        public IRealtimeResultEntryFilter<ResultEntry> ResultEntryFilter { get; set; }

        /// <summary>
        ///   Gets or sets success callback invoked when a candidate passes result entry filter.
        /// </summary>
        public Action<QueryResult> SuccessCallback { get; set; }
        
        /// <summary>
        ///  Gets or sets callback invoked when a candidate did not pass result entry filter, but has been considered a candidate.
        /// </summary>
        public Action<QueryResult> DidNotPassFilterCallback { get; set; }

        /// <summary>
        ///  Gets or sets ongoing result entry filter that will be invoked for every result entry filter that is captured by the aggregator.
        /// </summary>
        /// <remarks>
        ///  Experimental, may change in the future.
        /// </remarks>
        public IRealtimeResultEntryFilter<ResultEntry> OngoingResultEntryFilter { get; set; }
        
        /// <summary>
        ///  Gets or sets ongoing success callback that will be invoked once ongoing result entry filter is passed.
        /// </summary>
        /// <remarks>
        ///  Experimental, may change in the future.
        /// </remarks>
        public Action<ResultEntry> OngoingSuccessCallback { get; set; }

        /// <summary>
        ///  Gets or sets error callback which will be invoked in case if an error occurs during query time.
        /// </summary>
        /// <remarks>
        ///  Instance of the <see cref="Hashes"/> will be null in case if the error occured before hashes were generated, typically connection error to the realtime source. <br/>
        ///  If you need to stop querying immediately after an error occured, invoke token cancellation in the callback.
        /// </remarks>
        public Action<Exception, Hashes?> ErrorCallback { get; set; }

        /// <summary>
        ///  Gets or sets on error backoff policy that will be invoked before retrying to read from the realtime source.
        /// </summary>
        /// <remarks>
        ///  Default retry policy on streaming error is default <see cref="RandomExponentialBackoffPolicy"/>.
        /// </remarks>
        public IBackoffPolicy ErrorBackoffPolicy { get; set; }
        
        /// <summary>
        ///  Gets or sets error restore callback.
        /// </summary>
        public Action RestoredAfterErrorCallback { get; set; }

        /// <summary>
        ///  Gets or sets offline storage for hashes.
        /// </summary>
        /// <remarks>
        ///  Experimental, can be used to store hashes during the time the storage is not available. Will be consumed, after RestoredAfterErrorCallback invocation.
        /// </remarks>
        public IOfflineStorage OfflineStorage { get; set; }

        /// <summary>
        ///  Gets or sets downtime capture period (in seconds), value which will instruct the framework to cache realtime hashes for later processing in case if an unsuccessful request is made to Emy.
        /// </summary>
        /// <remarks>
        ///  Experimental.
        /// </remarks>
        public double DowntimeCapturePeriod { get; set; }

        /// <summary>
        ///  Gets or sets stride between 2 consecutive fingerprints used during querying.
        /// </summary>
        public IStride Stride
        {
            get => QueryConfiguration.Stride;
            set => QueryConfiguration.Stride = value;
        }

        /// <summary>
        ///  Gets or sets permitted gap between consecutive candidate so that they are glued together.
        /// </summary>
        public double PermittedGap
        {
            get => QueryConfiguration.PermittedGap;
            set => QueryConfiguration.PermittedGap = value;
        }

        /// <summary>
        ///  Gets or sets list of positive meta fields to consider when querying the data source for potential candidates.
        /// </summary>
        public IDictionary<string, string> YesMetaFieldsFilter
        {
            get => QueryConfiguration.YesMetaFieldsFilters;
            set => QueryConfiguration.YesMetaFieldsFilters = value;
        }

        /// <summary>
        ///  Gets or sets list of negative meta fields to consider when querying the data source for potential candidates.
        /// </summary>
        public IDictionary<string, string> NoMetaFieldsFilter
        {
            get => QueryConfiguration.NoMetaFieldsFilters;
            set => QueryConfiguration.NoMetaFieldsFilters = value;
        }

        internal QueryConfiguration QueryConfiguration { get; }
    }
}