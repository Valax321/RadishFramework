# Radish Framework
A collection of libraries and utilities for building games on the .NET platform.

It is inspired loosely by XNA and FNA, but with some opt-in higher level components included and more modern format support.

## Features
* Written with realtime application needs in mind (e.g. low/no garbage allocation counts where possible).
* Resource loading system with extensible file sources (directories, package files etc.) and async/await support.
* Platform backend based on SDL3 via SDL3-CS
* Rendering based on SDL_GPU
* Integrated Dear ImGui support (optional) via ImGui.NET
* Full support for NativeAOT and assembly trimming

## License
Radish Framework is available under the MIT license. Keep in mind that this is primarily developed as an internal set of tools, so support/bug fixes/API stability should not necessarily be expected.
