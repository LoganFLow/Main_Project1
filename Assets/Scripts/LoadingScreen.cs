using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    public Slider progressBar; // ������ �� ProgressBar
    public TMP_Text progressText;   // ������ �� ����� ��� ����������� ���������
    public float delayTime = 2f;
    public int scene;

    void Start()
    {
        // ��������� �������� ��� �������� �����
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        // �������� �������� ��������� ����� (��������, ����� � �������� 1)
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);

        // ���������, ��� ����� �� ������������ �� ���������� ��������
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            // ��������� ProgressBar � �����
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.value = progress;
            progressText.text = (progress * 100f).ToString("F0") + "%";

            // ���� �������� ���������, ���� ��������� �����
            if (operation.progress >= 0.9f)
            {
                progressBar.value = 1f;
                progressText.text = "100%";

                // ��������� �������� ����� ���������� �����
                yield return new WaitForSeconds(delayTime);

                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}