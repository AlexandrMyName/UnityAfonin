using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Локальные сохранения на сцене
/// </summary>
public class UserSaves
{
    public string UserName;
    public string UserMoney;
    public string UserLevel;
     

    public UserSaves (string UserName, string UserMoney, string UserLevel)
    {
        this.UserName = UserName;
        this.UserMoney = UserMoney;
        this.UserLevel = UserLevel;
    }

   
}
