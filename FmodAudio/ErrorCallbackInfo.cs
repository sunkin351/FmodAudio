using System;
using System.Runtime.InteropServices;

namespace FmodAudio
{
    public struct ErrorCallbackInfoNative
    {
        public Result Result;                     /* Error code result */
        public ErrorCallbackInstanceType InstanceType;               /* Type of instance the error occurred on */
        public IntPtr Instance;                   /* Instance pointer */
        public IntPtr FunctionName;      /* Function that the error occurred on */
        public IntPtr FunctionParams;    /* Function parameters that the error ocurred on */
    }

    public class ErrorCallbackInfo
    {
        public Result Result;                     /* Error code result */
        public ErrorCallbackInstanceType InstanceType;               /* Type of instance the error occurred on */
        public object Instance;                   /* Instance object */
        public string FunctionName;      /* Function that the error occurred on */
        public string FunctionParams;    /* Function parameters that the error ocurred on */

        public ErrorCallbackInfo(ref ErrorCallbackInfoNative native)
        {
            Result = native.Result;
            InstanceType = native.InstanceType;

            switch(native.InstanceType)
            {
                case ErrorCallbackInstanceType.System:
                    Instance = FmodSystem.GetSystem(native.Instance);
                    break;

                default:
                    Instance = null;
                    break;
            }

            FunctionName = Helpers.PtrToStringUnknownSize(native.FunctionName);
            FunctionParams = Helpers.PtrToStringUnknownSize(native.FunctionParams);
        }
    }
}
