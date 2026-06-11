using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class DialogueTrigger : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        public string text;
        [Tooltip("Vises kun hvis dette flag er sat. Lad stå tomt for altid at vise.")]
        public string requiresFlag;
    }

    [SerializeField] private List<DialogueLine> lines;
    [SerializeField] private float rotationSpeed = 0.5f;

    private List<DialogueLine> _activeLines = new();
    private int _currentIndex = -1;
    private bool _isActive;
    private Transform _player;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _player = other.transform;
        StartDialogue();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _player = null;
        EndDialogue();
    }

    private void Update()
    {
        if (_player != null)
        {
            Vector3 toPlayer = _player.position - transform.position;
            toPlayer.y = 0f;
            if (toPlayer.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(toPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        if (!_isActive) return;

#if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            ShowNext();
#else
        if (Input.GetMouseButtonDown(0))
            ShowNext();
#endif
    }

    private void StartDialogue()
    {
        _activeLines.Clear();
        foreach (DialogueLine line in lines)
        {
            if (string.IsNullOrEmpty(line.requiresFlag) || DialogueUI.Instance.HasFlag(line.requiresFlag))
                _activeLines.Add(line);
        }

        if (_activeLines.Count == 0) return;

        _currentIndex = 0;
        _isActive = true;
        DialogueUI.Instance.ShowLine(_activeLines[0].text);
    }

    private void ShowNext()
    {
        _currentIndex++;
        if (_currentIndex < _activeLines.Count)
            DialogueUI.Instance.ShowLine(_activeLines[_currentIndex].text);
        else
            EndDialogue();
    }

    private void EndDialogue()
    {
        _isActive = false;
        _currentIndex = -1;
        DialogueUI.Instance.Hide();
    }
}
