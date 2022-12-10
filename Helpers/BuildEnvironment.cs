using CliWrap;

namespace WAL.Helpers;

public class BuildEnvironment
{
    public string InputFile;
    public string CppFilename => Path.GetFileNameWithoutExtension(InputFile) + ".cpp";
    public string ProjectName => Path.GetFileNameWithoutExtension(InputFile);
    public string CppFile => Path.Join(WorkingDirectory, CppFilename);
    public string CmakeFile => Path.Join(WorkingDirectory, "CMakeLists.txt");
    public string WorkingDirectory => "External/BuildEnvironment";

    public string BuildDirectory =>
        Path.GetFullPath(Path.Join(Path.GetDirectoryName(InputFile), "build")).Replace("\\", "\\\\");

    public BuildEnvironment(string inputFile)
    {
        InputFile = inputFile;
        if (!Directory.Exists(WorkingDirectory))
            throw new Exception();

        File.WriteAllText(CppFile, "");
        File.WriteAllText(CmakeFile, @$"include(FetchContent)
cmake_minimum_required(VERSION 3.23)
project({ProjectName})
#set(CMAKE_MSVC_RUNTIME_LIBRARY ""MultiThreaded$<$<CONFIG:Debug>:Debug>"")
set(CMAKE_CXX_STANDARD 20)
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY_DEBUG {BuildDirectory})
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY_RELEASE {BuildDirectory})

include(FetchContent)
FetchContent_Declare(SFML
    GIT_REPOSITORY https://github.com/SFML/SFML.git
    GIT_TAG 2.6.x)
FetchContent_MakeAvailable(SFML)

add_executable({ProjectName} {Path.GetFileName(CppFile)})
target_link_libraries({ProjectName} PRIVATE sfml-graphics sfml-window sfml-audio sfml-system)");
    }

    public void SaveCppOutput()
    {
        var outputText = File.ReadAllText(CppFile);
        File.WriteAllText(Path.Join(Path.GetDirectoryName(InputFile), CppFilename), outputText);
    }

    public void Build()
    {
        Cli.Wrap("cmake")
            .WithArguments("-S. -Bbuild")
            .WithWorkingDirectory(WorkingDirectory)
            .WithStandardOutputPipe(PipeTarget.ToDelegate(Console.WriteLine))
            .ExecuteAsync().GetAwaiter().GetResult();
        Cli.Wrap("cmake")
            .WithArguments("--build build")
            .WithWorkingDirectory(WorkingDirectory)
            .WithStandardOutputPipe(PipeTarget.ToDelegate(Console.WriteLine))
            .ExecuteAsync().GetAwaiter().GetResult();
    }
}