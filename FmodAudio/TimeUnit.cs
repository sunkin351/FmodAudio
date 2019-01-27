using System;

namespace FmodAudio
{
    [Flags]
    public enum TimeUnit : uint
    {
        MS = 0x00000001,  /* Milliseconds. */
        PCM = 0x00000002,  /* PCM Samples, related to milliseconds * samplerate / 1000. */
        PCMBytes = 0x00000004,  /* Bytes, related to PCM samples * channels * datawidth (ie 16bit = 2 bytes). */
        RAWBytes = 0x00000008,  /* Raw file bytes of (compressed) sound data (does not include headers).  Only used by Sound::getLength and Channel::getPosition. */
        PCMFraction = 0x00000010,  /* Fractions of 1 PCM sample.  Unsigned int range 0 to 0xFFFFFFFF.  Used for sub-sample granularity for DSP purposes. */
        ModOrder = 0x00000100,  /* MOD/S3M/XM/IT.  Order in a sequenced module format.  Use Sound::getFormat to determine the format. */
        ModRow = 0x00000200,  /* MOD/S3M/XM/IT.  Current row in a sequenced module format.  Cannot use with Channel::setPosition.  Sound::getLength will return the number if rows in the currently playing or seeked to pattern. */
        ModPattern = 0x00000400,  /* MOD/S3M/XM/IT.  Current pattern in a sequenced module format.  Cannot use with Channel::setPosition.  Sound::getLength will return the number of patterns in the song and Channel::getPosition will return the currently playing pattern. */
    }
}
