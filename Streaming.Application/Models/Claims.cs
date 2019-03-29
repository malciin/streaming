namespace Streaming.Application.Models
{
    public static class Claims
    {
        public const string ClaimsNamespace = "http://streaming.com/claims";

        public const string CanEditOwnVideo = "canEditOwnVideo";
        public const string CanEditAnyVideo = "canEditAnyVideo";
        public const string CanUploadVideo = "canUploadVideo";
        public const string CanDeleteVideo = "canDeleteVideo";
        public const string CanAccessAuth0Api = "canAccessAuth0Api";
        public const string CanStream = "canStream";
    }
}
