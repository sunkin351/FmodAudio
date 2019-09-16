using System;

namespace FmodAudio
{
    public abstract class HandleBase : IDisposable
    {
        internal IntPtr Handle;

        public HandleBase()
        {
        }

        public HandleBase(IntPtr newPtr)
        {
            Handle = newPtr;
        }

        ~HandleBase()
        {
            if (!IsValid)
                return;

            ReleaseImpl();
        }

        internal bool IsValid
        {
            get => Handle != IntPtr.Zero;
        }
        
        public void Release()
        {
            if (!IsValid)
                return;

            ReleaseImpl();

            Handle = IntPtr.Zero;

            GC.SuppressFinalize(this);
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
            return Handle.GetHashCode();
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
