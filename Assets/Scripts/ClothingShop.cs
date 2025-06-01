using UnityEngine;
using TMPro; // ���������� TextMeshPro
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ClothingShop : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject dialogueBox;
    public TMP_Text dialogueText;
    public GameObject interactionPrompt; // ������ � �������
    public string interactionText = "������� 'F' ��� ���������"; // ����� �� ������

    [Header("Dialogue Settings")]
    public float textSpeed = 0.05f;
    public List<string> purchaseDialogueLines; // ������ ��� ������� ������
    public List<string> notEnoughMoneyDialogueLines; // ������, ���� ����� ������������
    public List<string> questNotCompletedDialogueLines; // ������, ���� ����� �� ��������
    public List<string> postPurchaseDialogueLines; // ������ ����� ������� ������

    [Header("Clothing Settings")]
    public int clothingCost = 100;
    public GameObject[] itemsToHide; // ��������, ������� ����� ������ ����� �������
    public GameObject[] clothingItems; // �������� ������, ������� ����� ��������

    [Header("Post Purchase Buttons")]
    public GameObject buttonContainer; // ��������� ��� ������
    public Button button1;
    public Button button2;
    public Button button3;

    private int currentLineIndex = 0;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private bool isInRange = false;
    private bool hasPurchased = false; // ���� ��� ��������, ���� �� �������

    private GameObject player;
    public float interactionDistance = 3f;
    public string interactKey = "f"; // ������ ��������������

    public Transform npcTransform; // ������ �� Transform NPC
    public ResourceCollection resourceCollection;
    public DialogueSystem dialogueSystem; // ������ �� DialogueSystem
    public StarSistem simpleDialogueSystem; // ������ �� SimpleDialogueSystem

    private List<string> currentDialogueLines; // ������� ����� ����� �������

    void Start()
    {
        dialogueBox.SetActive(false);
        interactionPrompt.SetActive(false);
        buttonContainer.SetActive(false);

        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("����� � ����� 'Player' �� ������!");
        }

        resourceCollection = player.GetComponent<ResourceCollection>();
        if (resourceCollection == null)
        {
            Debug.LogError("ResourceCollection ������ �� ������!");
        }

        dialogueSystem = FindObjectOfType<DialogueSystem>();
        if (dialogueSystem == null)
        {
            Debug.LogError("DialogueSystem ������ �� ������!");
        }

        simpleDialogueSystem = FindObjectOfType<StarSistem>();
        if (simpleDialogueSystem == null)
        {
            Debug.LogError("SimpleDialogueSystem ������ �� ������!");
        }

        if (interactionPrompt != null)
        {
            TMP_Text promptText = interactionPrompt.GetComponentInChildren<TMP_Text>();
            if (promptText != null)
            {
                promptText.text = interactionText;
            }
        }

        // ��������� ������
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
                simpleDialogueSystem.hasPurchased = true; // ������������� ���� ������� � SimpleDialogueSystem
            }
        }

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
        buttonContainer.SetActive(false);
    }

    void OnButton1Click()
    {
        Debug.Log("������ 1 ������");
        // �������� ����� ������ ��� ������ 1
    }

    void OnButton2Click()
    {
        Debug.Log("������ 2 ������");
        // �������� ����� ������ ��� ������ 2
    }

    void OnButton3Click()
    {
        Debug.Log("������ 3 ������");
        // �������� ����� ������ ��� ������ 3
    }
}