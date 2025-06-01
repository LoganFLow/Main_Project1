using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogueSystem : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject dialogueBox;
    public TMP_Text dialogueText;
    public GameObject interactionPrompt; // ������ � �������

    [Header("Dialogue Settings")]
    public float textSpeed = 0.05f;
    public List<string> initialDialogueLines; // ������ ��� ������ ��������������
    public List<string> questStartDialogueLines; // ������ ��� ������ �������
    public List<string> questCompleteDialogueLines; // ������ ��� ���������� �������
    public List<string> questRewardedDialogueLines; // ������ ����� ������� �� �����
    public GameObject objectToDisableOnQuestComplete;
    public GameObject objectToDisableOnQuestComplete1;
    public string interactionText = "������� 'F' ��� ���������"; // ����� �� ������
    public string questStartText = "������� 'E' ����� ������� �������"; // ����� �� ������ ��� �������� �������

    [Header("Quest Settings")]
    public bool hasQuest = false;

    private int currentLineIndex = 0;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private bool isInRange = false;
    public DialogueState currentDialogueState = DialogueState.Initial; // ������� ���������

    private GameObject player;
    public float interactionDistance = 3f;
    public string interactKey = "f"; // ������ interactKey = 'f'

    public Transform npcTransform; // ������ �� Transform NPC
    public ResourceCollection resourceCollection;

    [Header("Reward Settings")]
    public int rewardAmount = 100; //  ������ �������

    public enum DialogueState
    {
        Initial,
        QuestStart,
        QuestActive,
        QuestComplete,
        QuestRewarded
    }

    private List<string> currentDialogueLines; //  ������� ����� ����� �������

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
            Debug.LogError("����� � ����� 'Player' �� ������!");
        }

        resourceCollection = player.GetComponent<ResourceCollection>();
        if (resourceCollection == null)
        {
            Debug.LogError("ResourceCollection ������ �� ������!");
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
                        ShowInteractionPrompt("��������� �����!");
                        break;
                    case DialogueState.QuestComplete:
                        ShowInteractionPrompt("������� ���������! ������� 'F' ��� ��������� �������");
                        if (Input.GetKeyDown(interactKey.ToLower()))
                        {
                            StartDialogue(questCompleteDialogueLines);
                            RewardQuest();
                        }
                        break;
                    case DialogueState.QuestRewarded:
                        ShowInteractionPrompt("������� �� ������!");
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

    void StartDialogue(List<string> lines)
    {
        if (isDialogueActive) return;
        isDialogueActive = true;
        dialogueBox.SetActive(true);
        HideInteractionPrompt();
        currentLineIndex = 0;
        currentDialogueLines = lines; //  ������������� ������� ����� �����
        StartCoroutine(TypeLine(currentDialogueLines)); //  �������� currentDialogueLines
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
        Debug.Log("����� ������!");
        hasQuest = true;
        currentDialogueState = DialogueState.QuestActive;
        resourceCollection.ResetStoneCount(); // ���������� ������� ������
    }

    public void CompleteQuest() // ����������, ����� ����� �������� �������
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
        Debug.Log("�� �������� ������� �� �����!");
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