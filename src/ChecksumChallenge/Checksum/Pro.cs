using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChecksumChallenge;

internal partial class Checksum
{
public static uint ChecksumPro(ReadOnlySpan<byte> arr)
{
    if (arr.Length == 0) return 0;

    uint sum0 = 0, sum1 = 0, sum2 = 0, sum3 = 0;
    int z = 0;

    int rem = arr.Length % 16;
    var limit = arr.Length - rem;

    while (z < limit)
    {
        sum0 += (uint)(arr[z + 0] + arr[z + 4] + arr[z + 8] + arr[z + 12]);
        sum1 += (uint)(arr[z + 1] + arr[z + 5] + arr[z + 9] + arr[z + 13]);
        sum2 += (uint)(arr[z + 2] + arr[z + 6] + arr[z + 10] + arr[z + 14]);
        sum3 += (uint)(arr[z + 3] + arr[z + 7] + arr[z + 11] + arr[z + 15]);

        z += 16;
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



}
