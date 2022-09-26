using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace ChecksumChallenge;

public class Crc32
{


    public static uint ChecksumJunior(ReadOnlySpan<byte> arr)
    {
        if (arr.Length == 0) return 0;

        uint sum0 = 0, sum1 = 0, sum2 = 0, sum3 = 0;

        for (var i = 0; i < arr.Length; i++)
        {
            switch (i % 4)
            {
                case 0: sum0 += arr[i]; break;
                case 1: sum1 += arr[i]; break;
                case 2: sum2 += arr[i]; break;
                case 3: sum3 += arr[i]; break;
            }
        }

        var sum = sum3 + (sum2 << 8) + (sum1 << 16) + (sum0 << 24);

        return sum;
    }

    public static uint ChecksumPro(ReadOnlySpan<byte> arr)
    {
        if (arr.Length == 0) return 0;

        uint sum0 = 0, sum1 = 0, sum2 = 0, sum3 = 0;
        int z = 0;

        int rem = arr.Length % 32;
        var limit = arr.Length - rem;

        while (z < limit)
        {
            sum0 += (uint)(arr[z + 0] + arr[z + 4] + arr[z + 8] + arr[z + 12]
                    + arr[z + 16] + arr[z + 20] + arr[z + 24] + arr[z + 28]);

            sum1 += (uint)(arr[z + 1] + arr[z + 5] + arr[z + 9] + arr[z + 13]
                    + arr[z + 17] + arr[z + 21] + arr[z + 25] + arr[z + 29]);

            sum2 += (uint)(arr[z + 2] + arr[z + 6] + arr[z + 10] + arr[z + 14]
                    + arr[z + 18] + arr[z + 22] + arr[z + 26] + arr[z + 30]);

            sum3 += (uint)(arr[z + 3] + arr[z + 7] + arr[z + 11] + arr[z + 15]
                    + arr[z + 19] + arr[z + 23] + arr[z + 27] + arr[z + 31]);

            z += 32;
        }

        limit = arr.Length - 4;
        while (z <= limit)
        {
            sum0 += arr[z + 0];
            sum1 += arr[z + 1];
            sum2 += arr[z + 2];
            sum3 += arr[z + 3];
            z += 4;
        }

        var sum = sum3 + (sum2 << 8) + (sum1 << 16) + (sum0 << 24);

        rem = arr.Length - z;
        switch (rem & 3)
        {
            case 3:
                sum += (uint)(arr[z + 2]) << 8;
                sum += (uint)(arr[z + 1]) << 16;
                sum += (uint)(arr[z + 0]) << 24;
                break;
            case 2:
                sum += (uint)(arr[z + 1]) << 16;
                sum += (uint)(arr[z + 0]) << 24;
                break;
            case 1:
                sum += (uint)(arr[z + 0]) << 24;
                break;
        }

        return sum;
    }

