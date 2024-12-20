﻿using System.ComponentModel.DataAnnotations;

namespace API.DTO
{
    public class AddressDTO
    {

        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string City { get; set; }
        public string State { get; set; }

        [Required]
        public string ZipCode { get; set; }

    }
}
