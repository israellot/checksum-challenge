using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Globalization;
using Xunit;

namespace ChecksumChallenge;

public class Program
{
    [Config(typeof(AntiVirusFriendlyConfig))]
    public class ChecksumBenchmark
    {
        private byte[]? SourceBytes { get; set; }

        private ReadOnlySpan<byte> Span => SourceBytes.AsSpan();

        [Params(1_000_000, 100_000_000)]
        public int Length { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            SourceBytes = new byte[Length];
            Random.Shared.NextBytes(SourceBytes);
        }

        [Benchmark(Baseline = true)]
        public uint Junior()
        {
            return Checksum.ChecksumJunior(Span);
        }

        [Benchmark]
        public uint Pro()
        {
            return Checksum.ChecksumPro(Span);
        }

        [Benchmark]
        public uint Senior()
        {
            return Checksum.ChecksumSenior(Span);
        }

        [Benchmark]
        public uint Hacker()
        {
            return Checksum.ChecksumHacker(Span);
        }

        [Benchmark]
        public uint Expert()
        {
            return Checksum.ChecksumExpert(Span);
        }

        [Benchmark]
        public uint ExpertAvx()
        {
            return Checksum.ChecksumExpertAvx(Span);
        }

        [Benchmark]
        public uint ExpertAvx2()
        {
            return Checksum.ChecksumExpertAvx2(Span);
        }

        [Benchmark]
        public uint Marcelo()
        {
            return ChecksumMarcelo.Get(Span);
        }
    }

    public static void Assertions()
    {
        Span<byte> b = stackalloc byte[2048];
        Random.Shared.NextBytes(b);

        foreach (var i in Enumerable.Range(0, 1024))
        {
            var span = b.Slice(0, 1024 + i);
            
            var crc = Checksum.ChecksumJunior(span);
            
            Assert.Equal(crc, Checksum.ChecksumPro(span));
            Assert.Equal(crc, Checksum.ChecksumSenior(span));
            Assert.Equal(crc, Checksum.ChecksumHacker(span));
            Assert.Equal(crc, Checksum.ChecksumExpert(span));
            Assert.Equal(crc, Checksum.ChecksumExpertAvx(span));
            Assert.Equal(crc, Checksum.ChecksumExpertAvx2(span));
            Assert.Equal(crc, ChecksumMarcelo.Get(span));
        }
    }

    public static void Main(string[] args)
    {
        Assertions(); //make sure all versions are correct
        
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

        BenchmarkRunner.Run<ChecksumBenchmark>();
    }
}