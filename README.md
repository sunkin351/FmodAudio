# FmodAudio
# 1.0.0 has officially been released on NuGet! Feel free to try it out!

Welcome to FmodAudio, a C# Wrapper for the FMOD audio library.

This wrapper was written for .NET Core 2.1 and above.

This wrapper is now downloadable as a nuget package. https://www.nuget.org/packages/FmodAudio

In order to use this wrapper, you will need to download the FMOD native binaries from https://www.fmod.com/download

If you find a bug, or have a feature request, please file an issue on github, and I'll review it as soon as possible.
Alternatively you can now join a Discord server for FmodAudio, and talk with me directly. https://Discord.gg/cNcbHF5

If you wish to contribute, see `CONTRIBUTING.md`

How to use FmodAudio
--------------------
First off, this wrapper allows you to choose a location to look for the native library through the `Fmod.SetLibraryLocation()` method.
Use this if you have a non-standard place you put the library.

Now you have two options for calling into FMOD. The first and recommended way is to use the Object Oriented wrapper classes (`FmodSystem`, `Sound`, `Channel`, etc.) beginning with calling `Fmod.CreateSystem()`.

 Alternatively, you have direct access to the binding through the `Fmod.Library` property for situations where FMOD gives you a OOP handle directly via callbacks. All FMOD API methods are exposed through the class this property returns.

 All API methods and structures are documented in https://www.fmod.com/resources/documentation-api?version=2.0&page=core-api.html

 Example usage found in the `Examples` directory of this repository.
