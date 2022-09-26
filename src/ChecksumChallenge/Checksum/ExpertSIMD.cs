using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace ChecksumChallenge;

internal partial class Checksum
{

    public static unsafe uint ChecksumExpertAvx(ReadOnlySpan<byte> arr)
    {
        if (Avx.IsSupported)
        {
            fixed (byte* ptr = arr)
            {
                ref byte refSpan = ref MemoryMarshal.GetReference<byte>(arr);

                uint z = 0;
                uint sum = 0;

                var vectorSum = Avx.Xor(Vector128<byte>.Zero, Vector128<byte>.Zero).AsUInt32();

                var mask = Vector128.Create((byte)3, 2, 1, 0, 7, 6, 5, 4, 11, 10, 9, 8, 15, 14, 13, 12);
                mask = Avx.Or(mask, Vector128<byte>.Zero);

                uint limit = (uint)arr.Length - 64;

                while (z <= limit)
                {
                    var v1 = Avx.LoadVector128(ptr + z);
                    var v2 = Avx.LoadVector128(ptr + z + 16);
                    var v3 = Avx.LoadVector128(ptr + z + 32);
                    var v4 = Avx.LoadVector128(ptr + z + 48);

                    var s1 = Avx.Shuffle(v1, mask).AsUInt32();
                    var s2 = Avx.Shuffle(v2, mask).AsUInt32();
                    var s3 = Avx.Shuffle(v3, mask).AsUInt32();
                    var s4 = Avx.Shuffle(v4, mask).AsUInt32();

                    var s5 = Avx.Add(Avx.Add(s1, s2), Avx.Add(s3, s4));

                    vectorSum = Avx.Add(vectorSum, s5);

                    z += 64;
                }

                limit = (uint)arr.Length - 16;
                while (z <= limit)
                {
                    var v1 = Avx.LoadVector128(ptr + z);
                    var s1 = Avx.Shuffle(v1, mask).AsUInt32();
                    vectorSum = Avx.Add(vectorSum, s1);

                    z += 16;
                }



                sum += vectorSum.GetElement(0) + vectorSum.GetElement(1) + vectorSum.GetElement(2) + vectorSum.GetElement(3);


                limit = (uint)arr.Length - 4;
                while (z <= limit)
                {
                    sum += BinaryPrimitives.ReverseEndianness(*(uint*)(ptr + z));
                    z += 4;
                }

                uint rem = (uint)arr.Length - z;

                switch (rem & 3)
                {
                    case 3:
                        sum += (uint)(*(ptr + z + 2)) << 8;
                        sum += (uint)(*(ptr + z + 1)) << 16;
                        sum += (uint)(*(ptr + z)) << 24;
                        break;
                    case 2:
                        sum += (uint)(*(ptr + z + 1)) << 16;
                        sum += (uint)(*(ptr + z)) << 24;
                        break;
                    case 1:
                        sum += (uint)(*(ptr + z)) << 24;
                        break;
                }

                return sum;
            }

        }
        else
            return 0;
    }

    public static unsafe uint ChecksumExpertAvx2(ReadOnlySpan<byte> arr)
    {
        if (Avx2.IsSupported)
        {
            fixed (byte* ptr = arr)
            {
                uint z = 0;
                uint sum = 0;

                var vectorSum = Avx2.Xor(Vector256<byte>.Zero, Vector256<byte>.Zero).AsUInt32();

                var mask = Vector256.Create((byte)3, 2, 1, 0, 7, 6, 5, 4, 11, 10, 9, 8, 15, 14, 13, 12, (byte)3, 2, 1, 0, 7, 6, 5, 4, 11, 10, 9, 8, 15, 14, 13, 12);
                mask = Avx2.Or(mask, Vector256<byte>.Zero);//hints mask to stay in a fixed register

                uint limit = (uint)arr.Length - 128;

                while (z <= limit)
                {
                    var v1 = Avx2.LoadVector256(ptr + z);
                    var v2 = Avx2.LoadVector256(ptr + z + 32);
                    var v3 = Avx2.LoadVector256(ptr + z + 64);
                    var v4 = Avx2.LoadVector256(ptr + z + 96);

                    var s1 = Avx2.Shuffle(v1, mask).AsUInt32();
                    var s2 = Avx2.Shuffle(v2, mask).AsUInt32();
                    var s3 = Avx2.Shuffle(v3, mask).AsUInt32();
                    var s4 = Avx2.Shuffle(v4, mask).AsUInt32();

                    var s9 = Avx2.Add(Avx2.Add(s1, s2), Avx2.Add(s3, s4));

                    vectorSum = Avx2.Add(vectorSum, s9);

                    z += 128;
                }

                limit = (uint)arr.Length - 32;
                while (z <= limit)
                {
                    var v1 = Avx2.LoadVector256(ptr + z);
                    var s1 = Avx2.Shuffle(v1, mask).AsUInt32();
                    vectorSum = Avx2.Add(vectorSum, s1);

                    z += 32;
                }


                var zero = Avx.Xor(Vector128<byte>.Zero, Vector128<byte>.Zero).AsInt32();
                var reduced = Avx.HorizontalAdd(vectorSum.GetLower().AsInt32(), vectorSum.GetUpper().AsInt32());
                reduced = Avx.HorizontalAdd(reduced, zero);
                reduced = Avx.HorizontalAdd(reduced, zero);

                sum += reduced.AsUInt32().GetElement(0);

                limit = (uint)arr.Length - 4;
                while (z <= limit)
                {
                    sum += BinaryPrimitives.ReverseEndianness(*(uint*)(ptr + z));
                    z += 4;
                }

                uint rem = (uint)arr.Length - z;
                switch (rem & 3)
                {
                    case 3:
                        sum += (uint)(*(ptr + z + 2)) << 8;
                        sum += (uint)(*(ptr + z + 1)) << 16;
                        sum += (uint)(*(ptr + z)) << 24;
                        break;
                    case 2:
                        sum += (uint)(*(ptr + z + 1)) << 16;
                        sum += (uint)(*(ptr + z)) << 24;
                        break;
                    case 1:
                        sum += (uint)(*(ptr + z)) << 24;
                        break;
                }

                return sum;
            }
        }
        else
            return 0;
    }
}
