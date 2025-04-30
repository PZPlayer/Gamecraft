using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject Player;
    public GameObject DropOfWater;
    public GameObject Key;
    public GameObject Gun;
    public GameObject Inventory;
    public GameObject ChatBox;
    public GameObject CameraScript;
    public Animator PlayerAnimator;
    public Animator UIDeathAnimator;
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
