using UnityEngine;
using TMPro; // ���������� TextMeshPro
using System.Collections;
using System.Collections.Generic;

public class StarSistem : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject dialogueBox;
    public TMP_Text dialogueText;
    public GameObject interactionPrompt; // ������ � �������
    public string interactionText = "������� 'F' ��� ���������"; // ����� �� ������

    [Header("Dialogue Settings")]
    public float textSpeed = 0.05f;
    public List<string> firstEncounterDialogueLines; // ������ ��� ������ �������
    public List<string> secondEncounterDialogueLines; // ������ ��� ������ �������

    private int currentLineIndex = 0;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private bool isInRange = false;
    private bool hasFirstEncounter = false; // ���� ��� ��������, ��� �� ������ ������
    public bool hasPurchased = false; // ���� ��� ��������, ���� �� �������

    private GameObject player;
    public float interactionDistance = 3f;
    public string interactKey = "f"; // ������ ��������������

    private List<string> currentDialogueLines; // ������� ����� ����� �������

    void Start()
    {
        dialogueBox.SetActive(false);
        interactionPrompt.SetActive(false);

        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("����� � ����� 'Player' �� ������!");
        }

        if (interactionPrompt != null)
        {
            TMP_Text promptText = interactionPrompt.GetComponentInChildren<TMP_Text>();
            if (promptText != null)
            {
                promptText.text = interactionText;
            }
        }
    }

    void Update()
    {
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance <= interactionDistance && !isInRange)
            {
                isInRange = true;
                ShowInteractionPrompt(interactionText);
            }
            else if (distance > interactionDistance + 0.5f && isInRange)
            {
                isInRange = false;
                HideInteractionPrompt();
                HideDialogue();
            }

            if (isInRange && Input.GetKeyDown(interactKey.ToLower()))
            {
                OnInteractButtonClick();
            }

            if (isDialogueActive && Input.GetMouseButtonDown(1)) // ��� ��� �����
            {
                if (isTyping)
                {
                    SkipText();
                }
                else
                {
                    NextLine();
                }
            }
        }
    }

    void OnInteractButtonClick()
    {
        if (!isDialogueActive)
        {
            if (!hasFirstEncounter)
            {
                StartDialogue(firstEncounterDialogueLines);
                hasFirstEncounter = true;
            }
            else if (hasPurchased)
            {
                StartDialogue(secondEncounterDialogueLines);
            }
        }
    }

    void StartDialogue(List<string> lines)
    {
        if (isDialogueActive) return;
        isDialogueActive = true;
        dialogueBox.SetActive(true);
        HideInteractionPrompt();
        currentLineIndex = 0;
        currentDialogueLines = lines; // ������������� ������� ����� �����
        StartCoroutine(TypeLine(currentDialogueLines)); // �������� currentDialogueLines

        // ��������� ������
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    IEnumerator TypeLine(List<string> lines)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char letter in lines[currentLineIndex].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
        isTyping = false;
    }

    void NextLine()
    {
        if (isTyping) return;

        currentLineIndex++;
        if (currentLineIndex < currentDialogueLines.Count)
        {
            StartCoroutine(TypeLine(currentDialogueLines));
        }
        else
        {
            EndDialogue();
        }
    }

    void SkipText()
    {
        StopAllCoroutines();
        dialogueText.text = currentDialogueLines[currentLineIndex];
        isTyping = false;
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        dialogueBox.SetActive(false);

        // ������������ ������
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void ShowInteractionPrompt(string text)
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(true);
            TMP_Text promptText = interactionPrompt.GetComponentInChildren<TMP_Text>();
            if (promptText != null)
            {
                promptText.text = text;
            }
        }
    }

    void HideInteractionPrompt()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    void HideDialogue()
    {
        isDialogueActive = false;
        dialogueBox.SetActive(false);
    }
}