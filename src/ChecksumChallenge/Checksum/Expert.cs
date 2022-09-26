using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;

namespace ChecksumChallenge;

internal partial class Checksum
{
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


}
