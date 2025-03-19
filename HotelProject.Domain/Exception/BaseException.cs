namespace HotelProject.Domain.Exception ;

public abstract class BaseException : System . Exception
{
    protected BaseException(string title, string message)
        : base(message) =>
        Title = title;

    public string Title { get; }    
    public int StatusCode { get; set; }
}