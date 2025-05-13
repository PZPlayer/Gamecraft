using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;

namespace Gamecraft.UI
{
    public class SettingsManager : MonoBehaviour
    {
        public static SettingsManager SETTINGS;

        [SerializeField] private float mainVolume;
        [SerializeField] private float sFVolume;
        [SerializeField] private float melodyVolume;

        [SerializeField] private float mouseX;
        [SerializeField] private float mouseY;

        [SerializeField] private int quality;
        [SerializeField] private int resolution;

        public Slider _mainVolume;
        public Slider _sFVolume;
        public Slider _melodyVolume;

        public Slider _mouseX;
        public Slider _mouseY;

        public TMP_Dropdown _quality;
        public TMP_Dropdown _resolution;

        public Button SaveButton;

        [SerializeField] private AudioSource _mainMenuMelody;
        [SerializeField] private AudioSource _mainMenuSounds;

        private void Start ()
        {
            if(SETTINGS == null)
            {
                DontDestroyOnLoad(gameObject);
                SETTINGS = this;
                UpdateAllSettings();
            }
            else
            {
                SETTINGS._mainVolume = _mainVolume;
                SETTINGS._sFVolume = _sFVolume;
                SETTINGS._melodyVolume = _melodyVolume;
                SETTINGS._mouseX = _mouseX;
                SETTINGS._mouseY = _mouseY;
                SETTINGS._quality = _quality;
                SETTINGS._resolution = _resolution;
                SETTINGS.GiveCompanents();
                Destroy(gameObject);
            }
        }

        public void GiveCompanents()
        {
            _mainVolume.value = mainVolume;
            _sFVolume.value = sFVolume;
            _melodyVolume.value = melodyVolume;
            _mouseX.value = mouseX;
            _mouseY.value = mouseY;
            _quality.value = quality;
            _resolution.value = resolution;
        }

        public void UpdateAllSettings()
        {
            AddListners();
            UpdateParameters();
            Updatequality();
            Updateresolution();
            UpdateMouseSettings();
            UpdateVolume();
        }

        public void AddListners(Button newButton = null)
        {
            if (SaveButton == null) SaveButton = GameManager.Instance.SaveButton;
            SaveButton.onClick.AddListener(UpdateAllSettings);
        }

        private void UpdateParameters(float useless = 0.0f)
        {
            mainVolume = _mainVolume.value;
            sFVolume = _sFVolume.value;
            melodyVolume = _melodyVolume.value;

            mouseX = _mouseX.value;
            mouseY = _mouseY.value;

            quality = _quality.value;
            resolution = _resolution.value;
        }

        private void Updatequality()
        {
            switch (_quality.value)
            {
                case 0:
                    QualitySettings.SetQualityLevel(5);
                    break;
                case 1:
                    QualitySettings.SetQualityLevel(4);
                    break;
                case 2:
                    QualitySettings.SetQualityLevel(2);
                    break;
                case 3:
                    QualitySettings.SetQualityLevel(0);
                    break;
                default:
                    QualitySettings.SetQualityLevel(5);
                    break;
            }
            
        }

        private void UpdateMouseSettings()
        {
            if (GameManager.Instance == null) return;
            
            GameManager.Instance.CameraContoller.m_XAxis.m_MaxSpeed = mouseX;
            GameManager.Instance.CameraContoller.m_YAxis.m_MaxSpeed = mouseY;
        }

        private void UpdateVolume()
        {
            float mainVolume = _mainVolume.value / 100;
            if (GameManager.Instance == null) 
            {

                float finalVolume = Mathf.Lerp(0, 1, mainVolume * (melodyVolume / 100));
                _mainMenuMelody.volume = finalVolume;

                float finalSoundVolume = Mathf.Lerp(0, 1, mainVolume * (sFVolume / 100));
                _mainMenuSounds.volume = finalSoundVolume;

                return;
            }

            foreach (AudioSource source in GameManager.Instance.AudioMelodyes)
            {
                float finalVolume = Mathf.Lerp(0, 1, mainVolume * (melodyVolume / 100));
                source.volume = finalVolume;
            }

            foreach (AudioSource source in GameManager.Instance.AudioEffects)
            {
                float finalVolume = Mathf.Lerp(0, 1, mainVolume * (sFVolume / 100));
                source.volume = finalVolume;
            }
        }

        private void Updateresolution()
        {
            switch (resolution)
            {
                case 0:
                    Screen.SetResolution(3840, 2160, true);
                    break;
                case 1:
                    Screen.SetResolution(2560, 1440, true);
                    break;
                case 2:
                    Screen.SetResolution(1920, 1080, true);
                    break;
                case 3:
                    Screen.SetResolution(960, 540, true);
                    break;
                default:
                    Screen.SetResolution(1920, 1080, true);
                    break;
            }
        }
    }
}

