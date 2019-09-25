using System;

namespace FmodAudio
{
    using Interop;
    public class Geometry : HandleBase
    {
        private readonly IFmodLibrary library;

        internal Geometry(IntPtr handle, IFmodLibrary lib) : base(handle)
        {
            library = lib;
        }

        protected override void ReleaseImpl()
        {
            library.Geometry_Release(Handle).CheckResult();
        }

        public unsafe int AddPolygon(float directOcclusion, float reverbOcclusion, bool doubleSided, Span<Vector> vertices)
        {
            int len = vertices.Length, polyIndex;

            if (len == 0)
            {
                throw new ArgumentException("Vertices count is 0", nameof(vertices));
            }

            fixed (Vector* vert = vertices)
            {
                library.Geometry_AddPolygon(Handle, directOcclusion, reverbOcclusion, doubleSided, len, vert, out polyIndex).CheckResult();
            }
            return polyIndex;
        }

        public int PolygonCount
        {
            get
            {
                var res = library.Geometry_GetNumPolygons(Handle, out int value);
                FmodSystem.CheckResult(res);
                return value;
            }
        }

        public void GetMaxPolygons(out int maxPolygons, out int maxVertices)
        {
            var res = library.Geometry_GetMaxPolygons(Handle, out maxPolygons, out maxVertices);
            FmodSystem.CheckResult(res);
        }

        public int GetPolygonVertexCount(int index)
        {
            var res = library.Geometry_GetPolygonNumVertices(Handle, index, out int count);
            FmodSystem.CheckResult(res);
            return count;
        }

        public void SetPolygonVertex(int index, int vertexIndex, ref Vector vertex)
        {
            var res = library.Geometry_SetPolygonVertex(Handle, index, vertexIndex, ref vertex);
            FmodSystem.CheckResult(res);
        }

        public void GetPolygonVertex(int index, int vertexIndex, out Vector vertex)
        {
            vertex = default;
            var res = library.Geometry_GetPolygonVertex(Handle, index, vertexIndex, out vertex);
            FmodSystem.CheckResult(res);
        }

        public void SetPolygonAttributes(int index, float directOcclusion, float reverbOcclusion, bool doubleSided)
        {
            var res = library.Geometry_SetPolygonAttributes(Handle, index, directOcclusion, reverbOcclusion, doubleSided);
            FmodSystem.CheckResult(res);
        }

        public void GetPolygonAttributes(int index, out float directOcclusion, out float reverbOcclusion, out bool doubleSided)
        {
            var res = library.Geometry_GetPolygonAttributes(Handle, index, out directOcclusion, out reverbOcclusion, out doubleSided);
            FmodSystem.CheckResult(res);
        }

        public bool Active
        {
            get
            {
                var res = library.Geometry_GetActive(Handle, out bool value);
                FmodSystem.CheckResult(res);
                return value;
            }

            set
            {
                var res = library.Geometry_SetActive(Handle, value);
                FmodSystem.CheckResult(res);
            }
        }

        public void SetRotation(ref Vector forward, ref Vector up)
        {
            library.Geometry_SetRotation(Handle, ref forward, ref up).CheckResult();
        }

        public void GetRotation(out Vector forward, out Vector up)
        {
            forward = default;
            up = default;
            library.Geometry_GetRotation(Handle, out forward, out up).CheckResult();
        }

        public void SetPosition(ref Vector position)
        {
            library.Geometry_SetPosition(Handle, ref position).CheckResult();
        }

        public void GetPosition(out Vector position)
        {
            position = default;
            library.Geometry_GetPosition(Handle, out position).CheckResult();
        }

        public void SetScale(ref Vector scale)
        {
            library.Geometry_SetScale(Handle, ref scale).CheckResult();
        }

        public void GetScale(out Vector scale)
        {
            scale = default;
            library.Geometry_GetScale(Handle, out scale).CheckResult();
        }

        /// <summary>
        /// Returns false if the given buffer is not large enough, throws on API Errors
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public unsafe bool TrySave(Span<byte> buffer, out int RequiredSize)
        {
            RequiredSize = 0;
            library.Geometry_Save(Handle, null, out RequiredSize).CheckResult();

            if (buffer.Length < RequiredSize)
                return false;

            fixed(byte* ptr = buffer)
            {
                library.Geometry_Save(Handle, ptr, out RequiredSize).CheckResult();
            }

            return true;
        }

        public bool TrySave(Span<byte> buffer)
        {
            return TrySave(buffer, out _);
        }

        public IntPtr UserData
        {
            get
            {
                library.Geometry_GetUserData(Handle, out IntPtr value).CheckResult();
                return value;
            }

            set
            {
                library.Geometry_SetUserData(Handle, value).CheckResult();
            }
        }
    }
}
