using UnityEngine;
using TMPro; // Используем TextMeshPro
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ClothingShop : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject dialogueBox;
    public TMP_Text dialogueText;
    public GameObject interactionPrompt; // Плашка с текстом
    public string interactionText = "Нажмите 'F' для разговора"; // Текст на плашке

    [Header("Dialogue Settings")]
    public float textSpeed = 0.05f;
    public List<string> purchaseDialogueLines; // Диалог при покупке одежды
    public List<string> notEnoughMoneyDialogueLines; // Диалог, если денег недостаточно
    public List<string> questNotCompletedDialogueLines; // Диалог, если квест не выполнен
    public List<string> postPurchaseDialogueLines; // Диалог после покупки одежды

    [Header("Clothing Settings")]
    public int clothingCost = 100;
    public GameObject[] itemsToHide; // Предметы, которые нужно скрыть после покупки
    public GameObject[] clothingItems; // Предметы одежды, которые нужно включить

    [Header("Post Purchase Buttons")]
    public GameObject buttonContainer; // Контейнер для кнопок
    public Button button1;
    public Button button2;
    public Button button3;

    private int currentLineIndex = 0;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private bool isInRange = false;
    private bool hasPurchased = false; // Флаг для проверки, была ли покупка

    private GameObject player;
    public float interactionDistance = 3f;
    public string interactKey = "f"; // Кнопка взаимодействия

    public Transform npcTransform; // Ссылка на Transform NPC
    public ResourceCollection resourceCollection;
    public DialogueSystem dialogueSystem; // Ссылка на DialogueSystem
    public StarSistem simpleDialogueSystem; // Ссылка на SimpleDialogueSystem

    private List<string> currentDialogueLines; // Текущий набор строк диалога

    void Start()
    {
        dialogueBox.SetActive(false);
        interactionPrompt.SetActive(false);
        buttonContainer.SetActive(false);

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

        dialogueSystem = FindObjectOfType<DialogueSystem>();
        if (dialogueSystem == null)
        {
            Debug.LogError("DialogueSystem скрипт не найден!");
        }

        simpleDialogueSystem = FindObjectOfType<StarSistem>();
        if (simpleDialogueSystem == null)
        {
            Debug.LogError("SimpleDialogueSystem скрипт не найден!");
        }

        if (interactionPrompt != null)
        {
            TMP_Text promptText = interactionPrompt.GetComponentInChildren<TMP_Text>();
            if (promptText != null)
            {
                promptText.text = interactionText;
            }
        }

        // Настройка кнопок
        button1.onClick.AddListener(OnButton1Click);
        button2.onClick.AddListener(OnButton2Click);
        button3.onClick.AddListener(OnButton3Click);
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
            if (hasPurchased)
            {
                StartDialogue(postPurchaseDialogueLines);
                buttonContainer.SetActive(true);
            }
            else if (dialogueSystem.currentDialogueState == DialogueSystem.DialogueState.QuestRewarded)
            {
                if (resourceCollection.money >= clothingCost)
                {
                    StartDialogue(purchaseDialogueLines);
                }
                else
                {
                    StartDialogue(notEnoughMoneyDialogueLines);
                }
            }
            else
            {
                StartDialogue(questNotCompletedDialogueLines);
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

        if (dialogueSystem.currentDialogueState == DialogueSystem.DialogueState.QuestRewarded && !hasPurchased)
        {
            if (resourceCollection.money >= clothingCost)
            {
                resourceCollection.AddMoney(-clothingCost);
                foreach (var item in clothingItems)
                {
                    item.SetActive(true);
                }
                foreach (var item in itemsToHide)
                {
                    item.SetActive(false);
                }
                hasPurchased = true;
                simpleDialogueSystem.hasPurchased = true; // Устанавливаем флаг покупки в SimpleDialogueSystem
            }
        }

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
        buttonContainer.SetActive(false);
    }

    void OnButton1Click()
    {
        Debug.Log("Кнопка 1 нажата");
        // Добавьте здесь логику для кнопки 1
    }

    void OnButton2Click()
    {
        Debug.Log("Кнопка 2 нажата");
        // Добавьте здесь логику для кнопки 2
    }

    void OnButton3Click()
    {
        Debug.Log("Кнопка 3 нажата");
        // Добавьте здесь логику для кнопки 3
    }
}