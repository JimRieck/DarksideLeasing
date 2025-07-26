using System.Diagnostics;

RunPowerShellScript(@"C:\Code\MyStuff\DarksideLeasing\Run-Darkside-Leasing.ps1");
RunPowerShellScript(@"C:\Code\MyStuff\DarksideLeasing\Logger\run-darkside-logging.ps1");

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Darkside_LeasingCalc_UI>("darkside-leasingcalc-ui");

builder.AddAzureFunctionsProject<Projects.DarkSideLeasing_App_Api>("darksideleasing-app-api");

builder.Build().Run();

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