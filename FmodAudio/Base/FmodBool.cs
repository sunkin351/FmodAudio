using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FmodAudio.Base
{
    public readonly struct FmodBool
    {
        internal readonly int value;

        internal FmodBool(int val)
        {
            value = val;
        }

        public static implicit operator bool(FmodBool val)
        {
            return val.value != 0;
        }

        public static implicit operator FmodBool(bool val)
        {
            return new FmodBool(val ? 1 : 0);
        }
    }
}
