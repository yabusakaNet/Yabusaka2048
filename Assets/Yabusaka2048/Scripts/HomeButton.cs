using UnityEngine;

public class HomeButton : MonoBehaviour
{
    public void OnHome ()
    {
        FadeManager.Instance.LoadScene ("Top");
    }
}
