using DatingAPI.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace DatingAPI.Helpers
{
  public class LogUserActivity : IAsyncActionFilter
  {
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
      var resultContext = await next();
      var userId = resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
      var userServices = resultContext.HttpContext.RequestServices.GetService<IUserServices>();
      try
      {
        await userServices.UpdateActivity(userId);
      }
      catch (System.Exception ex)
      {

        throw ex;
      }
      
    }
  }
}
