using UnityEngine;

public class NoPathWarning : MonoBehaviour
{
    public GameController gameController;

    void Start()
    {
        gameController.NoPath += OnNoPath;
    }

    void OnNoPath()
    {
        GetComponent<Animator>().SetTrigger("Play");
    }
}