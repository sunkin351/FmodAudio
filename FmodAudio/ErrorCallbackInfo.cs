#pragma warning disable CA1815

using System;

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
        public IntPtr Instance;                         /* Instance object */
        public string FunctionName;                     /* Function that the error occurred on */
        public string FunctionParams;                   /* Function parameters that the error ocurred on */

        public ErrorCallbackInfo(ref ErrorCallbackInfoNative native)
        {
            Result = native.Result;

            InstanceType = native.InstanceType;

            Instance = native.Instance;

            FunctionName = FmodHelpers.PtrToStringUnknownSize(native.FunctionName);
            FunctionParams = FmodHelpers.PtrToStringUnknownSize(native.FunctionParams);
        }
    }
}
