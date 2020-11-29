using System.Threading.Tasks;

namespace DatingAPI.Hubs
{
  public interface IHubClient
  {
    Task SendMessage(string message);
  }
}