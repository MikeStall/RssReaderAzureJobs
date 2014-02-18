using System.ServiceModel.Syndication;
using System.IO;
using System.Xml;
using Microsoft.WindowsAzure.Jobs;

// Custom model binder to bind between a SyndicationFeed and a stream.
public class SyndicationFeedBinder : ICloudBlobStreamBinder<SyndicationFeed>
{
    public SyndicationFeed ReadFromStream(Stream input)
    {
        throw new System.NotImplementedException();
    }

    public void WriteToStream(SyndicationFeed result, Stream output)
    {
        using (var writer = XmlWriter.Create(output))
        {
            result.SaveAsRss20(writer);
        }
    }
}
