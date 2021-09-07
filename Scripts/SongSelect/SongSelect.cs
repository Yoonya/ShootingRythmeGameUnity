using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using BackEnd; //�ڳ� ���� ����

[System.Serializable]
public class Song //�� ����
{
    public int number; //��������
    public string name; //�̸�
    public Sprite sprite; //�̹���
    public AudioClip audioClip; //�̸���� ��
    public Song(int number, string name, Sprite sprite, AudioClip audioClip)
    {
        this.number = number;
        this.name = name;
        this.sprite = sprite;
        this.audioClip = audioClip;
    }
}

public class SongSelect : MonoBehaviour
{
    [SerializeField] private Song[] songList; //�� Ŭ���� ����Ʈ
    [SerializeField] private Image[] songImages; //��, ��, �߰� �̹���
    [SerializeField] public Text currentSongText; //���� �ؽ�Ʈ
    [SerializeField] public Text currentTopScoreText; //�ְ����� �ؽ�Ʈ
    private int[] rankScore = new int[3]; //�ڽ��� �� �ְ�����, 3�� ���� �� ����->��� ������Ʈ

    [SerializeField] private AudioSource audioSource; //�������
    [SerializeField] private VideoPlayer videoPlayer; //�޹��

    private string id;

    private SettingBtn settingBtn;

    public int currentNumber = 0; //���� ���õǰ� �ִ� �ε��� 

    void Start()
    {
        settingBtn = FindObjectOfType<SettingBtn>();

        for (int i = 0; i < songList.Length; i++) //���� �ʱ�ȭ
        {
            rankScore[i] = 0;
        }
        ConfirmRankScore(); //�� ���ÿ��� �ڽ��� �ְ����� ����� ���� ���� �޾ƿ���
        currentTopScoreText.text = rankScore[0].ToString(); //�ְ����� �ؽ�Ʈ

        //���� �ʱ�ȭ
        currentSongText.text = songList[0].name;
        songImages[0].sprite = songList[0].sprite; //���
        songImages[1].sprite = songList[1].sprite; //������
        songImages[2].sprite = songList[songList.Length-1].sprite; //����
        audioSource.clip = songList[0].audioClip; //���� �� ����
        audioSource.Play(); //�̸���� ��
        videoPlayer.Play(); //��� ����
    }

    public void BtnRightArrow() //������ ȭ��ǥ��ư
    {
        if (!settingBtn.IsSetting) //ȯ�漳�� â�� ���� �ִ� ���¿�����
        {
            currentNumber++;
            if (currentNumber > songList.Length - 1)
                currentNumber = 0;

            currentSongText.text = songList[currentNumber].name;
            songImages[0].sprite = songList[currentNumber].sprite;
            currentTopScoreText.text = rankScore[currentNumber].ToString(); //�ְ����� �ؽ�Ʈ

            if (currentNumber == songList.Length - 1)
                songImages[1].sprite = songList[0].sprite;
            else
                songImages[1].sprite = songList[currentNumber + 1].sprite;

            if (currentNumber == 0)
                songImages[2].sprite = songList[songList.Length - 1].sprite;
            else
                songImages[2].sprite = songList[currentNumber - 1].sprite;

            audioSource.clip = songList[currentNumber].audioClip; //�̸���� �缳��
            audioSource.Play();
            AudioManager.instance.PlaySFX("Slide");
        }
    }

    public void BtnLeftArrow() //���� ȭ��ǥ ��ư
    {
        if (!settingBtn.IsSetting) //ȯ�漳�� â�� ���� �ִ� ���¿�����
        {
            currentNumber--;
            if (currentNumber < 0)
                currentNumber = songList.Length - 1;

            currentSongText.text = songList[currentNumber].name;
            songImages[0].sprite = songList[currentNumber].sprite;
            currentTopScoreText.text = rankScore[currentNumber].ToString(); //�ְ����� �ؽ�Ʈ

            if (currentNumber == songList.Length - 1)
                songImages[1].sprite = songList[0].sprite;
            else
                songImages[1].sprite = songList[currentNumber + 1].sprite;

            if (currentNumber == 0)
                songImages[2].sprite = songList[songList.Length - 1].sprite;
            else
                songImages[2].sprite = songList[currentNumber - 1].sprite;

            audioSource.clip = songList[currentNumber].audioClip; //�̸���� �缳��
            audioSource.Play();
            AudioManager.instance.PlaySFX("Slide");
        }
    }

