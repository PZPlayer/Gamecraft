using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject Player;

    public static GameManager Instance;

    private void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            if(Instance.Player == null)
            {
                Instance.Player = Player;
            }
            Destroy(gameObject);
        }
    }
}
