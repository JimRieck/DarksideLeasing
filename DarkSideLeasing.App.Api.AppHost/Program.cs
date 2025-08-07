using System.Diagnostics;

// Execute PowerShell scripts to set up the environment or perform pre-run tasks
RunPowerShellScript(@"C:\Code\MyStuff\DarksideLeasing\Run-Darkside-Leasing.ps1");
RunPowerShellScript(@"C:\Code\MyStuff\DarksideLeasing\Logger\run-darkside-logging.ps1");

// Create a distributed application builder to configure and initialize the application
var builder = DistributedApplication.CreateBuilder(args);

// Add the Blazor UI project to the application
builder.AddProject<Projects.Darkside_LeasingCalc_UI>("darkside-leasingcalc-ui");

// Add the Azure Functions project to the application
builder.AddAzureFunctionsProject<Projects.DarkSideLeasing_App_Api>("darksideleasing-app-api");

// Build and run the application
builder.Build().Run();

// Method to execute a PowerShell script
static void RunPowerShellScript(string scriptPath)
{
    // Configure the process to run PowerShell with the specified script
    var psi = new ProcessStartInfo
    {
        FileName = "powershell.exe", // Specify PowerShell executable
        Arguments = $"-ExecutionPolicy Bypass -File \"{scriptPath}\"", // Set execution policy and script path
        RedirectStandardOutput = true, // Redirect standard output for capturing script output
        RedirectStandardError = true, // Redirect standard error for capturing errors
        UseShellExecute = false, // Do not use shell execution
        CreateNoWindow = true // Run without creating a visible window
    };

    // Start the PowerShell process
    using var process = Process.Start(psi);
    if (process == null)
    {
        // Log an error if the process fails to start
        Console.WriteLine("Failed to start PowerShell process.");
        return;
    }

    // Capture the output and error streams from the PowerShell script
    string output = process.StandardOutput.ReadToEnd();
    string error = process.StandardError.ReadToEnd();

    // Wait for the process to complete
    process.WaitForExit();

    // Log the output and errors from the script execution
    Console.WriteLine("PowerShell Output:\n" + output);
    if (!string.IsNullOrEmpty(error))
        Console.WriteLine("PowerShell Error:\n" + error);
}