using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct Talk
{
    public string Text;
    public List<string> Options;
    public List<int> NextTalkIndices;
}

[CreateAssetMenu(fileName = "Chat", menuName = "Dialogue/Chat")]
public class Chat : ScriptableObject
{
    public string Name;
    public List<Talk> Talks;
}

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] private float _interactionDistance = 3f;
    [SerializeField] private KeyCode _interactionKey = KeyCode.E;
    [SerializeField] private Transform _cameraTarget;
    [SerializeField] private Transform _buttonsSpawn;
    [SerializeField] private GameObject _buttonPrefab;
    [SerializeField] private GameObject _chatBox;
    [SerializeField] private GameObject _inventory;
    [SerializeField] private TMP_Text _dialogueText;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private AudioClip _typingSound;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Chat _currentChat;

    private bool _isInDialogue = false;
    private bool _isTyping = false;
    private bool _waitingForChoice = false;
    private Camera _mainCamera;
    private Vector3 _originalCameraPosition;
    private Quaternion _originalCameraRotation;
    private GameObject _player;
    private int _currentTalkIndex = 0; 

    private void Start()
    {
        _mainCamera = Camera.main;
        _inventory = GameManager.Instance.Inventory;
        _chatBox = GameManager.Instance.ChatBox;
    }

    private void Update()
    {
        _player = GameManager.Instance.Player;

        if (Vector3.Distance(transform.position, _player.transform.position) <= _interactionDistance)
        {
            if (Input.GetKeyDown(_interactionKey) && !_isInDialogue)
            {
                StartDialogue();
            }
        }

        if (_isInDialogue && Input.GetKeyDown(KeyCode.Escape))
        {
            EndDialogue();
        }

        if (_isInDialogue && !_waitingForChoice && !_isTyping && Input.GetMouseButtonDown(0))
        {
            _currentTalkIndex++;
            StartCoroutine(ShowTalk(_currentTalkIndex));
        }
    }

    private void StartDialogue()
    {
        _currentTalkIndex = 0;
        _inventory.SetActive(false);
        _chatBox.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GameManager.Instance.CameraScript.SetActive(false);

        _isInDialogue = true;
        _nameText.text = _currentChat.Name;
        _originalCameraPosition = _mainCamera.transform.position;
        _originalCameraRotation = _mainCamera.transform.rotation;

        _mainCamera.transform.position = _cameraTarget.position;
        _mainCamera.transform.rotation = _cameraTarget.rotation;

        StartCoroutine(ShowTalk(_currentTalkIndex));
    }

    private void EndDialogue()
    {
        _isInDialogue = false;
        _mainCamera.transform.position = _originalCameraPosition;
        _mainCamera.transform.rotation = _originalCameraRotation;
        _dialogueText.text = "";
        ClearButtons();

        _inventory.SetActive(true);
        _chatBox.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GameManager.Instance.CameraScript.SetActive(true);
    }

    private IEnumerator ShowTalk(int talkIndex)
    {
        if (talkIndex >= _currentChat.Talks.Count || talkIndex < 0)
        {
            EndDialogue();
            yield break;
        }

        _currentTalkIndex = talkIndex; 
        Talk currentTalk = _currentChat.Talks[talkIndex];
        ClearButtons();

        _isTyping = true;
        yield return StartCoroutine(TypeText(currentTalk.Text));
        _isTyping = false;

        if (currentTalk.Options != null && currentTalk.Options.Count > 0)
        {
            _waitingForChoice = true;
            for (int i = 0; i < currentTalk.Options.Count; i++)
            {
                int optionIndex = i;
                GameObject buttonObj = Instantiate(_buttonPrefab, _buttonsSpawn);
                Button button = buttonObj.GetComponent<Button>();
                TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>();
                buttonText.text = currentTalk.Options[optionIndex];

                int nextTalkIndex = currentTalk.NextTalkIndices[optionIndex];
                button.onClick.AddListener(() =>
                {
                    _waitingForChoice = false;
                    ClearButtons();
                    _currentTalkIndex = nextTalkIndex; 
                    StartCoroutine(ShowTalk(nextTalkIndex));
                });
            }
        }
    }

    private IEnumerator TypeText(string text)
    {
        _mainCamera.transform.position = _cameraTarget.position;
        _mainCamera.transform.rotation = _cameraTarget.rotation;

        _dialogueText.text = "";
        foreach (char letter in text.ToCharArray())
        {
            _dialogueText.text += letter;
            if (_typingSound != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(_typingSound);
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void ClearButtons()
    {
        foreach (Transform child in _buttonsSpawn)
        {
            Destroy(child.gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _interactionDistance);
    }
}