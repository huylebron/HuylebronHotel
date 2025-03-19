namespace HotelProject.Domain.Exception ;

public static class ModelException
{
    public class ModelNotValidException : BadRequestException
    {
        public ModelNotValidException(string message)
            : base(message)
        {

        }
      
    }
}