using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
public MouseSensitivity mouseSensitivity; // ������ �� ������ ���������� ����������������� ����
public float rotationSpeed = 100f; // �������� ��������

void Start()
{
    // ������������� ������ � ������ ������ � ������� ��� ���������
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;

    if (mouseSensitivity == null)
    {
        Debug.LogError("MouseSensitivity ������ �� ������!");
    }
}

void Update()
{
    // �������� ���� � ����
    float mouseX = Input.GetAxis("Mouse X");

    // �������� ������� ���������������� ����
    float sensitivity = mouseSensitivity.GetSensitivity();

    // ��������� ���� �������� � ������ ����������������
    float rotationAngle = mouseX * rotationSpeed * sensitivity * Time.deltaTime;

    // ��������� ������� ������ �� ��� Y
    transform.Rotate(Vector3.up, rotationAngle);
}

void OnApplicationFocus(bool hasFocus)
{
    if (hasFocus)
    {
        // ������������� ������ � ������ ������ � ������� ��� ���������, ����� ���������� �������� �����
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    else
    {
        // ������� ������ �������, ����� ���������� ������ �����
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
}