using System.Buffers.Binary;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace ChecksumChallenge;

internal static class ChecksumMarcelo
{
    /// <summary>
    /// Attempt to extract most juice out of the ExpertAvx2 version, but in vain.
    /// RAM speed is the bottleneck.
    /// Made corrections.
    /// </summary>
    /// <param name="arr">The array of bytes to calculate the checksum from</param>
    /// <returns>0 if AVX2 is not supported in your architecture, otherwise the checksum.</returns>
    public static unsafe uint Get(ReadOnlySpan<byte> arr)
    {
        // Going straight into the *fixed* block.
        fixed (byte* ptr = arr)
        {
            if (!Avx2.IsSupported)
            {
                return 0u;
            }

            uint sum = 0u;
            int z = 0;
            int limit;

            // Block below is meant to isolate the scope of *mask* and *vectorSum*.
            // Trying to allocate *sum* and *z* into fixed 32-bit CPU registers.
            {
                Vector256<byte> mask;
                Vector256<uint> vectorSum;

                vectorSum = Vector256<byte>.Zero.AsUInt32();
                vectorSum = Avx2.Or(vectorSum, vectorSum);
                vectorSum = Avx2.Or(vectorSum, vectorSum);

                mask = Vector256.Create((byte)3, 2, 1, 0, 7, 6, 5, 4, 11, 10, 9, 8, 15, 14, 13, 12, (byte)3, 2, 1, 0, 7, 6, 5, 4, 11, 10, 9, 8, 15, 14, 13, 12);
                mask = Avx2.Or(mask, mask);
                mask = Avx2.Or(mask, mask);

                limit = arr.Length - 128;
                while (z <= limit)
                {
                    Vector256<uint> v1;

                    // 1
                    v1 = Avx2.Shuffle(Avx2.LoadVector256(ptr + z), mask).AsUInt32();
                    vectorSum = Avx2.Add(vectorSum, v1);

                    // 2
                    v1 = Avx2.Shuffle(Avx2.LoadVector256(ptr + z + 32), mask).AsUInt32();
                    vectorSum = Avx2.Add(vectorSum, v1);

                    // 3
                    v1 = Avx2.Shuffle(Avx2.LoadVector256(ptr + z + 64), mask).AsUInt32();
                    vectorSum = Avx2.Add(vectorSum, v1);

                    // 4
                    v1 = Avx2.Shuffle(Avx2.LoadVector256(ptr + z + 96), mask).AsUInt32();
                    vectorSum = Avx2.Add(vectorSum, v1);

                    z += 128;
                }

                limit = arr.Length - 32;
                while (z <= limit)
                {
                    Vector256<uint> v1;

                    v1 = Avx2.Shuffle(Avx2.LoadVector256(ptr + z), mask).AsUInt32();
                    vectorSum = Avx2.Add(vectorSum, v1);

                    z += 32;
                }

                // Summing eight unsigned integers.
                sum += vectorSum.GetElement(0) + vectorSum.GetElement(1) + vectorSum.GetElement(2) + vectorSum.GetElement(3) +
                    vectorSum.GetElement(4) + vectorSum.GetElement(5) + vectorSum.GetElement(6) + vectorSum.GetElement(7);
            }

            limit = arr.Length - 4;
            while (z <= limit)
            {
                sum += BinaryPrimitives.ReverseEndianness(*(uint*)(ptr + z));

                z += 4;
            }

            switch ((arr.Length - z) & 3)
            {
                case 3:
                    sum += ((uint)(*(ptr + z + 2)) << 8) | ((uint)(*(ptr + z + 1)) << 16) | ((uint)(*(ptr + z)) << 24);
                    break;
                case 2:
                    sum += ((uint)(*(ptr + z + 1)) << 16) | ((uint)(*(ptr + z)) << 24);
                    break;
                case 1:
                    sum += (uint)(*(ptr + z)) << 24;
                    break;
                default:
                    break;
            }

            return sum;
        }
    }
}
