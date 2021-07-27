using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FmodAudio
{
    public sealed class BindingMismatchException : Exception
    {
        // TODO: Add Message
        public BindingMismatchException()
        {
        }

        public BindingMismatchException(string message) : base(message)
        {
        }
    }
}
