﻿using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Web.Models.Person;

public class PersonWriteWithUserModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Address? Address { get; set; }
    public string EmailAddress { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public Role Role { get; set; }
}