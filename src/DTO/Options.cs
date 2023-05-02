using CommandLine;
namespace GameListDB.DTO
{
    public class Options
    {
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }
    //  [Option('D', "database", Required = false, HelpText = "Specify a database")]
    //  public bool Database { get; set; }
        [Option('f', "force", Required = false, HelpText = "Force a Clean Action")]
        public bool Force { get; set; }
    }

}