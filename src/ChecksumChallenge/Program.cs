using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Globalization;
using Xunit;

namespace ChecksumChallenge;

public class Program
{         
    public class CrcBenchmark
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
            return Crc32.ChecksumJunior(SourceBytes);
        }                

        [Benchmark]
        public uint Pro()
        {
            return Crc32.ChecksumPro(SourceBytes);
        }

        [Benchmark]
        public uint Senior()
        {
            return Crc32.ChecksumSenior(SourceBytes);
        }

        [Benchmark]
        public uint Hacker()
        {
            return Crc32.ChecksumHacker(SourceBytes);
        }

        [Benchmark]
        public uint Expert()
        {
            return Crc32.ChecksumExpert(SourceBytes);
        }

        [Benchmark]
        public uint ExpertAvx()
        {
            return Crc32.ChecksumExpertAvx(SourceBytes);
        }

        [Benchmark]
        public uint ExpertAvx2()
        {
            return Crc32.ChecksumExpertAvx2(SourceBytes);
        }

        
    }

    public static void Assertions()
    {
        var b = new byte[2048];
        Random.Shared.NextBytes(b);

        foreach (var i in Enumerable.Range(0, 1024))
        {
            var span = b.AsSpan().Slice(0, 1024 + i);
            
            var crc = Crc32.ChecksumJunior(span);
            
            Assert.Equal(crc, Crc32.ChecksumPro(span));
            Assert.Equal(crc, Crc32.ChecksumSenior(span));
            Assert.Equal(crc, Crc32.ChecksumHacker(span));
            Assert.Equal(crc, Crc32.ChecksumExpert(span));
            Assert.Equal(crc, Crc32.ChecksumExpertAvx(span));
            Assert.Equal(crc, Crc32.ChecksumExpertAvx2(span));
        }
        
    }

    public static void Main(string[] args)
    {

        Assertions(); //make sure all versions are correct
        
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

        BenchmarkRunner.Run<CrcBenchmark>();

    }

}