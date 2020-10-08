using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HomeButton : MonoBehaviour
{
    public void OnHome()
    {   
        SceneManager.LoadScene("Top");
    }
}
