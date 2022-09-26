using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChecksumChallenge;

internal partial class Checksum
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


}