    public void BtnSongSelect()//�� ���� ��ư �̺�Ʈ
    {
        if (!settingBtn.IsSetting) //ȯ�漳�� â�� ���� �ִ� ���¿�����
        {
            BtnSongSelectUpdate();
            AudioManager.instance.PlaySFX("Button");
            StartCoroutine(SceneLoad());
        }
    }

    private void BtnSongSelectUpdate() //������ �� ���� ������ ������ ����
    {
        ConfirmStatus();

        Param param = new Param();
        param.Add("songName", songList[currentNumber].name);
        param.Add("songNumber", currentNumber.ToString());

        Where where = new Where(); //id�� ���� ����
        where.Equal("ID", id);

        Backend.GameData.Update("song", where, param); //update
    }

    private void ConfirmStatus() //�������� status DB�κ��� ���� �޾ƿ���, ���⼭�� id���� �ʿ�
    {
        var bro = Backend.GameData.GetMyData("status", new Where(), 10);
        if (bro.IsSuccess() == false)
        {
            // ��û ���� ó��
            Debug.Log(bro);
            return;
        }
        if (bro.GetReturnValuetoJSON()["rows"].Count <= 0)
        {
            // ��û�� �����ص� where ���ǿ� �����ϴ� �����Ͱ� ���� �� �ֱ� ������
            // �����Ͱ� �����ϴ��� Ȯ��
            // ���� ���� new Where() ������ ��� ���̺� row�� �ϳ��� ������ Count�� 0 ���� �� �� �ִ�.
            Debug.Log(bro);
            return;
        }
        // �˻��� �������� ��� row�� inDate �� Ȯ��
        for (int i = 0; i < bro.Rows().Count; ++i)
        {
            string tempID = bro.Rows()[i]["ID"]["S"].ToString();

            //������ ����
            id = tempID;
        }
    }

    private void ConfirmRankScore() //�������� rankScore DB�κ��� ���� �޾ƿ���
    {
        var bro = Backend.GameData.GetMyData("rankScore", new Where(), 10);
        if (bro.IsSuccess() == false)
        {
            // ��û ���� ó��
            Debug.Log(bro);
            return;
        }
        if (bro.GetReturnValuetoJSON()["rows"].Count <= 0)
        {
            // ��û�� �����ص� where ���ǿ� �����ϴ� �����Ͱ� ���� �� �ֱ� ������
            // �����Ͱ� �����ϴ��� Ȯ��
            // ���� ���� new Where() ������ ��� ���̺� row�� �ϳ��� ������ Count�� 0 ���� �� �� �ִ�.
            Debug.Log(bro);
            return;
        }
        // �˻��� �������� ��� row�� inDate �� Ȯ��
        for (int i = 0; i < bro.Rows().Count; ++i)
        {
            string tempSongNumber = bro.Rows()[i]["songNumber"]["S"].ToString();
            string tempTopScore = bro.Rows()[i]["topScore"]["S"].ToString();

            //������ ����
            rankScore[int.Parse(tempSongNumber)] = int.Parse(tempTopScore);
        }
    }

    private IEnumerator SceneLoad() //����â �ҷ�����
    {
        yield return new WaitForSeconds(1f);

        LoadingSceneManager.LoadScene("Game");
        gameObject.GetComponentInChildren<VideoPlayer>().enabled = false; //�ı����� �ʰ� �������� ������ ��Ȱ��ȭ �ӽ�
        gameObject.GetComponentInChildren<AudioSource>().enabled = false;
        gameObject.GetComponentInChildren<Text>().enabled = false;
    }

    public void SceneBack()
    {
        //�ٸ� ������ ���ƿ� ���
        gameObject.GetComponentInChildren<VideoPlayer>().enabled = true; //�ı����� �ʰ� �������� ������ Ȱ��ȭ  �ӽ�
        gameObject.GetComponentInChildren<AudioSource>().enabled = true;
        gameObject.GetComponentInChildren<Text>().enabled = true;
        audioSource.Play();
        videoPlayer.Play();
    }

}
