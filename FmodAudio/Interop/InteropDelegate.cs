using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace FmodAudio.Interop
{
    static class DelegateILGeneration
    {
        static int MethodNumber = 0;
        
        static readonly ConstructorInfo ArgNullExceptConstructor;

        static DelegateILGeneration()
        {
            ArgNullExceptConstructor = typeof(ArgumentNullException).GetConstructor(new Type[] { typeof(string) });
        }

        /// <summary>
        /// Generates an indirect call to a native function pointer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GenerateCalli<T>() where T: Delegate
        {
            Signature sig = GetDelegateSignature(typeof(T));

            //First argument of the delegate needs to be the Function Pointer
            if (sig.Parameters.Length == 0 || sig.Parameters[0].ParameterType != typeof(IntPtr))
            {
                throw new ArgumentException("Invalid Delegate parameters. First param needs to be IntPtr, for the Function Pointer");
            }

            int len = sig.Parameters.Length;
            var ArgPinList = new List<PinnedRefArgument>(len - 1);

            //All arguments must be Blittable structs
            for (int i = 1; i < len; ++i)
            {
                var type = sig.Parameters[i].ParameterType;
                CheckType(type);

                if (type.IsByRef) //Setup for argument Pinning
                {
                    ArgPinList.Add(new PinnedRefArgument(i, type));
                }
            }

            //Return Type must also be blittable
            if (sig.ReturnType != typeof(void))
                CheckType(sig.ReturnType);

            //List of argument types
            var paramList = sig.Parameters.Select(o => o.ParameterType).ToArray();
            
            var method = new DynamicMethod("CustomStaticDelegate" + MethodNumber++, sig.ReturnType, paramList);

            var gen = method.GetILGenerator();

            var FunctionPointerNullLabel = gen.DefineLabel();
            
            //Test if the function pointer is null
            gen.Emit(OpCodes.Ldc_I4_0);
            gen.Emit(OpCodes.Conv_I);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Beq, FunctionPointerNullLabel);
            
            if (len > 1)
            {
                //Prepare for native call by pinning all references
                for (int i = 0; i < ArgPinList.Count; ++i)
                {
                    var arg = ArgPinList[i];

                    var local = gen.DeclareLocal(arg.PinnedType, true);

                    gen.Emit(OpCodes.Ldarg, (short)arg.ArgumentIndex);
                    gen.Emit(OpCodes.Stloc, local);
                }

                //Load all arguments onto the stack
                for (short i = 1; i < len; ++i)
                {
                    gen.Emit(OpCodes.Ldarg, i);
                }
            }

            //Load Function Pointer onto the stack
            gen.Emit(OpCodes.Ldarg_0);

            //Make the call
            gen.EmitCalli(OpCodes.Calli, CallingConvention.StdCall, sig.ReturnType, paramList.Skip(1).ToArray());

            //Return Immediately
            gen.Emit(OpCodes.Ret);

            //If the Function pointer is null
            gen.MarkLabel(FunctionPointerNullLabel);
            gen.Emit(OpCodes.Ldstr, "Function_Pointer");
            gen.Emit(OpCodes.Newobj, ArgNullExceptConstructor);
            gen.Emit(OpCodes.Throw);

            return method.CreateDelegate<T>();
        }

        static Signature GetDelegateSignature(Type del)
        {
            Signature sig = default;

            var method = del.GetMethod("Invoke");

            sig.ReturnType = method.ReturnType;
            sig.Parameters = method.GetParameters();

            return sig;
        }

        static void CheckType(Type type)
        {
            if (type.IsByRef)
            {
                type = type.GetElementType();
            }

            if (type.IsPrimitive || type.IsEnum || type.IsPointer)
                return;

            if (type.IsClass)
            {
                throw new ArgumentException($"Invalid Delegate Parameters. One or more parameters are not unmanaged blittable types.");
            }

            foreach (var field in type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                CheckType(field.FieldType);
            }
        }

        static T CreateDelegate<T>(this DynamicMethod method) where T : Delegate
        {
            return (T)method.CreateDelegate(typeof(T));
        }

        struct Signature
        {
            public Type ReturnType;
            public ParameterInfo[] Parameters;
        }

        struct PinnedRefArgument
        {
            public int ArgumentIndex;
            public Type PinnedType;

            public PinnedRefArgument(int argIndex, Type type)
            {
                ArgumentIndex = argIndex;
                PinnedType = type;
            }
        }

    }
}
