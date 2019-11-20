using McMaster.Extensions.CommandLineUtils;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace hackernews
{
    class Program
    {
        static IHackerNewsScraper scraper;

        static int Main(string[] args)
        {
            scraper = new HackerNewsScraper(new Http());


            var rootCommand = new RootCommand
            {
                new Option(new[] {"-p", "--posts"}, "How many posts to print. A positive integer <= 100.")
                {
                    Argument = new Argument<int>()
                }
            };

            rootCommand.Description = "Hacker News Scraper Test";
            rootCommand.Handler = CommandHandler.Create<int>(async (posts) =>
            {
                if (posts <= 0 || posts > 100)
                {
                    Console.WriteLine("Number of posts should be a positive integer <= 100");
                    return;
                }
                var result = await scraper.GetPostsJson(posts);

                Console.WriteLine($"Result: {result}");
            });

            return rootCommand.InvokeAsync(args).Result;
        }
    }
}
