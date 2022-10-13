using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChecksumChallenge;

internal partial class Checksum
{
    public static uint ChecksumJunior2(ReadOnlySpan<byte> arr)
    {
        if (arr.Length == 0) return 0;

        uint sum = 0;

        for (var i = 0; i < arr.Length; i++)
        {
            switch (i % 4)
            {
                case 0: sum += arr[i] << 24; break;
                case 1: sum += arr[i] << 16; break;
                case 2: sum += arr[i] <<  8; break;
                case 3: sum += arr[i] <<  0; break;
            }
        }

        return sum;
    }
}
