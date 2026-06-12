using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance { get; private set; }
    public static bool IsActive { get; private set; }

    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject dialoguePanel;

    private readonly HashSet<string> _flags = new HashSet<string>();

    private void Awake()
    {
        Instance = this;
        dialoguePanel.SetActive(false);
    }

    public void ShowLine(string text)
    {
        dialogueText.text = text;
        dialoguePanel.SetActive(true);
        IsActive = true;
    }

    public void Hide()
    {
        dialoguePanel.SetActive(false);
        IsActive = false;
    }

    public void SetFlag(string key) => _flags.Add(key);
    public bool HasFlag(string key) => _flags.Contains(key);
}
