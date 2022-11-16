using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Security.Claims;

namespace GrpSample.API.Identity
{
    public class DomainUserLoginProvider : ILoginProvider
    {
        private readonly string _domain;
        public DomainUserLoginProvider(string domain)
        {
            _domain = domain;
        }

        //public bool ValidateCredentials(string userName, string password)
        //{
        //    using (var pc = new PrincipalContext(ContextType.Domain, _domain))
        //    {
        //        bool isValid = pc.ValidateCredentials(userName, password);

        //        return isValid;
        //    }
        //}

        ///IsAccountLockedOut check 
         public bool ValidateCredentials(string userName, string password)
        {
            using (var pc = new PrincipalContext(ContextType.Domain, _domain))
            {
                bool isValid = pc.ValidateCredentials(userName, password);
                if (isValid)
                {
                    var user = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, userName);
                    if (user.AccountExpirationDate.HasValue || user.IsAccountLockedOut())
                    {
                        if (user.AccountExpirationDate != null)
                        {
                            DateTime expireationdate = user.AccountExpirationDate.Value.ToLocalTime();
                        }

                        isValid = false;
                    }

                }
                return isValid;
            }
        }
        

        public string GetUserFullName(string adUserName)
        {
            string retVal = string.Empty;

            using (var pc = new PrincipalContext(ContextType.Domain, _domain))
            {
                var user = UserPrincipal.FindByIdentity(pc, adUserName);
                if (user != null)
                    retVal = user.DisplayName;
            }

            return retVal;
        }
    }
}