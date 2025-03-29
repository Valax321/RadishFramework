#!/usr/bin/env bash

# Little helper script to build imgui shaders as an embedded resource
# We're using the official shader for SDL_GPU from the imgui git repo

SCRIPT_DIR=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )
cd "$SCRIPT_DIR" || exit

which -s glslang
if [ $? -ne 0 ]
then
    echo "Failed to find glslang"
    exit 1
fi

which -s shadercross
if [ $? -ne 0 ]
then
    echo "Failed to find shadercross"
    exit 1
fi

echo "Compiling SPIR-V"
glslang -V100 -o imgui.vert.spv imgui.vert.glsl || exit
glslang -V100 -o imgui.frag.spv imgui.frag.glsl || exit

echo "Transpiling DXIL"
shadercross imgui.vert.spv -s SPIRV -d DXIL -t vertex -o imgui.vert.bin || exit
shadercross imgui.frag.spv -s SPIRV -d DXIL -t fragment -o imgui.frag.bin || exit

echo "Transpiling MSL & metallib"
shadercross imgui.vert.spv -s SPIRV -d MSL -t vertex -o imgui.vert.metal || exit
xcrun -sdk macosx metal -o imgui.vert.ir -c imgui.vert.metal || exit
xcrun -sdk macosx metallib -o imgui.vert.metallib imgui.vert.ir || exit
rm imgui.vert.ir
rm imgui.vert.metal

shadercross imgui.frag.spv -s SPIRV -d MSL -t fragment -o imgui.frag.metal || exit
xcrun -sdk macosx metal -o imgui.frag.ir -c imgui.frag.metal || exit
xcrun -sdk macosx metallib -o imgui.frag.metallib imgui.frag.ir || exit
rm imgui.frag.ir
rm imgui.frag.metal
