using System;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace hackernews
{
    class Program
    {
        static IHackerNewsScraper scraper;

        static int Main(string[] args)
        {
            // Initialize our hackernews scraper with our IHttp implementation
            scraper = new HackerNewsScraper(new Http());

            // Set up our CLI commands
            var rootCommand = new RootCommand
            {
                // Create option for -p|--posts
                new Option(new[] {"-p", "--posts"}, "How many posts to print. A positive integer <= 100.")
                {
                    Argument = new Argument<int>()
                }
            };
            rootCommand.Description = "Hacker News Scraper Test";

            // Create the handler that will parse the CLI args and call our scraper
            rootCommand.Handler = CommandHandler.Create<int>(async (posts) =>
            {
                if (posts <= 0 || posts > 100)
                {
                    Console.WriteLine("Number of posts should be a positive integer <= 100");
                    return;
                }
                var result = await scraper.GetPostsJson(posts);

                Console.WriteLine(result);
            });

            // Invoke the CLI handler with the args passed in to Main
            return rootCommand.InvokeAsync(args).Result;
        }
    }
}
