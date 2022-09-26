using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Globalization;
using Xunit;

namespace ChecksumChallenge;

public class Program
{         
    public class ChecksumBenchmark
    {
        public byte[]? SourceBytes { get; set; }
                
        [Params(1_000_000)]
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
            return Checksum.ChecksumJunior(SourceBytes);
        }                

        [Benchmark]
        public uint Pro()
        {
            return Checksum.ChecksumPro(SourceBytes);
        }

        [Benchmark]
        public uint Senior()
        {
            return Checksum.ChecksumSenior(SourceBytes);
        }

        [Benchmark]
        public uint Hacker()
        {
            return Checksum.ChecksumHacker(SourceBytes);
        }

        [Benchmark]
        public uint Expert()
        {
            return Checksum.ChecksumExpert(SourceBytes);
        }

        [Benchmark]
        public uint ExpertAvx()
        {
            return Checksum.ChecksumExpertAvx(SourceBytes);
        }

        [Benchmark]
        public uint ExpertAvx2()
        {
            return Checksum.ChecksumExpertAvx2(SourceBytes);
        }

        
    }

    public static void Assertions()
    {
        var b = new byte[2048];
        Random.Shared.NextBytes(b);

        foreach (var i in Enumerable.Range(0, 1024))
        {
            var span = b.AsSpan().Slice(0, 1024 + i);
            
            var crc = Checksum.ChecksumJunior(span);
            
            Assert.Equal(crc, Checksum.ChecksumPro(span));
            Assert.Equal(crc, Checksum.ChecksumSenior(span));
            Assert.Equal(crc, Checksum.ChecksumHacker(span));
            Assert.Equal(crc, Checksum.ChecksumExpert(span));
            Assert.Equal(crc, Checksum.ChecksumExpertAvx(span));
            Assert.Equal(crc, Checksum.ChecksumExpertAvx2(span));
        }
        
    }

    public static void Main(string[] args)
    {

        Assertions(); //make sure all versions are correct
        
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

        BenchmarkRunner.Run<ChecksumBenchmark>();

    }

}