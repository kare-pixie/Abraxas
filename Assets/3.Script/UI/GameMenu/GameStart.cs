using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{
    [SerializeField] private Slider progressBar; // 진행 상황을 표시할 슬라이더
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

        // 비동기 로드 시작
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        asyncLoad.allowSceneActivation = false; // 자동으로 씬 전환되지 않도록 설정

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            progressBar.value = progress;

            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true; // 씬 전환 허용
            }

            yield return null;
        }
    }
}
