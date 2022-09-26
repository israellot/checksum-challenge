using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChecksumChallenge;

internal partial class Checksum
{
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



}
