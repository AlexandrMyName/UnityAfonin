using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Firebase.Auth;
using Firebase.Database;
using Firebase;
using System;
using UnityEngine.SceneManagement;

public class Autorization_Game : MonoBehaviour
{
    [SerializeField] private GameObject button_All;
    [SerializeField] private GameObject button_Register;
    [SerializeField] private GameObject button_SignIN;
    [SerializeField] private GameObject button_SignOUT;

    private DatabaseReference db;
    private FirebaseAuth autorization;
    [SerializeField] private bool isSign_IN;

    [SerializeField] private string[] urls;
    [SerializeField] private bool isIthernet;

    DataSnapshot dataSnapshot;

    [Obsolete]
    void Start()
    {
        StartCoroutine(Check_IthernetConection(result =>
        {
            db = FirebaseDatabase.DefaultInstance.RootReference;
            autorization = FirebaseAuth.DefaultInstance;

            isIthernet = result;
            if(result)
            {
                Debug.Log("Подключено!");
                autorization.StateChanged += Check_CurentUser;
            }
            else
            {
                Debug.Log("Сбой подключения!");
                autorization.StateChanged -= Check_CurentUser;
            }
        }));
    }

    [Obsolete]
    private IEnumerator Check_IthernetConection(Action <bool> callBack)
    {
        foreach(string url in urls)
        {
            UnityWebRequest web = UnityWebRequest.Get(url);

            yield return web.SendWebRequest();

            if(web.isNetworkError != true)
            {
                callBack(true);
                yield break;
            }
            callBack(false);
        }
    }

    private void Check_CurentUser(object slender,EventArgs e)
    {
        if(autorization.CurrentUser != null)
        {
            isSign_IN = true;
            Debug.Log("Добро пожаловать : " + autorization.CurrentUser.Email);
        }
        else
        {
            isSign_IN = false;
            Debug.Log("Сбой авторизации..");
        }
    }
    [Obsolete]
    private IEnumerator Check_Email_Registr(string email,string password, Action <bool> callBack)
    {
        var auth = autorization.CreateUserWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(predicate: () => auth.IsCompleted);

        if(auth.Exception != null)
        {
            Debug.Log($"Ошибка АУНТИФИКАЦИИ: " + auth.Exception);
        }
        else
        {
            Debug.Log($"АУНТИФИКАЦИЯ ВЫПОЛНЕНА:" + autorization.CurrentUser.Email);
        }

        if(autorization.CurrentUser != null)
        {
            callBack(true);
            yield break;
        }
        callBack(false);
    }

    [Obsolete]
    private IEnumerator Check_Email_SignIN(string email, string password, Action<bool> callBack)
    {
        var auth = autorization.SignInWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(predicate: () => auth.IsCompleted);

        if (auth.Exception != null)
        {
            Debug.Log($"Ошибка АУНТИФИКАЦИИ: " + auth.Exception);
        }
        else
        {
            Debug.Log($"АУНТИФИКАЦИЯ ВЫПОЛНЕНА:" + autorization.CurrentUser.Email);
        }

        if (autorization.CurrentUser != null)
        {
            callBack(true);
            yield break;
        }
        callBack(false);
    }

    #region Autorization [REGISTER/LOGIN]
    [Obsolete]
    public void Autorization_Register(string email,string name,string password)
    {
        StartCoroutine(Check_IthernetConection(result =>
        {
            isIthernet = result;
            if (result)
            {
                Debug.Log("Подключено!");

                StartCoroutine(Check_Email_Registr(email, password, result =>
                {
                    if (result)
                    {
                       StartCoroutine(DataBase_Save(email, name, password));     
                    }
                    else
                    {
                        Debug.Log("Email не корректный или слабый пароль!");
                    }
                }));
            }
            else
            {
                Debug.Log("Сбой подключения!");
               
            }
        }));
    }

    [Obsolete]
    public void Autorization_SignIN(string email, string name, string password)
    {
        StartCoroutine(Check_IthernetConection(result =>
        {
            isIthernet = result;
            if (result)
            {
                Debug.Log("Подключено!");

                StartCoroutine(Check_Email_SignIN(email, password, result =>
                {
                    if (result)
                    {
                        StartCoroutine(DataBase_Load(email,name, password));
                    }
                    else
                    {
                        Debug.Log("Email не корректный или слабый пароль!");
                    }
                }));
            }
            else
            {
                Debug.Log("Сбой подключения!");
            }
        }));
    }

    public void Autorization_SignOUT()
    {
        Debug.Log("Выполнен выход из акаунта!");
        autorization.SignOut();
        SceneManager.LoadScene(0);
    }
    #endregion
    private IEnumerator DataBase_Save(string email,string name,string password)
    {
        DateTime date = new DateTime();
        User curentUser = new User(name, password, email, date.ToString());

        var json = JsonUtility.ToJson(curentUser);

        User user = JsonUtility.FromJson<User>(json);

        var curentDB = db.Child("Users:").Child(name).SetRawJsonValueAsync(json);

        yield return new WaitUntil(predicate: () => curentDB.IsCompleted);

        if(curentDB.Exception != null)
        {
            Debug.Log("Ошибка базы данных: " + curentDB.Exception.Message);
        }
        else
        {
            Debug.Log("Данные сохранены");
        }
    }

    private IEnumerator DataBase_Load(string email,string name,string password)
    {

        var curentDB = db.Child("Users:").Child(name).GetValueAsync();

        
        yield return new WaitUntil(predicate: () => curentDB.IsCompleted);

        if (curentDB.Exception != null)
        {
            Debug.Log("Ошибка базы данных: " + curentDB.Exception.Message);
        }
        else
        {
            DataSnapshot data = curentDB.Result;
            if(
                data.Child("Users:").Child(name).Value.ToString() == name
                             && 
                data.Child("Users:").Child("password").Value.ToString() == password
                )
            {
                Debug.Log("Данные сохранены" + data.Child("Users:").Value);
            }
        }
    }
}
