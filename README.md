# hackernews
 
## How to run

### Pre-requisites
*hackernews* was built in .net core 2.1, therefore .net core will need to be install, version 2.1 or higher which can be [found here](https://dotnet.microsoft.com/download/dotnet-core/2.1)

### Building and running the code
- Navigate to the solution folder, open a command line and run `dotnet build`
- To run the code from the framwork, run `dotnet run -- --posts <p>` (the first `--` escapes args sent to the dotnet runtime)
- Alternatively, build an executable by running `dotnet publish -c Release -r <rid>`, then navigate to build directory and run `hackernews --posts <p>` (RIDs can be found [here](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog#rid-graph)

## Libraries used
- [System.CommandLine](https://github.com/dotnet/command-line-api) - To help handle CLI parameters
- [NewtonSoft.Json](https://github.com/JamesNK/Newtonsoft.Json) - To handle JSON serialization

## Other considerations
- Unit Tests aren't exhaustive, but cover the general functionality of the code and (hopefully) illustrate logic behind a more complete coverage.
