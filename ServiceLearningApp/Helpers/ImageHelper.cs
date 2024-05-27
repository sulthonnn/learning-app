using ImageMagick;

namespace ServiceLearningApp.Helpers
{
    public static class ImageHelper
    {
        public static void SaveImage(string filePath, byte[] imageBytes)
        {
            using (var image = new MagickImage(imageBytes))
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                image.Write(fileStream);
            }
        }

        public static void Resize(Stream input, Stream output, int maxSize = 300)
        {
            using (var image = new MagickImage(input, new MagickReadSettings { Density = new Density(300, 300) }))
            {
                image.Format = MagickFormat.WebP;
                image.Resize(new MagickGeometry(maxSize));
                image.Write(output);
            }
        }

        public static string SaveThumbnail(string filePath, int maxSize = 300)
        {
            var outputFilePath = GetThumbnailPath(filePath);

            using (var image = new MagickImage(filePath))
            {
                image.Format = MagickFormat.WebP;
                image.Thumbnail(new MagickGeometry(maxSize));
                image.Write(outputFilePath);
                return outputFilePath;
            }
        }

        public static void CreateThumbnail(Stream input, Stream output, int maxSize = 300)
        {
            using (var image = new MagickImage(input, new MagickReadSettings { Density = new Density(300, 300) }))
            {
                image.Format = MagickFormat.WebP;
                image.Thumbnail(new MagickGeometry(maxSize));
                image.Write(output);
            }
        }

        public static void CreateThumbnail(byte[] input, Stream output, int maxSize = 300)
        {
            using (var image = new MagickImage(input, new MagickReadSettings { Density = new Density(300, 300) }))
            {
                image.Alpha(AlphaOption.Remove);
                image.Format = MagickFormat.WebP;
                image.Thumbnail(new MagickGeometry(maxSize));
                image.Write(output);
            }
        }

        public static byte[] CreateThumbnail(byte[] input, int maxSize = 300)
        {
            using (var image = new MagickImage(input, new MagickReadSettings { Density = new Density(300, 300) }))
            {
                image.Alpha(AlphaOption.Remove);
                image.Format = MagickFormat.WebP;
                image.Thumbnail(new MagickGeometry(maxSize));
                return image.ToByteArray();
            }
        }

        public static void CompressImage(string inputFilePath)
        {
            var optimizer = new ImageOptimizer();
            optimizer.Compress(inputFilePath);
        }

        public static string GetThumbnailPath(string inputFilePath)
        {
            var inputFolderPath = Path.GetDirectoryName(inputFilePath);
            var inputFileName = Path.GetFileNameWithoutExtension(inputFilePath);
            var outputFilePath = Path.Combine(inputFolderPath, inputFileName + "-thumbnail.webp");
            return outputFilePath;
        }
    }
}
