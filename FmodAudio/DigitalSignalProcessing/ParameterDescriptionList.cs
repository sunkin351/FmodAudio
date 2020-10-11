using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using FmodAudio.DigitalSignalProcessing.Interop;

namespace FmodAudio.DigitalSignalProcessing
{
    public class ParameterDescriptionList
    {
        private readonly ParameterDescription[] ManagedArray;
        private ParameterDescriptionStruct[]? StructArray;

        [NotNull]
        private IntPtr[]? PointerList;

        public ParameterDescriptionList(ParameterDescription[] array)
        {
            if (array.Length == 0)
            {
                throw new ArgumentException("array length must be greater than 0");
            }

            ManagedArray = array;
        }

        public ParameterDescription[] List => ManagedArray;

        internal unsafe void GetPointerAndCount(out ParameterDescriptionStruct** ptr, out int length)
        {
            EnsureInitialized();

            ptr = (ParameterDescriptionStruct**)Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(PointerList));
            length = PointerList.Length;
        }

        private unsafe void EnsureInitialized()
        {
            if (Volatile.Read(ref PointerList) == null)
            {
                lock (ManagedArray)
                {
                    if (PointerList != null)
                        return;

                    StructArray = GC.AllocateArray<ParameterDescriptionStruct>(ManagedArray.Length, true);
                    var list = GC.AllocateArray<IntPtr>(ManagedArray.Length, true);

                    for (int i = 0; i < ManagedArray.Length; ++i)
                    {
                        ref var elem = ref StructArray[i];

                        elem = ManagedArray[i].internalDescription;

                        list[i] = (IntPtr)Unsafe.AsPointer(ref elem);
                    }

                    Volatile.Write(ref PointerList, list);
                }
            }
        }
    }
}
