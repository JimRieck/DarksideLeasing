using System.Diagnostics;

RunPowerShellScript(@"C:\Code\MyStuff\DarksideLeasing\Logger\run-darkside-logging.ps1");

//RunDockerCompose();

var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureFunctionsProject<Projects.Darkside_Logger_API>("darkside-logger-api");

builder.Build().Run();

void RunDockerCompose()
{
    try
    {
        // First try the project directory
        var projectDir = Path.GetDirectoryName(AppContext.BaseDirectory);
        while (projectDir != null && !File.Exists(Path.Combine(projectDir, "Program.cs")))
        {
            projectDir = Path.GetDirectoryName(projectDir);
        }

        var dockerComposeFile = Path.Combine(projectDir ?? AppContext.BaseDirectory, "docker-compose.yml");
        
        if (!File.Exists(dockerComposeFile))
        {
            Console.Error.WriteLine($"Could not find docker-compose.yml at {dockerComposeFile}");
            return;
        }

        // Check if docker is installed
        try
        {
            var checkDocker = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = "--version",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var dockerProcess = Process.Start(checkDocker);
            dockerProcess?.WaitForExit();
            if (dockerProcess?.ExitCode != 0)
            {
                Console.Error.WriteLine("Docker is not installed or not running.");
                return;
            }
        }
        catch (Exception)
        {
            Console.Error.WriteLine("Docker is not installed or not in PATH.");
            return;
        }

        Console.WriteLine($"Running docker-compose from {dockerComposeFile}");
        var psi = new ProcessStartInfo
        {
            FileName = "docker-compose",
            Arguments = $"-f \"{dockerComposeFile}\" up -d --build",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = Path.GetDirectoryName(dockerComposeFile)
        };

        using (var process = Process.Start(psi))
        {
            if (process == null)
            {
                Console.Error.WriteLine("Failed to start docker-compose process");
                return;
            }

            process.OutputDataReceived += (sender, e) => 
            {
                if (!string.IsNullOrEmpty(e.Data))
                    Console.WriteLine($"Docker: {e.Data}");
            };
            process.ErrorDataReceived += (sender, e) => 
            {
                if (!string.IsNullOrEmpty(e.Data))
                    Console.Error.WriteLine($"Docker Error: {e.Data}");
            };
            
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                Console.Error.WriteLine($"Docker-compose exited with code {process.ExitCode}");
            }
            else
            {
                Console.WriteLine("Docker-compose started successfully");
            }
        }
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Error running docker-compose: {ex.Message}");
    }
}

static void RunPowerShellScript(string scriptPath)
{
    var psi = new ProcessStartInfo
    {
        FileName = "powershell.exe",
        Arguments = $"-ExecutionPolicy Bypass -File \"{scriptPath}\"",
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };

    using var process = Process.Start(psi);
    if (process == null)
    {
        Console.WriteLine("Failed to start PowerShell process.");
        return;
    }

    string output = process.StandardOutput.ReadToEnd();
    string error = process.StandardError.ReadToEnd();

    process.WaitForExit();

    Console.WriteLine("PowerShell Output:\n" + output);
    if (!string.IsNullOrEmpty(error))
        Console.WriteLine("PowerShell Error:\n" + error);
}
