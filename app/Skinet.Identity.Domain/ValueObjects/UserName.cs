using System.Collections.Generic;
using Skinet.Identity.Domain.Entities;

namespace Skinet.Identity.Domain.ValueObjects
{
    public class UserName : ValueObject
    {
        public string FirstName { get; }
        public string LastName { get; }

        private UserName(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return FirstName;
            yield return LastName;
        }

        public static Result<UserName> Create(string firstName, string lastName)
        {
            firstName = (firstName ?? string.Empty).Trim();
            lastName = (lastName ?? string.Empty).Trim();

            if (firstName.Length == 0)
            {
                return Result.Fail<UserName>("First name cannot be empty");
            }            
            if (firstName.Length > 100)
            {
                return Result.Fail<UserName>("First name is too long");
            }
            
            if (lastName.Length == 0)
            {
                return Result.Fail<UserName>("Last name cannot be empty");
            }            
            if (lastName.Length > 100)
            {
                return Result.Fail<UserName>("Last name is too long");
            }
            
            return Result.Ok(new UserName(firstName, lastName));
        }
    }
}