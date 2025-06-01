using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
public MouseSensitivity mouseSensitivity; // Ссылка на скрипт управления чувствительностью мыши
public float rotationSpeed = 100f; // Скорость поворота

void Start()
{
    // Заблокировать курсор в центре экрана и сделать его невидимым
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;

    if (mouseSensitivity == null)
    {
        Debug.LogError("MouseSensitivity скрипт не найден!");
    }
}

void Update()
{
    // Получаем ввод с мыши
    float mouseX = Input.GetAxis("Mouse X");

    // Получаем текущую чувствительность мыши
    float sensitivity = mouseSensitivity.GetSensitivity();

    // Вычисляем угол поворота с учетом чувствительности
    float rotationAngle = mouseX * rotationSpeed * sensitivity * Time.deltaTime;

    // Применяем поворот только по оси Y
    transform.Rotate(Vector3.up, rotationAngle);
}

void OnApplicationFocus(bool hasFocus)
{
    if (hasFocus)
    {
        // Заблокировать курсор в центре экрана и сделать его невидимым, когда приложение получает фокус
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    else
    {
        // Сделать курсор видимым, когда приложение теряет фокус
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
}