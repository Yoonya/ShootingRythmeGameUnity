using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    //�ε��� �� ����
    public static string nextScene;

    [SerializeField] Image progressBar; //���� ��

    private void Start()
    {
        StartCoroutine(LoadScene()); //�ε�� ����
    }

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene"); //�ε������� �̵�
    }

    IEnumerator LoadScene()
    {
        yield return null;

        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false; //���� �ε��� ������ �ڵ����� �ҷ��� ������ �̵��� ������ ����->false�� 90%�� �ε��ϰ� ���

        float timer = 0.0f;
        while (!op.isDone) //�ε��� ������ ������
        {
            yield return null;

            timer += Time.deltaTime;
            if (op.progress < 0.9f) //90%��� ���̱� ������
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, timer);
                if (progressBar.fillAmount >= op.progress)
                {
                    timer = 0f;
                }
            }
            else //����ũ �ε�
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer); //0.9���� 1���� 1�� ���� ä�쵵��
                if (progressBar.fillAmount == 1.0f)
                {
                    op.allowSceneActivation = true; //������ �ҷ�����
                    yield break;
                }
            }
        }

    }

}
