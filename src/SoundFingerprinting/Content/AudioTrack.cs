﻿namespace SoundFingerprinting.Content
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NetFabric.Hyperlinq;
    using SoundFingerprinting.Audio;
    using static System.Math;

    /// <summary>
    ///  Class container for audio samples.
    /// </summary>
    public class AudioTrack
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioTrack"/> class.
        /// </summary>
        /// <param name="samples">Audio samples.</param>
        public AudioTrack(AudioSamples samples)
        {
            Samples = samples;
        }

        /// <summary>
        ///  Gets audio samples.
        /// </summary>
        public AudioSamples Samples { get; }

        /// <summary>
        ///  Gets audio samples duration.
        /// </summary>
        public double Duration => Samples.Duration;
        
        /// <summary>
        ///  Subtracts a part of the track according to start and length parameters.
        /// </summary>
        /// <param name="start">Start measured in seconds.</param>
        /// <param name="length">Length measured in seconds.</param>
        /// <returns>A new instance of the <see cref="AudioTrack"/> class.</returns>
        public AudioTrack SubTrack(double start, double length)
        {
            start = Max(start, 0);
            length = Min(length, Duration - start);
            var samples = Samples.Samples
                .Slice((int)(start * Samples.SampleRate), (int)(length * Samples.SampleRate));
            return new AudioTrack(new AudioSamples(samples, Samples.Origin, Samples.SampleRate, Samples.RelativeTo.AddSeconds(start)));
        }

        /// <summary>
        ///  Cuts the audio track from the beginning.
        /// </summary>
        /// <param name="length">Length measured in seconds to cut.</param>
        /// <returns>A new instance of the <see cref="AudioTrack"/> class.</returns>
        public AudioTrack Head(double length)
        {
            return SubTrack(0, length);
        }

        /// <summary>
        ///  Cuts the audio track from the end.
        /// </summary>
        /// <param name="start">Start measured in seconds to start at.</param>
        /// <returns>A new instance of the <see cref="AudioTrack"/> class.</returns>
        public AudioTrack Tail(double start)
        {
            return SubTrack(start, Duration - start);
        }

        /// <summary>
        ///  Concatenates multiple audio tracks in one.
        /// </summary>
        /// <param name="tracks">Tracks to concatenate.</param>
        /// <returns>A new instance of the <see cref="AudioTrack"/> class.</returns>
        /// <exception cref="ArgumentException">Argument exception if the collection is empty.</exception>
        public static AudioTrack Concat(IReadOnlyCollection<AudioTrack> tracks)
        {
            if (!tracks.Any())
            {
                throw new ArgumentException("Cannot concat empty set.", nameof(tracks));
            }

            var audioTrack = tracks.First();
            var first = audioTrack.Samples;
            var samples = tracks
                .SelectMany(t => t.Samples.Samples.AsValueEnumerable())
                .ToArray();
            var audioSamples = new AudioSamples(samples, first.Origin, first.SampleRate, first.RelativeTo);
            return new AudioTrack(audioSamples);
        }
    }
}
