using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChecksumChallenge;

internal partial class Checksum
{
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

}