    public unsafe static uint ChecksumSenior(ReadOnlySpan<byte> arr)
    {
        if (arr.Length == 0) return 0;

        fixed (byte* ptr = arr)
        {
            uint sum0 = 0, sum1 = 0, sum2 = 0, sum3 = 0;
            uint z = 0;


            var limit = arr.Length - 32;
            while (z <= limit)
            {
                sum0 += (uint)(*(ptr + z) + *(ptr + z + 4) + *(ptr + z + 8) + *(ptr + z + 12)
                        + *(ptr + z + 16) + *(ptr + z + 20) + *(ptr + z + 24) + *(ptr + z + 28));

                sum1 += (uint)(*(ptr + z + 1) + *(ptr + z + 5) + *(ptr + z + 9) + *(ptr + z + 13)
                        + *(ptr + z + 17) + *(ptr + z + 21) + *(ptr + z + 25) + *(ptr + z + 29));

                sum2 += (uint)(*(ptr + z + 2) + *(ptr + z + 6) + *(ptr + z + 10) + *(ptr + z + 14)
                        + *(ptr + z + 18) + *(ptr + z + 22) + *(ptr + z + 26) + *(ptr + z + 30));

                sum3 += (uint)(*(ptr + z + 3) + *(ptr + z + 7) + *(ptr + z + 11) + *(ptr + z + 15)
                        + *(ptr + z + 19) + *(ptr + z + 23) + *(ptr + z + 27) + *(ptr + z + 31));

                z += 32;
            }

            limit = arr.Length - 4;
            while (z <= limit)
            {
                sum0 += *(ptr + z);
                sum1 += *(ptr + z + 1);
                sum2 += *(ptr + z + 2);
                sum3 += *(ptr + z + 3);
                z += 4;
            }

            var sum = sum3 + (sum2 << 8) + (sum1 << 16) + (sum0 << 24);

            var rem = arr.Length - z;
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

    public unsafe static uint ChecksumHacker(ReadOnlySpan<byte> arr)
    {
        if (arr.Length == 0) return 0;

        fixed (byte* ptr = arr)
        {
            uint sum0 = 0, sum1 = 0, sum2 = 0, sum3 = 0;
            uint z = 0;

            ulong tmp0 = 0, tmp1 = 0;
            int limit2 = 64;

            int limit = arr.Length - 32;
            while (z <= limit)
            {
                ulong l1 = *(ulong*)(ptr + z);
                ulong l2 = *(ulong*)(ptr + z + 8);
                ulong l3 = *(ulong*)(ptr + z + 16);
                ulong l4 = *(ulong*)(ptr + z + 24);

                tmp0 += (l1 & 0x00ff00ff00ff00ff) + (l2 & 0x00ff00ff00ff00ff)
                    + (l3 & 0x00ff00ff00ff00ff) + (l4 & 0x00ff00ff00ff00ff);

                tmp1 += ((l1 & 0xff00ff00ff00ff00) >> 8) + ((l2 & 0xff00ff00ff00ff00) >> 8)
                    + ((l3 & 0xff00ff00ff00ff00) >> 8) + ((l4 & 0xff00ff00ff00ff00) >> 8);

                limit2--;
                if (limit2 == 0)
                {
                    sum0 += (uint)(tmp0 & 0xffff) + (uint)((tmp0 & 0x0000ffff00000000) >> 32);
                    sum1 += (uint)(tmp1 & 0xffff) + (uint)((tmp1 & 0x0000ffff00000000) >> 32);
                    sum2 += (uint)((tmp0 & 0xffff0000) >> 16) + (uint)(tmp0 >> 48);
                    sum3 += (uint)((tmp1 & 0xffff0000) >> 16) + (uint)(tmp1 >> 48);

                    tmp0 = 0; tmp1 = 0; limit2 = 64;
                }

                z += 32;
            }

            if (limit2 < 64)
            {
                sum0 += (uint)(tmp0 & 0xffff) + (uint)((tmp0 & 0x0000ffff00000000) >> 32);
                sum1 += (uint)(tmp1 & 0xffff) + (uint)((tmp1 & 0x0000ffff00000000) >> 32);
                sum2 += (uint)((tmp0 & 0xffff0000) >> 16) + (uint)(tmp0 >> 48);
                sum3 += (uint)((tmp1 & 0xffff0000) >> 16) + (uint)(tmp1 >> 48);
            }

            limit = arr.Length - 4;
            while (z <= limit)
            {
                sum0 += *(ptr + z);
                sum1 += *(ptr + z + 1);
                sum2 += *(ptr + z + 2);
                sum3 += *(ptr + z + 3);
                z += 4;
            }

            var sum = sum3 + (sum2 << 8) + (sum1 << 16) + (sum0 << 24);

            var rem = arr.Length - z;
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

    public unsafe static uint ChecksumHackerExpanded(ReadOnlySpan<byte> arr)
    {
        if (arr.Length == 0) return 0;

        fixed (byte* ptr = arr)
        {
            uint sum0 = 0, sum1 = 0, sum2 = 0, sum3 = 0;
            uint z = 0;

            ulong tmp0 = 0, tmp1 = 0;
            int limit2 = 64;

            int limit = arr.Length - 32;
            while (z <= limit)
            {
                ulong l1 = *(ulong*)(ptr + z);
                ulong l2 = *(ulong*)(ptr + z + 8);
                ulong l3 = *(ulong*)(ptr + z + 16);
                ulong l4 = *(ulong*)(ptr + z + 24);

                // 6 4 2 0
                var l11 = l1 & 0x00_ff_00_ff_00_ff_00_ff;

                // 7 5 3 1
                var l12 = (l1 & 0xff_00_ff_00_ff_00_ff_00) >> 8;

                // 14 12 10 8
                var l21 = l2 & 0x00_ff_00_ff_00_ff_00_ff;

                // 15 13 11 9
                var l22 = (l2 & 0xff_00_ff_00_ff_00_ff_00) >> 8;

                // 22 20 18 16
                var l31 = l3 & 0x00_ff_00_ff_00_ff_00_ff;

                // 23 21 19 17
                var l32 = (l3 & 0xff_00_ff_00_ff_00_ff_00) >> 8;

                // 30 28 26 24
                var l41 = (l4 & 0x00_ff_00_ff_00_ff_00_ff);

                // 31 29 27 25
                var l42 = (l4 & 0xff_00_ff_00_ff_00_ff_00) >> 8;

                //(6+14+22+30) (4+12+20+28) (2+10+18+26) (0+8+16+24)
                tmp0 += l11 + l21 + l31 + l41;

                //(7+15+23+31) (5+13+21+29) (3+11+19+27) (1+9+17+25)
                tmp1 += l12 + l22 + l32 + l42;

                limit2--;
                if (limit2 == 0)
                {
                    //(0+8+18+24)
                    var s01 = (uint)(tmp0 & 0xff_ff);
                    //(4+12+20+28)
                    var s02 = (uint)((tmp0 & 0x00_00_ff_ff_00_00_00_00) >> 32);
                    sum0 += s01 + s02;

                    //(1+9+17+25)
                    var s11 = (uint)(tmp1 & 0xff_ff);
                    //(5+13+21+29)
                    var s12 = (uint)((tmp1 & 0x00_00_ff_ff_00_00_00_00) >> 32);
                    sum1 += s11 + s12;

                    //(2+10+18+26)
                    var s21 = (uint)((tmp0 & 0xff_ff_00_00) >> 16);
                    //(6+14+22+30)
                    var s22 = (uint)(tmp0 >> 48);
                    sum2 += s21 + s22;

                    //(3+11+19+27)
                    var s31 = (uint)((tmp1 & 0xff_ff_00_00) >> 16);
                    //(7+15+23+31)
                    var s32 = (uint)(tmp1 >> 48);
                    sum3 += s31 + s32;

                    tmp0 = 0;
                    tmp1 = 0;
                    limit2 = 64;
                }

                z += 32;
            }

            if (limit2 < 64)
            {
                //(0+8+18+24)
                var s01 = (uint)(tmp0 & 0xff_ff);
                //(4+12+20+28)
                var s02 = (uint)((tmp0 & 0x00_00_ff_ff_00_00_00_00) >> 32);
                sum0 += s01 + s02;

                //(1+9+17+25)
                var s11 = (uint)(tmp1 & 0xff_ff);
                //(5+13+21+29)
                var s12 = (uint)((tmp1 & 0x00_00_ff_ff_00_00_00_00) >> 32);
                sum1 += s11 + s12;

                //(2+10+18+26)
                var s21 = (uint)((tmp0 & 0xff_ff_00_00) >> 16);
                //(6+14+22+30)
                var s22 = (uint)(tmp0 >> 48);
                sum2 += s21 + s22;

                //(3+11+19+27)
                var s31 = (uint)((tmp1 & 0xff_ff_00_00) >> 16);
                //(7+15+23+31)
                var s32 = (uint)(tmp1 >> 48);
                sum3 += s31 + s32;
            }


            limit = arr.Length - 4;

            while (z <= limit)
            {
                sum0 += *(ptr + z);
                sum1 += *(ptr + z + 1);
                sum2 += *(ptr + z + 2);
                sum3 += *(ptr + z + 3);
                z += 4;
            }

            var sum = sum3 + (sum2 << 8) + (sum1 << 16) + (sum0 << 24);

            var rem = arr.Length - z;
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

    public static unsafe uint ChecksumExpert(ReadOnlySpan<byte> arr)
    {
        if (arr.Length == 0) return 0;

        fixed (byte* ptr = arr)
        {
            uint sum = 0;
            int z = 0;

            var limit = arr.Length - 32;
            while (z <= limit)
            {
                sum += BinaryPrimitives.ReverseEndianness(*(uint*)(ptr + z));
                sum += BinaryPrimitives.ReverseEndianness(*(uint*)(ptr + z + 4));
                sum += BinaryPrimitives.ReverseEndianness(*(uint*)(ptr + z + 8));
                sum += BinaryPrimitives.ReverseEndianness(*(uint*)(ptr + z + 12));
                sum += BinaryPrimitives.ReverseEndianness(*(uint*)(ptr + z + 16));
                sum += BinaryPrimitives.ReverseEndianness(*(uint*)(ptr + z + 20));
                sum += BinaryPrimitives.ReverseEndianness(*(uint*)(ptr + z + 24));
                sum += BinaryPrimitives.ReverseEndianness(*(uint*)(ptr + z + 28));

                z += 32;
            }

            limit = arr.Length - 4;
            while (z <= limit)
            {
                sum += BinaryPrimitives.ReverseEndianness(*(uint*)(ptr + z));
                z += 4;
            }

            int rem = (arr.Length - z);

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
                    var v2 = Avx.LoadVector128(ptr + z+16);
                    var v3 = Avx.LoadVector128(ptr + z+32);
                    var v4 = Avx.LoadVector128(ptr + z+48);

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
