using Microsoft.AspNetCore.Identity;
using Vives.Security.Model;
using Vives.Services.Model.Enums;
using Vives.Services.Model;

namespace PeopleManager.RestApi.Services.Helpers
{
    public static class JwtAuthenticationHelpers
    {
        public static JwtAuthenticationResult LoginFailed()
        {
            return new JwtAuthenticationResult()
            {
                Messages = new List<ServiceMessage>()
                {
                    new ServiceMessage()
                    {
                        Code = "LoginFailed",
                        Message = "User/Password combination is incorrect.",
                        Type = ServiceMessageType.Error
                    }
                }
            };
        }

        public static JwtAuthenticationResult UserExists()
        {
            return new JwtAuthenticationResult()
            {
                Messages = new List<ServiceMessage>()
                {
                    new ServiceMessage()
                    {
                        Code = "UserExists",
                        Message = "This user already exists in the database",
                        Type = ServiceMessageType.Error
                    }
                }
            };
        }

        public static JwtAuthenticationResult JwtConfigurationError()
        {
            return new JwtAuthenticationResult()
            {
                Messages = new List<ServiceMessage>()
                {
                    new ServiceMessage()
                    {
                        Code = "JwtConfigurationError",
                        Message = "JWT Settings are not configured correctly",
                        Type = ServiceMessageType.Error
                    }
                }
            };
        }

        public static JwtAuthenticationResult RegisterError(IEnumerable<IdentityError> errors)
        {
            var jwtResult = new JwtAuthenticationResult();
            foreach (var error in errors)
            {
                jwtResult.Messages.Add(new ServiceMessage
                {
                    Code = error.Code,
                    Message = error.Description,
                    Type = ServiceMessageType.Error
                });
            }

            return jwtResult;
        }
    }
}
