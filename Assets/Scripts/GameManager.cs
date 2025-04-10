using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject Player;
    public GameObject DropOfWater;
    public Animator PlayerAnimator;
    public bool ifAiming;

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
