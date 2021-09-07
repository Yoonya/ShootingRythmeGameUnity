using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using BackEnd; //�ڳ� ���� ����

public class Title : MonoBehaviour //����Ÿ��Ʋ
{
    [SerializeField]
    private VideoPlayer videoPlayer0; //����Ÿ��Ʋ
    [SerializeField]
    private VideoPlayer videoPlayer1; //Ÿ��Ʋ ����
    [SerializeField]
    private AudioSource audioSourceBGM; //���
    [SerializeField]
    private AudioSource audioSourceSFX; //��ư sfx

    [SerializeField]
    private GameObject loginUI; //�α���â
    [SerializeField]
    private InputField id = null; //���̵� ��ǲ
    [SerializeField]
    private InputField pwd = null;//��й�ȣ ��ǲ

    private bool isMain = true;

    public string sceneName = "SongSelect"; //�������� �Ѿ ��

    private void Start()
    {
        Backend.Initialize(InitializeCallback); //��� �ʱ�ȭ
    }

    void Update()
    {
        if (!isMain && videoPlayer1.isPlaying && audioSourceBGM.volume != 0f) //�� �̵��� ���� ������ �پ���
        {
            audioSourceBGM.volume -= 0.01f;
            if (audioSourceBGM.volume <= 0.005f)
                audioSourceBGM.volume = 0f;
        }
    }

    void InitializeCallback()
    {
        if (Backend.IsInitialized)
        {
            Debug.Log(Backend.Utils.GetServerTime());//���� �ð�
            Debug.Log(Backend.Utils.GetGoogleHash());//�ڳ� ���� ��ſ� �ʿ��� ���� �ؽ�Ű
        }
        else
            Debug.Log("�ʱ�ȭ ����");
    }

    public void TouchToPlay() //Ŭ������ �α��� â �ҷ�����
    {
        if (isMain) //�ߺ� Ŭ�� ����
        {
            audioSourceSFX.Play();
            isMain = false;

            loginUI.SetActive(true);         
        }
    }

    private IEnumerator SceneLoad()
    {
        videoPlayer1.Play(); //���� ü����
        videoPlayer0.Stop();
        audioSourceSFX.Play();
        yield return new WaitForSeconds(1.5f); //���� ��� �����°� ���� �̵�

        LoadingSceneManager.LoadScene("SongSelect");
    }

    public void BtnSignUp()//ȸ������
    {
        string tempID = id.text;
        string tempPWD = pwd.text;

        BackendReturnObject bro = Backend.BMember.CustomSignUp(tempID, tempPWD);//ȸ������ ȣ��

        if (bro.IsSuccess()) //ȸ������ ���� �޼���
        {
            Debug.Log("ȸ������ ����");
            loginUI.SetActive(false);
            SignUpInsert(tempID);
            StartCoroutine(SceneLoad());
        }
        else
        {
            Debug.Log("ȸ������ ����");
        }
    }

    public void BtnLogin()
    {
        string tempID = id.text;
        string tempPWD = pwd.text;

        BackendReturnObject bro = Backend.BMember.CustomLogin(tempID, tempPWD);//�α��� ȣ��

        if (bro.IsSuccess()) //�α��� ���� �޼���
        {
            Debug.Log("�α��� ����");
            loginUI.SetActive(false);
            StartCoroutine(SceneLoad());
        }
        else
        {
            Debug.Log("�α��� ����");
        }
    }

    private void SignUpInsert(string id) //���� ȸ������
    {
        //rankScore DB�� �ٸ��� �̵��� 1�÷����� ����ؼ� update�� ����ϱ� ������ ���⼭ �̸� �����صд�.
        
        //���� null����� default �� �����߱� ������ �� ������ id�� param�� �����ص�        
        Param param = new Param();
        param.Add("ID", id);

        //������ ���� �� ȯ�漳�� ���� defalut�÷� ����
        Backend.GameData.Insert("status", param);
        //������ �� ���� defalut�÷� ����
        Backend.GameData.Insert("song", param);
        //���â defalut�÷� ����
        Backend.GameData.Insert("result", param);
    }

}
