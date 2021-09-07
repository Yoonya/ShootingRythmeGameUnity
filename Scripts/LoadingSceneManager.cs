using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    //로딩간 씬 관리
    public static string nextScene;

    [SerializeField] Image progressBar; //진행 바

    private void Start()
    {
        StartCoroutine(LoadScene()); //로드씬 시작
    }

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene"); //로딩씬으로 이동
    }

    IEnumerator LoadScene()
    {
        yield return null;

        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false; //씬의 로딩이 끝나면 자동으로 불러온 씬으로 이동할 것인지 설정->false시 90%만 로드하고 대기

        float timer = 0.0f;
        while (!op.isDone) //로딩이 끝나지 않으면
        {
            yield return null;

            timer += Time.deltaTime;
            if (op.progress < 0.9f) //90%대기 중이기 때문에
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, timer);
                if (progressBar.fillAmount >= op.progress)
                {
                    timer = 0f;
                }
            }
            else //페이크 로딩
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer); //0.9에서 1까지 1초 동안 채우도록
                if (progressBar.fillAmount == 1.0f)
                {
                    op.allowSceneActivation = true; //나머지 불러오기
                    yield break;
                }
            }
        }

    }

}
