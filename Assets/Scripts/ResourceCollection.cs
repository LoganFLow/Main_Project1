using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ResourceCollection : MonoBehaviour
{
    [Header("Stone Settings")]
    public string questItemTag = "Stone";
    public int questItemCountNeeded = 5;
    private int questItemCountCollected = 0;
    public string collectKey = "e";
    public GameObject Box;

    [Header("UI")]
    public TMP_Text stoneCountText;
    public TMP_Text moneyCountText;
    public GameObject interactionPrompt;

    public DialogueSystem dialogueSystem;

    [Header("Money Settings")]
    public int money = 0;

    [Header("Animation Settings")]
    public Animator playerAnimator; // ������ �� Animator ������
    public string collectAnimationTrigger = "Collect"; // ��� �������� ��������
    private Rigidbody playerRigidbody;
    private bool isInStoneRange = false; // ���� ��� ��������, ��������� �� ����� � ���� �����
    private GameObject currentStone; // ������� ������, � ������� ��������������� �����
    private CharacterController characterController; // ������ �� CharacterController ������
    private bool isCollecting = false; // ���� ��� ��������, ���� �� ���� �����
    private BoxCollider playerBoxCollider;

    void Start()
    {
        if (dialogueSystem == null)
        {
            Debug.LogError("������ �� DialogueSystem �� ����������� � ResourceCollection!");
        }

        if (playerAnimator == null)
        {
            Debug.LogError("������ �� Animator �� ����������� � ResourceCollection!");
        }

        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("CharacterController �� ������ �� ������!");
        }

        playerBoxCollider = GetComponent<BoxCollider>();
        if (playerBoxCollider == null)
        {
            Debug.LogError("BoxCollider �� ������ �� ������!");
        }

        if (Box == null)
        {
            Debug.LogError("������ �� Box �� ����������� � ResourceCollection!");
        }

        UpdateUI();
    }

    void Update()
    {
        if (dialogueSystem != null && dialogueSystem.currentDialogueState == DialogueSystem.DialogueState.QuestActive)
        {
            if (isInStoneRange && !isCollecting)
            {
                ShowInteractionPrompt("������� 'E' ��� ����� �����");
                if (Input.GetKeyDown(collectKey.ToLower()))
                {
                    CollectQuestItem();
                }
            }
            else
            {
                HideInteractionPrompt();
            }
        }
        else
        {
            HideInteractionPrompt();
        }

        UpdateUI();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(questItemTag))
        {
            isInStoneRange = true;
            currentStone = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(questItemTag))
        {
            isInStoneRange = false;
            currentStone = null;
        }
    }

    void CollectQuestItem()
    {
        if (currentStone != null && !isCollecting)
        {
            questItemCountCollected++;
            Destroy(currentStone); 
            Debug.Log("������� " + questItemCountCollected + " �� " + questItemCountNeeded);

            // ��������������� �������� �����
            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger(collectAnimationTrigger);
            }

            
            StartCoroutine(BlockMovementForSeconds(2f));

            if (questItemCountCollected >= questItemCountNeeded && dialogueSystem != null)
            {
                dialogueSystem.CompleteQuest();
                Debug.Log("����� ��������!");
            }
        }
    }

    IEnumerator<WaitForSeconds> BlockMovementForSeconds(float seconds)
    
        {
        isCollecting = true;
        playerBoxCollider.enabled = true;
        yield return new WaitForSeconds(0.3f);
        characterController.enabled = false;
        
        yield return new WaitForSeconds(2f);
        characterController.enabled = true; 
        playerBoxCollider.enabled = false; 
        isCollecting = false;
    }

        void UpdateUI()
    {
        if (stoneCountText != null)
        {
            stoneCountText.text = "������: " + questItemCountCollected + "/" + questItemCountNeeded;
        }

        if (moneyCountText != null)
        {
            moneyCountText.text = "�����: " + money;
        }
    }

    public void AddMoney(int amount) // ����� ��� ���������� �����
    {
        money += amount;
        UpdateUI();
    }

    public void ResetStoneCount() // ����� ��� ������ �������� ������
    {
        questItemCountCollected = 0;
        UpdateUI();
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
}