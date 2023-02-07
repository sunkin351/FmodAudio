using System.Collections.Generic;

namespace FmodAudioSourceGenerator;

internal static class Helpers
{
    public static int SequenceHashCode<T>(this IEnumerable<T> values)
    {
        return values.SequenceHashCode(EqualityComparer<T>.Default);
    }

    public static int SequenceHashCode<T>(this IEnumerable<T> values, IEqualityComparer<T> comparer)
    {
        int code = 0;

        if (values is not null)
        {
            foreach (var value in values)
            {
                var valCode = comparer.GetHashCode(value);

                code = code * -1521134295 + valCode;
            }
        }

        return code;
    }
}