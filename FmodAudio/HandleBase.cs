#pragma warning disable CA1063

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FmodAudio
{
    public abstract class HandleBase : IDisposable
    {
        internal IntPtr handle;
        internal bool ownsHandle;

        public IntPtr Handle => handle;

        protected HandleBase()
        {
        }

        protected HandleBase(IntPtr newPtr, bool ownsHandle)
        {
            handle = newPtr;
            this.ownsHandle = ownsHandle;

            if (ownsHandle)
            {
                if (!ClassManagedGCHandle)
                {
                    this.UserData = (IntPtr)GCHandle.Alloc(this, GCHandleType.Weak);
                }
            }
            else
            {
                GC.SuppressFinalize(this);
            }
        }

        ~HandleBase()
        {
            Release();
        }

        internal bool IsValid
        {
            get => handle != default;
        }
        
        internal abstract IntPtr UserData { get; set; }

        /// <summary>
        /// Mechanism for specifying whether this handle's constructor should create a GCHandle for it
        /// </summary>
        internal virtual bool ClassManagedGCHandle => false;

        public void Release() 
        {
            if (!IsValid)
                return;

            if (ownsHandle)
            {
                if (ClassManagedGCHandle)
                {
                    ReleaseImpl();
                }
                else
                {
                    var gcHandle = GCHandle.FromIntPtr(this.UserData);

                    Debug.Assert(gcHandle.IsAllocated && ReferenceEquals(gcHandle.Target, this));

                    ReleaseImpl();

                    gcHandle.Free();
                }

                GC.SuppressFinalize(this);
            }

            handle = IntPtr.Zero;
        }

        protected virtual void ReleaseImpl()
        {
        }

        public void Dispose()
        {
            Release();
        }

        #region Object Equality

        public override bool Equals(object obj)
        {
            return obj is HandleBase tmp && this.Equals(tmp);
        }

        public bool Equals(HandleBase p)
        {
            return ReferenceEquals(this, p) || (!(p is null) && Handle == p.Handle);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Handle);
        }

        public static bool operator ==(HandleBase a, HandleBase b)
        {
            return ReferenceEquals(a, b) || (!(a is null) && !(b is null) && a.Handle == b.Handle);
        }

        public static bool operator !=(HandleBase a, HandleBase b)
        {
            return !(a == b);
        }

        #endregion
        
    }
}
