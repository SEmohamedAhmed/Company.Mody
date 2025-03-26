namespace Company.Mody.PL.Helper
{
    public static class FileManuplation
    {
        public static string Upload(IFormFile file, string folderName)
        {
            
            // get folder path to save the file
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\assets\\{folderName}");
            
            // get unique file name
            string fileName = $"{Guid.NewGuid()}{file.FileName}";

            // get the file path
            string filePath = Path.Combine(folderPath, fileName);


            // create file stream
            using var fileStream = new FileStream(filePath, FileMode.Create);

            file.CopyTo(fileStream);


            return fileName;
        }


        public static void Delete(string fileName, string folderName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\assets\\{folderName}\\{fileName}");

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

        }


        public static bool IsImage(IFormFile file)
        {
            string[] permittedMimeTypes = { "image/jpeg", "image/png", "image/gif", "image/bmp" };

            var contentType = file.ContentType.ToLower();
            if (!permittedMimeTypes.Contains(contentType))
            {
                return false;
            }
            return true;
        }
    }
}
