using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Souns_Behavior : MonoBehaviour
{
   AudioSource m_AudioSource;
  // public GameObject stepPC;

    private void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
       

       
    }
    public void Step_Sound()
    {
        m_AudioSource.PlayOneShot(m_AudioSource.clip);
        //Instantiate(stepPC,transform.position,Quaternion.identity);
        //Destroy(stepPC,1.5f);
    }
   
}
