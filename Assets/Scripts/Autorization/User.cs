using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User 
{
    public string name;
    public string password;
    public string email;
    public string dataSignIN;

    public User(string name, string password, string email, string dataSignIN)
    {
        this.name = name;
        this.password = password;
        this.email = email;
        this.dataSignIN = dataSignIN;
    }
}
