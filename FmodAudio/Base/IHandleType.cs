namespace FmodAudio.Base
{
    public interface IHandleType<T> where T: unmanaged
    {
        T Handle { get; }
    }
}
