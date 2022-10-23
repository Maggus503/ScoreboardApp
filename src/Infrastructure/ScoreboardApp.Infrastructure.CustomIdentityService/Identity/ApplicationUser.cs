﻿using Microsoft.AspNetCore.Identity;
using System.Runtime.Serialization;

namespace ScoreboardApp.Infrastructure.CustomIdentityService.Identity
{
    public sealed class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [IgnoreDataMember]
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }
    }
}