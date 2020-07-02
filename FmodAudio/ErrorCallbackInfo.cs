#pragma warning disable CA1815

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using FmodAudio.Dsp;

namespace FmodAudio
{
    public struct ErrorCallbackInfoNative
    {
        public Result Result;                           /* Error code result */
        public ErrorCallbackInstanceType InstanceType;  /* Type of instance the error occurred on */
        public IntPtr Instance;                         /* Instance pointer */
        public IntPtr FunctionName;                     /* Function that the error occurred on */
        public IntPtr FunctionParams;                   /* Function parameters that the error ocurred on */
    }

    public class ErrorCallbackInfo
    {
        public Result Result;                           /* Error code result */
        public ErrorCallbackInstanceType InstanceType;  /* Type of instance the error occurred on */
        public object Instance;                         /* Instance object */
        public string FunctionName;                     /* Function that the error occurred on */
        public string FunctionParams;                   /* Function parameters that the error ocurred on */

        public ErrorCallbackInfo(ref ErrorCallbackInfoNative native)
        {
            Result = native.Result;

            InstanceType = native.InstanceType;

            switch (native.InstanceType)
            {
                case ErrorCallbackInstanceType.System:
                    Instance = FmodSystem.FromHandle(native.Instance);
                    break;

                case ErrorCallbackInstanceType.Channel:
                    Instance = Channel.FromHandle(native.Instance);
                    break;

                case ErrorCallbackInstanceType.ChannelGroup:
                    Instance = ChannelGroup.FromHandle(native.Instance);

                    if (Instance is null)
                    {
                        Fmod.Library.ChannelGroup_GetSystemObject(native.Instance, out var sysHandle).CheckResult();

                        Instance = new ChannelGroup(FmodSystem.FromHandle(sysHandle), native.Instance, false);
                    }
                    break;

                case ErrorCallbackInstanceType.ChannelControl:
                    Instance = ChannelGroup.FromHandle(native.Instance);

                    if (Instance is null)
                    {
                        Instance = new ChannelControlHandle(native.Instance);
                    }
                    break;

                case ErrorCallbackInstanceType.Sound:
                    Instance = Sound.FromHandle(native.Instance);

                    if (Instance is null)
                    {
                        Fmod.Library.Sound_GetSystemObject(native.Instance, out var sysHandle).CheckResult();

                        Instance = new Sound(FmodSystem.FromHandle(sysHandle), native.Instance, false);
                    }
                    break;

                case ErrorCallbackInstanceType.SoundGroup:
                    Instance = SoundGroup.FromHandle(native.Instance);

                    if (Instance is null)
                    {
                        Fmod.Library.SoundGroup_GetSystemObject(native.Instance, out IntPtr sysHandle).CheckResult();

                        Instance = new SoundGroup(FmodSystem.FromHandle(sysHandle), native.Instance, false);
                    }
                    break;

                case ErrorCallbackInstanceType.DSP:
                    Instance = DSP.FromHandle(native.Instance);

                    if (Instance is null)
                    {
                        Fmod.Library.DSP_GetSystemObject(native.Instance, out IntPtr sysHandle).CheckResult();

                        Instance = new SystemDefinedDsp(FmodSystem.FromHandle(sysHandle), native.Instance, false);
                    }
                    break;

                case ErrorCallbackInstanceType.DSPConnection:
                    Instance = new DSPConnection(native.Instance);
                    break;

                case ErrorCallbackInstanceType.Geometry:
                    Instance = Geometry.FromHandle(native.Instance);
                    break;

                case ErrorCallbackInstanceType.Reverb3D:
                    Instance = new Reverb3D(native.Instance);
                    break;
                
                default:
                    throw new NotSupportedException("Unsupported instance type");
            }

            FunctionName = FmodHelpers.PtrToStringUnknownSize(native.FunctionName);
            FunctionParams = FmodHelpers.PtrToStringUnknownSize(native.FunctionParams);
        }

        private sealed class ChannelControlHandle : ChannelControl
        {
            private static FmodSystem GetSystemForHandle(IntPtr handle)
            {
                Fmod.Library.ChannelGroup_GetSystemObject(handle, out IntPtr syshandle).CheckResult();

                return FmodSystem.FromHandle(syshandle);
            }

            public ChannelControlHandle(IntPtr handle) : base(GetSystemForHandle(handle), handle, false)
            {
            }
        }
    }
}
