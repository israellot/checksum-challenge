using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChecksumChallenge;

internal partial class Checksum
{
    public static uint ChecksumPro2(ReadOnlySpan<byte> arr)
    {
        if (arr.Length == 0) return 0;

        uint sum = 0;
        var i = 0;
        for (i = 0; i <= arr.Length - 4; i+=4)
        {
            sum += (arr[i] << 24) + (arr[i+1] << 16) + (arr[i+2] << 8) + (arr[i+3]);
        }

        for (; i < arr.Length; i++)
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
