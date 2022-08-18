using pp;
using Serilog;

Log.Logger = new LoggerConfiguration()
.MinimumLevel.Debug()
.WriteTo.Console()
.WriteTo.File(Path.Combine("Logs", "pp_.log"), rollingInterval: RollingInterval.Day)
.CreateLogger();

Log.Information("Hello from python process supervisor   ver 0.0");

MainProcess mp = new MainProcess();
mp.MainCircuit();

