namespace WebApp.Net.Service;

public class AppService
{
    public IFormFile CreateMockFormFile(string filePath)
    {
        var fileStream = new FileStream(filePath, FileMode.Open);
        var fileName = Path.GetFileName(filePath);
        var contentLength = fileStream.Length;

        var formFile = new FormFile(fileStream, 0, contentLength, null, fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/octet-stream" // Или любой другой тип контента, соответствующий вашему файлу
        };

        return formFile;
    }
}