using Microsoft.WindowsAzure.Jobs; // Microsoft.WindowsAzure.Jobs.Host
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;

namespace RssReader
{
    class Program
    {
        public static void Main()
        {
            // See the AzureJobsData and AzureJobsRuntime in the app.config
            var host = new JobHost();
            var m = typeof(Program).GetMethod("AggregateRss");            
            host.Call(m);

            Console.WriteLine(" aggregated to:");
            Console.WriteLine("https://{0}.blob.core.windows.net/blog/output.rss.xml", host.UserAccountName);
        }

        // RSS reader.
        // Aggregates to: http://<mystorage>.blob.core.windows.net/blog/output.rss.xml
        // Get blog roll from a table.
        public static void AggregateRss(
            [Table("blogroll")] IDictionary<Tuple<string, string>, BlogRollEntry> blogroll,
            [BlobOutput(@"blog/output.rss.xml")] out SyndicationFeed output
            )
        {
            // get blog roll form an azure table
            var urls = (from kv in blogroll select kv.Value.url).ToArray();

            List<SyndicationItem> items = new List<SyndicationItem>();
            foreach (string url in urls)
            {
                var reader = new XmlTextReader(url);
                var feed = SyndicationFeed.Load(reader);

                items.AddRange(feed.Items.Take(5));
            }
            var sorted = items.OrderBy(item => item.PublishDate);

            output = new SyndicationFeed("Status", "Status from SimpleBatch", null, sorted);
        }

        // Format for blog roll in the azure table
        public class BlogRollEntry
        {
            public string url { get; set; }
        }
    }
}
