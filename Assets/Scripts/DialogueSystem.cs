using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogueSystem : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject dialogueBox;
    public TMP_Text dialogueText;
    public GameObject interactionPrompt; // Плашка с текстом

    [Header("Dialogue Settings")]
    public float textSpeed = 0.05f;
    public List<string> initialDialogueLines; // Диалог при первом взаимодействии
    public List<string> questStartDialogueLines; // Диалог при старте задания
    public List<string> questCompleteDialogueLines; // Диалог при завершении задания
    public List<string> questRewardedDialogueLines; // Диалог после награды за квест
    public GameObject objectToDisableOnQuestComplete;
    public GameObject objectToDisableOnQuestComplete1;
    public string interactionText = "Нажмите 'F' для разговора"; // Текст на плашке
    public string questStartText = "Нажмите 'E' чтобы принять задание"; // Текст на плашке для принятия задания

    [Header("Quest Settings")]
    public bool hasQuest = false;

    private int currentLineIndex = 0;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private bool isInRange = false;
    public DialogueState currentDialogueState = DialogueState.Initial; // Сделали публичным

    private GameObject player;
    public float interactionDistance = 3f;
    public string interactKey = "f"; // Теперь interactKey = 'f'

    public Transform npcTransform; // Ссылка на Transform NPC
    public ResourceCollection resourceCollection;

    [Header("Reward Settings")]
    public int rewardAmount = 100; //  Размер награды

    public enum DialogueState
    {
        Initial,
        QuestStart,
        QuestActive,
        QuestComplete,
        QuestRewarded
    }

    private List<string> currentDialogueLines; //  Текущий набор строк диалога

    void Start()
    {
        dialogueBox.SetActive(false);
        interactionPrompt.SetActive(false);

        if (interactionPrompt != null)
        {
            TMP_Text promptText = interactionPrompt.GetComponentInChildren<TMP_Text>();
            if (promptText != null)
            {
                promptText.text = interactionText;
            }
        }

        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Игрок с тегом 'Player' не найден!");
        }

        resourceCollection = player.GetComponent<ResourceCollection>();
        if (resourceCollection == null)
        {
            Debug.LogError("ResourceCollection скрипт не найден!");
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
            }
            else if (distance > interactionDistance + 0.5f && isInRange)
            {
                isInRange = false;
                HideInteractionPrompt();
            }

            if (isInRange)
            {
                switch (currentDialogueState)
                {
                    case DialogueState.Initial:
                        ShowInteractionPrompt(interactionText);
                        if (Input.GetKeyDown(interactKey.ToLower()))
                        {
                            StartDialogue(initialDialogueLines);
                        }
                        break;
                    case DialogueState.QuestStart:
                        ShowInteractionPrompt(questStartText);
                        if (Input.GetKeyDown(interactKey.ToLower()))
                        {
                            StartDialogue(questStartDialogueLines);
                            ActivateQuest();
                        }
                        break;
                    case DialogueState.QuestActive:
                        ShowInteractionPrompt("Собирайте камни!");
                        break;
                    case DialogueState.QuestComplete:
                        ShowInteractionPrompt("Задание выполнено! Нажмите 'F' для получения награды");
                        if (Input.GetKeyDown(interactKey.ToLower()))
                        {
                            StartDialogue(questCompleteDialogueLines);
                            RewardQuest();
                        }
                        break;
                    case DialogueState.QuestRewarded:
                        ShowInteractionPrompt("Спасибо за помощь!");
                        if (Input.GetKeyDown(interactKey.ToLower()))
                        {
                            StartDialogue(questRewardedDialogueLines);
                            HideInteractionPrompt();
                        }
                        break;
                }
            }
            else
            {
                HideInteractionPrompt();
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

    void StartDialogue(List<string> lines)
    {
        if (isDialogueActive) return;
        isDialogueActive = true;
        dialogueBox.SetActive(true);
        HideInteractionPrompt();
        currentLineIndex = 0;
        currentDialogueLines = lines; //  Устанавливаем текущий набор строк
        StartCoroutine(TypeLine(currentDialogueLines)); //  Передаем currentDialogueLines
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

        switch (currentDialogueState)
        {
            case DialogueState.Initial:
                currentDialogueState = DialogueState.QuestStart;
                break;
            case DialogueState.QuestStart:
                currentDialogueState = DialogueState.QuestActive;
                break;
            case DialogueState.QuestComplete:
                currentDialogueState = DialogueState.QuestRewarded;
                break;
        }
    }

    void ActivateQuest()
    {
        Debug.Log("Квест принят!");
        hasQuest = true;
        currentDialogueState = DialogueState.QuestActive;
        resourceCollection.ResetStoneCount(); // Сбрасываем счетчик камней
    }

    public void CompleteQuest() // Вызывается, когда игрок выполнил задание
    {
        currentDialogueState = DialogueState.QuestComplete;
        currentDialogueState = DialogueState.QuestComplete;
        if (objectToDisableOnQuestComplete != null)
        {
            objectToDisableOnQuestComplete.SetActive(false);
            objectToDisableOnQuestComplete1.SetActive(false);
        }
    }

    void RewardQuest()
    {
        Debug.Log("Вы получили награду за квест!");
        if (resourceCollection != null)
        {
            resourceCollection.AddMoney(rewardAmount);
        }
        currentDialogueState = DialogueState.QuestRewarded;
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

    public bool HasQuest()
    {
        return hasQuest;
    }
}