REM Must be run from within a Visual Studio Developer Command Prompt

nuget restore Plugin.AudioRecorder.sln
nuget restore Plugin.AudioRecorder.NuGet\Plugin.AudioRecorder.NuGet.nuproj

msbuild Plugin.AudioRecorder.sln /property:Configuration=Release

msbuild Plugin.AudioRecorder.NuGet\Plugin.AudioRecorder.NuGet.nuproj /property:Configuration=Release

copy Plugin.AudioRecorder.NuGet\bin\Release\*.nupkg .