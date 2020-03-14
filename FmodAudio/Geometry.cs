#pragma warning disable IDE0059

using System;
using System.Numerics;

namespace FmodAudio
{
    using Interop;
    public class Geometry : HandleBase
    {
        private readonly NativeLibrary library;

        internal Geometry(IntPtr handle, NativeLibrary lib) : base(handle)
        {
            library = lib;
        }

        protected override void ReleaseImpl()
        {
            library.Geometry_Release(Handle).CheckResult();
        }

        public unsafe int AddPolygon(float directOcclusion, float reverbOcclusion, bool doubleSided, ReadOnlySpan<Vector3> vertices)
        {
            int polyIndex;

            if (vertices.IsEmpty)
            {
                throw new ArgumentException("Vertex count is 0", nameof(vertices));
            }

            library.Geometry_AddPolygon(Handle, directOcclusion, reverbOcclusion, doubleSided, vertices, &polyIndex).CheckResult();
            return polyIndex;
        }

        public int PolygonCount
        {
            get
            {
                library.Geometry_GetNumPolygons(Handle, out int value).CheckResult();
                return value;
            }
        }

        public void GetMaxPolygons(out int maxPolygons, out int maxVertices)
        {
            library.Geometry_GetMaxPolygons(Handle, out maxPolygons, out maxVertices).CheckResult();
        }

        public unsafe int GetPolygonVertexCount(int index)
        {
            int count;
            library.Geometry_GetPolygonNumVertices(Handle, index, &count).CheckResult();
            return count;
        }

        public void SetPolygonVertex(int index, int vertexIndex, in Vector3 vertex)
        {
            library.Geometry_SetPolygonVertex(Handle, index, vertexIndex, in vertex).CheckResult();
        }

        public void GetPolygonVertex(int index, int vertexIndex, out Vector3 vertex)
        {
            library.Geometry_GetPolygonVertex(Handle, index, vertexIndex, out vertex).CheckResult();
        }

        public void SetPolygonAttributes(int index, float directOcclusion, float reverbOcclusion, bool doubleSided)
        {
            library.Geometry_SetPolygonAttributes(Handle, index, directOcclusion, reverbOcclusion, doubleSided).CheckResult();
        }

        public void GetPolygonAttributes(int index, out float directOcclusion, out float reverbOcclusion, out bool doubleSided)
        {
            library.Geometry_GetPolygonAttributes(Handle, index, out directOcclusion, out reverbOcclusion, out doubleSided).CheckResult();
        }

        public bool Active
        {
            get
            {
                library.Geometry_GetActive(Handle, out bool value).CheckResult();
                return value;
            }

            set
            {
                library.Geometry_SetActive(Handle, value).CheckResult();
            }
        }

        public void SetRotation(in Vector3 forward, in Vector3 up)
        {
            library.Geometry_SetRotation(Handle, in forward, in up).CheckResult();
        }

        public void GetRotation(out Vector3 forward, out Vector3 up)
        {
            library.Geometry_GetRotation(Handle, out forward, out up).CheckResult();
        }

        public void SetPosition(in Vector3 position)
        {
            library.Geometry_SetPosition(Handle, in position).CheckResult();
        }

        public void GetPosition(out Vector3 position)
        {
            library.Geometry_GetPosition(Handle, out position).CheckResult();
        }

        public void SetScale(in Vector3 scale)
        {
            library.Geometry_SetScale(Handle, in scale).CheckResult();
        }

        public void GetScale(out Vector3 scale)
        {
            library.Geometry_GetScale(Handle, out scale).CheckResult();
        }

        /// <summary>
        /// Returns false if the given buffer is not large enough, throws on API Errors
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public unsafe bool TrySave(Span<byte> buffer, out int RequiredSize)
        {
            fixed (int* pRequired = &RequiredSize)
            {
                library.Geometry_Save(Handle, null, pRequired).CheckResult();

                if (buffer.Length < *pRequired)
                    return false;

                fixed (byte* ptr = buffer)
                {
                    library.Geometry_Save(Handle, ptr, pRequired).CheckResult();
                    return true;
                }
            }
        }

        public bool TrySave(Span<byte> buffer)
        {
            return TrySave(buffer, out _);
        }

        [Obsolete]
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
