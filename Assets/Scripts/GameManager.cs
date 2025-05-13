using Cinemachine;
using Gamecraft.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject Player;
    public GameObject PlayerBody;
    public GameObject DropOfWater;
    public GameObject Key;
    public GameObject Gun;
    public GameObject Inventory;
    public GameObject ChatBox;
    public GameObject CameraScript;
    public Animator PlayerAnimator;
    public Animator UIDeathAnimator;
    public CinemachineFreeLook CameraContoller;
    public List<AudioSource> AudioEffects;
    public List<AudioSource> AudioMelodyes;
    public bool ifAiming;
    public Button SaveButton;

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            if (Instance.Player == null)
            {
                Instance.Player = Player;
            }
            Destroy(gameObject);
        }

        SettingsManager.SETTINGS.UpdateAllSettings();
    }
}