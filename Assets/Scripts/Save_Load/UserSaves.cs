using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Локальные сохранения на сцене
/// </summary>
public class UserSaves : MonoBehaviour
{
     public string Username { get; set; }
     public int Userlevel { get; set; }


    private void Update()
    {
        Debug.Log(Username);
    }
}
