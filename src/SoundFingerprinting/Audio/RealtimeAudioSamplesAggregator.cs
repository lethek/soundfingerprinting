namespace SoundFingerprinting.Audio
{
    using System;
    using SoundFingerprinting.Configuration;
    using SoundFingerprinting.Strides;

    /// <summary>
    ///  Realtime samples aggregator.
    /// </summary>
    public class RealtimeAudioSamplesAggregator : IRealtimeAudioSamplesAggregator
    {
        private readonly int minSamplesPerFingerprint;
        private Memory<float> tail;

        /// <summary>
        ///  Initializes a new instance of the <see cref="RealtimeAudioSamplesAggregator"/> class.
        /// </summary>
        /// <param name="minSamplesPerFingerprint">Minimum number of sampler per one fingerprint (see <see cref="SpectrogramConfig"/>).</param>
        /// <param name="stride">Stride to use between consecutive fingerprints.</param>
        public RealtimeAudioSamplesAggregator(int minSamplesPerFingerprint, IStride stride)
        {
            this.minSamplesPerFingerprint = minSamplesPerFingerprint;
            Stride = stride;
            tail = Memory<float>.Empty;
        }

        private IStride Stride { get; }
        
        /// <inheritdoc cref="IRealtimeAudioSamplesAggregator.Aggregate"/>
        public AudioSamples? Aggregate(AudioSamples chunk)
        {
            var withPreviousTail = AttachNewChunk(chunk);
            CacheTail(withPreviousTail);
            return withPreviousTail.Samples.Length >= minSamplesPerFingerprint ? withPreviousTail : null;
        }

        private AudioSamples AttachNewChunk(AudioSamples chunk)
        {
            var prefixed = new Memory<float>(new float[tail.Length + chunk.Samples.Length]);
            tail.CopyTo(prefixed.Slice(0, tail.Length));
            chunk.Samples.CopyTo(prefixed.Slice(tail.Length, chunk.Samples.Length));

            var samples = new AudioSamples(prefixed, chunk.Origin, chunk.SampleRate, chunk.RelativeTo.AddSeconds(-(float)tail.Length / chunk.SampleRate), -(double)tail.Length / chunk.SampleRate);
            tail = prefixed;
            return samples;
        }

        private void CacheTail(AudioSamples samples)
        {
            if (samples.Samples.Length >= minSamplesPerFingerprint)
            {
                int nextStride = Stride.NextStride;
                if (nextStride < minSamplesPerFingerprint)
                {
                    // this value is exact when we use IncrementalStaticStride which can tell exactly how much tail length we ignore because it is not evenly divisible by length
                    // Example:
                    // hash   |       |   |   |    = 3 hashes with
                    // q      ------------------
                    // stride |   |   |   |   |
                    // cache               -----
                    int estimatedIgnoredWindow = (samples.Samples.Length - minSamplesPerFingerprint) % nextStride;
                    
                    // tail size is always shorter than minSamplesPerFingerprint
                    int tailSize = minSamplesPerFingerprint - nextStride + estimatedIgnoredWindow;
                    tail = new Memory<float>(new float[tailSize]);
                    samples.Samples.Slice(samples.Samples.Length - tailSize, tailSize).CopyTo(tail);
                }
                else
                {
                    tail = Memory<float>.Empty;
                }
            }
        }
    }
}