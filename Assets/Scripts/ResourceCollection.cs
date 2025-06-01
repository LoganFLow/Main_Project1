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
    public Animator playerAnimator; // Ссылка на Animator игрока
    public string collectAnimationTrigger = "Collect"; // Имя триггера анимации
    private Rigidbody playerRigidbody;
    private bool isInStoneRange = false; // Флаг для проверки, находится ли игрок в зоне камня
    private GameObject currentStone; // Текущий камень, с которым взаимодействует игрок
    private CharacterController characterController; // Ссылка на CharacterController игрока
    private bool isCollecting = false; // Флаг для проверки, идет ли сбор камня
    private BoxCollider playerBoxCollider;

    void Start()
    {
        if (dialogueSystem == null)
        {
            Debug.LogError("Ссылка на DialogueSystem не установлена в ResourceCollection!");
        }

        if (playerAnimator == null)
        {
            Debug.LogError("Ссылка на Animator не установлена в ResourceCollection!");
        }

        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("CharacterController не найден на игроке!");
        }

        playerBoxCollider = GetComponent<BoxCollider>();
        if (playerBoxCollider == null)
        {
            Debug.LogError("BoxCollider не найден на игроке!");
        }

        if (Box == null)
        {
            Debug.LogError("Ссылка на Box не установлена в ResourceCollection!");
        }

        UpdateUI();
    }

    void Update()
    {
        if (dialogueSystem != null && dialogueSystem.currentDialogueState == DialogueSystem.DialogueState.QuestActive)
        {
            if (isInStoneRange && !isCollecting)
            {
                ShowInteractionPrompt("Нажмите 'E' для сбора камня");
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
            Debug.Log("Собрано " + questItemCountCollected + " из " + questItemCountNeeded);

            // Воспроизведение анимации сбора
            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger(collectAnimationTrigger);
            }

            
            StartCoroutine(BlockMovementForSeconds(2f));

            if (questItemCountCollected >= questItemCountNeeded && dialogueSystem != null)
            {
                dialogueSystem.CompleteQuest();
                Debug.Log("Квест выполнен!");
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
            stoneCountText.text = "Камней: " + questItemCountCollected + "/" + questItemCountNeeded;
        }

        if (moneyCountText != null)
        {
            moneyCountText.text = "Денег: " + money;
        }
    }

    public void AddMoney(int amount) // Метод для добавления денег
    {
        money += amount;
        UpdateUI();
    }

    public void ResetStoneCount() // Метод для сброса счетчика камней
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