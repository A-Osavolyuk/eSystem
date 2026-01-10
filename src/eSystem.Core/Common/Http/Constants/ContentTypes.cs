namespace eSystem.Core.Common.Http.Constants;

public class ContentTypes
{
    public static class Application
    {
        public const string Json = "application/json";
        public const string XwwwFormUrlEncoded = "application/x-www-form-urlencoded";
        public const string Xml = "application/xml";
        public const string Pdf = "application/pdf";
        public const string OctetStream = "application/octet-stream";
        public const string JavaScript = "application/javascript";
        public const string MsgPack = "application/msgpack";
    }

    public static class Multipart
    {
        public const string FormData = "multipart/form-data";
        public const string Mixed = "multipart/mixed";
        public const string Alternative = "multipart/alternative";
    }

    public static class Text
    {
        public const string Plain = "text/plain";
        public const string Html = "text/html";
        public const string Css = "text/css";
        public const string Csv = "text/csv";
        public const string Xml = "text/xml";
    }

    public static class Image
    {
        public const string Jpeg = "image/jpeg";
        public const string Png = "image/png";
        public const string Gif = "image/gif";
        public const string Svg = "image/svg+xml";
        public const string WebP = "image/webp";
    }

    public static class Audio
    {
        public const string Mp3 = "audio/mpeg";
        public const string Wav = "audio/wav";
        public const string Ogg = "audio/ogg";
    }

    public static class Video
    {
        public const string Mp4 = "video/mp4";
        public const string WebM = "video/webm";
        public const string Ogg = "video/ogg";
    }
}
