using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    public Slider progressBar; // Ссылка на ProgressBar
    public TMP_Text progressText;   // Ссылка на текст для отображения прогресса
    public float delayTime = 2f;
    public int scene;

    void Start()
    {
        // Запускаем корутину для загрузки сцены
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        // Начинаем загрузку следующей сцены (например, сцены с индексом 1)
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);

        // Убедитесь, что сцена не активируется до завершения загрузки
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            // Обновляем ProgressBar и текст
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.value = progress;
            progressText.text = (progress * 100f).ToString("F0") + "%";

            // Если загрузка завершена, ждем некоторое время
            if (operation.progress >= 0.9f)
            {
                progressBar.value = 1f;
                progressText.text = "100%";

                // Добавляем задержку перед активацией сцены
                yield return new WaitForSeconds(delayTime);

                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}