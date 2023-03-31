namespace RavenDbFinalTest.Models
{
    public class ImageDocument
    {
            public string Id { get; set; }
            public byte[] ImageData { get; set; }
            public string ContentType { get; set; }
        }

}
