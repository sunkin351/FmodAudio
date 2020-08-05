#pragma warning disable IDE0059

using System;
using System.Numerics;

namespace FmodAudio
{
    using Base;

    public readonly struct Geometry : IDisposable
    {
        public static implicit operator Geometry(GeometryHandle handle)
        {
            return new Geometry(handle);
        }

        public static implicit operator GeometryHandle(Geometry geometry)
        {
            return geometry.Handle;
        }

        private static readonly FmodLibrary library = Fmod.Library;
        private readonly GeometryHandle Handle;

        internal Geometry(GeometryHandle handle)
        {
            Handle = handle;
        }

        public void Dispose()
        {
            Release();
        }

        public void Release()
        {
            library.Geometry_Release(Handle).CheckResult();
        }

        public unsafe int AddPolygon(float directOcclusion, float reverbOcclusion, bool doubleSided, ReadOnlySpan<Vector3> vertices)
        {
            int polyIndex;

            if (vertices.Length < 3)
            {
                throw new ArgumentException("Vertex count is less than 3", nameof(vertices));
            }

            library.Geometry_AddPolygon(Handle, directOcclusion, reverbOcclusion, doubleSided, vertices, &polyIndex).CheckResult();
            return polyIndex;
        }

        public unsafe int PolygonCount
        {
            get
            {
                int value;
                library.Geometry_GetNumPolygons(Handle, &value).CheckResult();
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

        public void GetPolygonAttributes(int index, out float directOcclusion, out float reverbOcclusion, out FmodBool doubleSided)
        {
            library.Geometry_GetPolygonAttributes(Handle, index, out directOcclusion, out reverbOcclusion, out doubleSided).CheckResult();
        }

        public bool Active
        {
            get
            {
                library.Geometry_GetActive(Handle, out FmodBool value).CheckResult();
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
        /// Saves the Geometry data to memory.
        /// Returns false if the given buffer is not large enough, throws on API Errors
        /// </summary>
        /// <param name="buffer">Buffer to save to</param>
        /// <param name="RequiredSize">Total bytes required to save</param>
        /// <returns>Whether the operation succeeded</returns>
        public unsafe bool TrySave(Span<byte> buffer, out int RequiredSize)
        {
            RequiredSize = 0;

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

        /// <summary>
        /// Saves the Geometry data to memory.
        /// Returns false if the given buffer is not large enough, throws on API Errors
        /// </summary>
        /// <param name="buffer">Buffer to save to</param>
        /// <returns>Whether the operation succeeded</returns>
        public bool TrySave(Span<byte> buffer)
        {
            return TrySave(buffer, out _);
        }

        /// <summary>
        /// Saves the Geometry data to memory.
        /// </summary>
        /// <returns>
        /// Byte array containing the geometry data
        /// </returns>
        public unsafe byte[] Save()
        {
            int required = 0;

            library.Geometry_Save(Handle, null, &required).CheckResult();

            byte[] data = new byte[required];

            fixed (byte* pData = data)
            {
                library.Geometry_Save(Handle, pData, &required).CheckResult();
            }

            return data;
        }

        internal unsafe IntPtr UserData
        {
            get
            {
                IntPtr value;
                library.Geometry_GetUserData(Handle, &value).CheckResult();
                return value;
            }

            set
            {
                library.Geometry_SetUserData(Handle, value).CheckResult();
            }
        }
    }
}
