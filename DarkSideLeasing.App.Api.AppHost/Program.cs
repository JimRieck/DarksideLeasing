var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Darkside_LeasingCalc_UI>("darkside-leasingcalc-ui");

builder.AddAzureFunctionsProject<Projects.DarkSideLeasing_App_Api>("darksideleasing-app-api");

builder.Build().Run();
