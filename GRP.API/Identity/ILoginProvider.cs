
using System.Threading.Tasks;

namespace GrpSample.API.Identity
{
    public interface ILoginProvider
    {
        //bool ValidateCredentials(string userName, string password, out ClaimsIdentity identity);
        bool ValidateCredentials(string userName, string password);
    }

}
