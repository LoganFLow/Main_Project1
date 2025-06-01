using UnityEngine;
using TMPro; // Используем TextMeshPro
using System.Collections;
using System.Collections.Generic;

public class StarSistem : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject dialogueBox;
    public TMP_Text dialogueText;
    public GameObject interactionPrompt; // Плашка с текстом
    public string interactionText = "Нажмите 'F' для разговора"; // Текст на плашке

    [Header("Dialogue Settings")]
    public float textSpeed = 0.05f;
    public List<string> firstEncounterDialogueLines; // Диалог при первом подходе
    public List<string> secondEncounterDialogueLines; // Диалог при втором подходе

    private int currentLineIndex = 0;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private bool isInRange = false;
    private bool hasFirstEncounter = false; // Флаг для проверки, был ли первый подход
    public bool hasPurchased = false; // Флаг для проверки, была ли покупка

    private GameObject player;
    public float interactionDistance = 3f;
    public string interactKey = "f"; // Кнопка взаимодействия

    private List<string> currentDialogueLines; // Текущий набор строк диалога

    void Start()
    {
        dialogueBox.SetActive(false);
        interactionPrompt.SetActive(false);

        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Игрок с тегом 'Player' не найден!");
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

            if (isDialogueActive && Input.GetMouseButtonDown(1)) // ЛКМ для скипа
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
        currentDialogueLines = lines; // Устанавливаем текущий набор строк
        StartCoroutine(TypeLine(currentDialogueLines)); // Передаем currentDialogueLines

        // Блокируем курсор
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

        // Разблокируем курсор
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