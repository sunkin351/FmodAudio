Contributing to FmodAudio
=========================
These guidelines are a WIP, and are incomplete.

I thank you for taking interest in this project. Your assistance is very much welcome.

There are several ways to contribute to this library. Not the least of which is submitting bug reports and feature requests,
as well as sending PR's (Pull Requests) implementing these in code. All contributions will go through github.

# Bug Reports and Feature requests
To submit a bug report or feature request, Open an issue on this github repository. I will reply to your issue as soon as possible.
Feature requests can be for an API change that you believe would make this library more elegant/easy to use.

# Guidelines for Pull Requests
The goal of this project is to provide an elegant and efficient means to use the Fmod Low Level API.
That said, here's a few guidelines to help achieve this:

  * All PR's are to be made against the `dev` branch of this repository.
  * Class Methods should be 1 to 1 with the native API where possible, marshal where necessary.
  * Methods/Functions used by multiple classes go in the `FmodAudio.Helpers` internal class. (e.g. string marshalling routines)
  * Structures should be 1 to 1 with the native structures for ease of interop. (`IntPtr` for most Pointers, Fmod bools are `int`, etc.) Wrap them in Classes where needed. (Examples: `FmodAudio.Dsp.ParameterDescription` and subclasses, `FmodAudio.Dsp.DspDescription`)
