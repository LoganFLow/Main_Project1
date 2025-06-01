using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public GameObject settingsPanel;
    public void StartGame()
    {
        SceneManager.LoadScene(2);
    }
    public void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true); // �������� ������ ��������
        }
    }

    // ����� ��� �������� ������ ��������
    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }
    void Update()
    {
        // ��������� ������� ������� ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // ���� ������ �������� �������, ��������� �
            if (settingsPanel != null && settingsPanel.activeSelf)
            {
                CloseSettings();
            }
        }
    }
    public void ExitToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
