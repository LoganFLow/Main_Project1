using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TriggerTeleport: MonoBehaviour
{
    [Header("Teleport Settings")]
    public Transform teleportTarget; // Место, куда телепортируется игрок
    public GameObject objectToActivate; // Объект, который нужно активировать при входе в триггер
    public KeyCode teleportKey = KeyCode.E; // Клавиша для телепортации

    private GameObject player; // Ссылка на игрока по тегу
    private bool isInRange = false; // Флаг, указывающий, находится ли игрок в зоне триггера
    private CharacterController playerController; // Ссылка на CharacterController игрока
    private PlayerMove playerMovement; // Ссылка на скрипт движения игрока

    void Start()
    {
        //Ищем игрока в сцене
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Не найден GameObject с тегом 'Player' в сцене!");
            enabled = false;
            return;
        }

        playerController = player.GetComponent<CharacterController>();
        if (playerController == null)
        {
            Debug.LogError("Player не имеет компонента CharacterController!");
            enabled = false;
            return;
        }

        playerMovement = player.GetComponent<PlayerMove>();
        if (playerMovement == null)
        {
            Debug.LogWarning("У игрока не найден скрипт GTAStyleMovement. Телепортация может работать некорректно.");
        }


        // Проверка наличия телепорт таргета
        if (teleportTarget == null)
        {
            Debug.LogError("Не указан Teleport Target! Пожалуйста, укажите объект в инспекторе.");
            enabled = false;
            return;
        }

        // Проверка наличия объекта для активации
        if (objectToActivate == null)
        {
            Debug.LogError("Не указан Object To Activate! Пожалуйста, укажите объект в инспекторе.");
            enabled = false;
            return;
        }

        // Отключаем объект при старте
        objectToActivate.SetActive(false);
    }

    void Update()
    {
        // Если игрок находится в зоне триггера и нажал клавишу телепортации
        if (isInRange && Input.GetKeyDown(teleportKey))
        {
            TeleportPlayer();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Если в триггер вошел игрок
        if (other.gameObject.CompareTag("Player"))
        {
            isInRange = true; // Устанавливаем флаг, что игрок в зоне триггера
            objectToActivate.SetActive(true); // Активируем объект
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Если игрок вышел из триггера
        if (other.gameObject.CompareTag("Player"))
        {
            isInRange = false; // Сбрасываем флаг
            objectToActivate.SetActive(false); // Деактивируем объект
        }
    }

    void TeleportPlayer()
    {
        if (player != null) // Если игрок существует в сцене
        {
            // Останавливаем движение, чтобы избежать проблем с физикой
            if (playerMovement != null)
            {
                playerMovement.enabled = false; // Отключаем скрипт движения
            }

            // Отключаем CharacterController
            playerController.enabled = false;
            player.transform.position = teleportTarget.position; // Телепортируем игрока
            // Включаем CharacterController
            playerController.enabled = true;

            // Снова включаем скрипт движения
            if (playerMovement != null)
            {
                playerMovement.enabled = true; // Включаем скрипт движения
            }
        }
        else
        {
            Debug.LogError("GameObject с тегом 'Player' не существует в сцене!"); //Выводим ошибку если игрока не существует
        }

        objectToActivate.SetActive(false); // Отключаем объект после телепортации
    }
}