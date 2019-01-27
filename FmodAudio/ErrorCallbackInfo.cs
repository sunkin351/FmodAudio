using System;
using System.Runtime.InteropServices;

namespace FmodAudio
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ErrorCallbackInfo
    {
        public Result Result;                     /* Error code result */
        public ErrorCallbackInstanceType InstanceType;               /* Type of instance the error occurred on */
        public IntPtr Instance;                   /* Instance pointer */
        private IntPtr FunctionName_Internal;      /* Function that the error occurred on */
        private IntPtr FunctionParams_Internal;    /* Function parameters that the error ocurred on */

        public string FunctionName
        {
            get
            {
                return Helpers.PtrToStringUnknownSize(FunctionName_Internal);
            }
        }

        public string FunctionParams
        {
            get
            {
                return Helpers.PtrToStringUnknownSize(FunctionParams_Internal);
            }
        }
    }
}
