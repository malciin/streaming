namespace Streaming.Api.Requests.Video
{
    public class UploadVideoRequest
    {
        public string UploadToken { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
