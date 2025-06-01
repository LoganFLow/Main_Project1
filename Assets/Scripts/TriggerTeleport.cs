using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TriggerTeleport: MonoBehaviour
{
    [Header("Teleport Settings")]
    public Transform teleportTarget; // �����, ���� ��������������� �����
    public GameObject objectToActivate; // ������, ������� ����� ������������ ��� ����� � �������
    public KeyCode teleportKey = KeyCode.E; // ������� ��� ������������

    private GameObject player; // ������ �� ������ �� ����
    private bool isInRange = false; // ����, �����������, ��������� �� ����� � ���� ��������
    private CharacterController playerController; // ������ �� CharacterController ������
    private PlayerMove playerMovement; // ������ �� ������ �������� ������

    void Start()
    {
        //���� ������ � �����
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("�� ������ GameObject � ����� 'Player' � �����!");
            enabled = false;
            return;
        }

        playerController = player.GetComponent<CharacterController>();
        if (playerController == null)
        {
            Debug.LogError("Player �� ����� ���������� CharacterController!");
            enabled = false;
            return;
        }

        playerMovement = player.GetComponent<PlayerMove>();
        if (playerMovement == null)
        {
            Debug.LogWarning("� ������ �� ������ ������ GTAStyleMovement. ������������ ����� �������� �����������.");
        }


        // �������� ������� �������� �������
        if (teleportTarget == null)
        {
            Debug.LogError("�� ������ Teleport Target! ����������, ������� ������ � ����������.");
            enabled = false;
            return;
        }

        // �������� ������� ������� ��� ���������
        if (objectToActivate == null)
        {
            Debug.LogError("�� ������ Object To Activate! ����������, ������� ������ � ����������.");
            enabled = false;
            return;
        }

        // ��������� ������ ��� ������
        objectToActivate.SetActive(false);
    }

    void Update()
    {
        // ���� ����� ��������� � ���� �������� � ����� ������� ������������
        if (isInRange && Input.GetKeyDown(teleportKey))
        {
            TeleportPlayer();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // ���� � ������� ����� �����
        if (other.gameObject.CompareTag("Player"))
        {
            isInRange = true; // ������������� ����, ��� ����� � ���� ��������
            objectToActivate.SetActive(true); // ���������� ������
        }
    }

    void OnTriggerExit(Collider other)
    {
        // ���� ����� ����� �� ��������
        if (other.gameObject.CompareTag("Player"))
        {
            isInRange = false; // ���������� ����
            objectToActivate.SetActive(false); // ������������ ������
        }
    }

    void TeleportPlayer()
    {
        if (player != null) // ���� ����� ���������� � �����
        {
            // ������������� ��������, ����� �������� ������� � �������
            if (playerMovement != null)
            {
                playerMovement.enabled = false; // ��������� ������ ��������
            }

            // ��������� CharacterController
            playerController.enabled = false;
            player.transform.position = teleportTarget.position; // ������������� ������
            // �������� CharacterController
            playerController.enabled = true;

            // ����� �������� ������ ��������
            if (playerMovement != null)
            {
                playerMovement.enabled = true; // �������� ������ ��������
            }
        }
        else
        {
            Debug.LogError("GameObject � ����� 'Player' �� ���������� � �����!"); //������� ������ ���� ������ �� ����������
        }

        objectToActivate.SetActive(false); // ��������� ������ ����� ������������
    }
}