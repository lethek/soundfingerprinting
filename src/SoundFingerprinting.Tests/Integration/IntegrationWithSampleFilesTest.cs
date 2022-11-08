namespace SoundFingerprinting.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    using Audio;
    using Data;

    using NUnit.Framework;

    public abstract class IntegrationWithSampleFilesTest : AbstractTest
    {
        private readonly string pathToSamples = Path.Combine(TestContext.CurrentContext.TestDirectory, "chopinsamples.json");
        
        protected readonly string PathToWav = Path.Combine(TestContext.CurrentContext.TestDirectory, "chopin_short.wav");

        protected void AssertHashDatasAreTheSame(Hashes h1, Hashes h2)
        {
            var firstHashes = h1.ToList();
            var secondHashes = h2.ToList();
            Assert.AreEqual(h1.DurationInSeconds, h2.DurationInSeconds);
            Assert.AreEqual(firstHashes.Count, secondHashes.Count);
         
            // hashes are not ordered as parallel computation is involved
            firstHashes = SortHashesBySequenceNumber(firstHashes);
            secondHashes = SortHashesBySequenceNumber(secondHashes);

            for (int i = 0; i < firstHashes.Count; i++)
            {
                Assert.AreEqual(firstHashes[i].SequenceNumber, secondHashes[i].SequenceNumber);
                Assert.AreEqual(firstHashes[i].StartsAt, secondHashes[i].StartsAt, 0.0001);
                CollectionAssert.AreEqual(firstHashes[i].HashBins, secondHashes[i].HashBins);
            }
        }

        protected TagInfo GetTagInfo()
        {
            return new TagInfo
            {
                Album = "Album",
                AlbumArtist = "AlbumArtist",
                Artist = "Chopin",
                Composer = "Composer",
                Duration = 10.0d,
                Genre = "Genre",
                IsEmpty = false,
                ISRC = "ISRC",
                Title = "Nocture",
                Year = 1857
            };
        }

        protected AudioSamples GetAudioSamples()
        {
            lock (this) {
                using var stream = new FileStream(pathToSamples, FileMode.Open, FileAccess.Read);
                return JsonSerializer.Deserialize<AudioSamples>(stream, SerializerOptions);
            }
        }

        private static List<HashedFingerprint> SortHashesBySequenceNumber(IEnumerable<HashedFingerprint> hashDatasFromFile)
        {
            return hashDatasFromFile.OrderBy(hashData => hashData.SequenceNumber).ToList();
        }

        private static readonly JsonSerializerOptions SerializerOptions = new() {
            Converters = { new FloatMemoryJsonConverter() }
        };

        private class FloatMemoryJsonConverter : JsonConverter<ReadOnlyMemory<float>>
        {
            public override ReadOnlyMemory<float> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
                => new(JsonSerializer.Deserialize<float[]>(ref reader, options));

            public override void Write(Utf8JsonWriter writer, ReadOnlyMemory<float> value, JsonSerializerOptions options)
                => JsonSerializer.Serialize(writer, value.ToArray(), options);
        }
    }
}
