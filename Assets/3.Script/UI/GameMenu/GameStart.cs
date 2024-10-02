using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{
    [SerializeField] private Slider progressBar; // ���� ��Ȳ�� ǥ���� �����̴�
    [SerializeField] private GameObject gameStartBtn;
    [SerializeField] private GameObject gameEntBtn;
    public void StartLoading(int sceneIndex)
    {
        StartCoroutine(LoadSceneAsync(sceneIndex));
    }
    public IEnumerator LoadSceneAsync(int sceneIndex)
    {
        gameStartBtn.SetActive(false);
        gameEntBtn.SetActive(false);
        progressBar.gameObject.SetActive(true);

        // �񵿱� �ε� ����
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        asyncLoad.allowSceneActivation = false; // �ڵ����� �� ��ȯ���� �ʵ��� ����

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            progressBar.value = progress;

            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true; // �� ��ȯ ���
            }

            yield return null;
        }
    }
}
