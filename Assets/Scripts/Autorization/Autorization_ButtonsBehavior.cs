using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Autorization_ButtonsBehavior : MonoBehaviour
{
    private Autorization_Game gameAuth;

    [SerializeField] private TMP_InputField textInput_Name;
    [SerializeField] private TMP_InputField textInput_Email;
    [SerializeField] private TMP_InputField textInput_Password;

    void Start()
    {
        gameAuth = GetComponent<Autorization_Game>();
        textInput_Password.inputType = TMP_InputField.InputType.Password;
    }

    
    public void Button_Register()
    {
        gameAuth.Autorization_Register(textInput_Email.text, textInput_Name.text, textInput_Password.text);

    }

    
    public void Button_SignIN()
    {
        gameAuth.Autorization_SignIN(textInput_Email.text, textInput_Name.text, textInput_Password.text);

    }
    public void Button_SignOUT()
    {
        gameAuth.Autorization_SignOUT();
    }
}
