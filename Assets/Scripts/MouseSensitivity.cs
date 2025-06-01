using UnityEngine;
using UnityEngine.UI;

public class MouseSensitivity : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider sensitivitySlider;

    [Header("Mouse Settings")]
    public float defaultSensitivity = 2.0f;
    public float minSensitivity = 0.1f;
    public float maxSensitivity = 10.0f;

    private float currentSensitivity;

    void Start()
    {
        // ��������� ����������� ���������������� ��� ������������� �������� �� ���������
        currentSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", defaultSensitivity);

        // ����������� �������
        sensitivitySlider.minValue = minSensitivity;
        sensitivitySlider.maxValue = maxSensitivity;
        sensitivitySlider.value = currentSensitivity;

        // ��������� ��������� ��� ��������� �������� ��������
        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
    }

    void OnSensitivityChanged(float value)
    {
        currentSensitivity = value;
        PlayerPrefs.SetFloat("MouseSensitivity", currentSensitivity);
    }

    public float GetSensitivity()
    {
        return currentSensitivity;
    }
}