using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class RegisterDto
{
    [Required]  //Data Annotations/properties
    [MaxLength(100)]
    public required string Username { get; set; }
    
     [Required]
    public required string Password { get; set; }
}
