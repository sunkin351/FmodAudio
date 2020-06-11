using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

#nullable enable

namespace FmodAudio.Dsp
{
    internal unsafe sealed class SystemDefinedDsp : DSP
    {
        internal DspDescription? Description;
        private readonly bool OwnHandle = false;

        public SystemDefinedDsp(FmodSystem sys, IntPtr handle) : base(sys, handle)
        {
            GC.SuppressFinalize(this);
        }

        public SystemDefinedDsp(FmodSystem sys, IntPtr handle, bool ownsHandle) : base (sys, handle)
        {
            if (!ownsHandle)
            {
                GC.SuppressFinalize(this);
            }
            else
            {
                var gcHandle = FmodHelpers.CreateGCHandle(this);

                library.DSP_SetUserData(this.Handle, GCHandle.ToIntPtr(gcHandle)).CheckResult();
            }

            this.OwnHandle = ownsHandle;
        }

        public override int ParameterCount
        {
            get
            {
                if (Description != null)
                {
                    return Description.ParameterCount;
                }
                else
                {
                    int count;

                    library.DSP_GetNumParameters(this.Handle, &count).CheckResult();

                    return count;
                }
            }
        }

        protected override void ReleaseImpl()
        {
            if (this.OwnHandle)
            {
                library.DSP_Release(Handle);
            }
        }

        public override ParameterDescription GetParameterInfo(int index)
        {
            if ((uint)index >= (uint)this.ParameterCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            Interop.ParameterDescriptionStruct* ptr;

            library.DSP_GetParameterInfo(Handle, index, &ptr).CheckResult();

            return ParameterDescription.CreateFromPointer(ptr);
        }
    }
}
