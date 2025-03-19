namespace HotelProject.Domain.Exception ;

public class FileExceptionNotFoundException : NotFoundException
{
    public FileExceptionNotFoundException(string path)
        : base($"The file with the path {path} was not found.")
    {
      
    }
}