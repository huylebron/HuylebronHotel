using HotelProject . Domain . Exception ;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;


namespace HotelProject . Api . Filters 
{

    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                List<string> errors = context.ModelState.Values.SelectMany(s => s.Errors.Select(x => x.ErrorMessage)).ToList();
                throw new ModelException . ModelNotValidException(JsonConvert.SerializeObject(errors));

            }
        }
    }
}
