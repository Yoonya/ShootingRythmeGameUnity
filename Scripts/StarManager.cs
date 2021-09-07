using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class go_Star //노트설정
{
    public int starType = 0;
    public int starLocation = 0;
    public float appearTime = 0f;
    public go_Star(int starType, int starLocation, float appearTime)
    {
        this.starType = starType;
        this.starLocation = starLocation;
        this.appearTime = appearTime;
    }
}

public class StarManager : MonoBehaviour
{
    private int starSpeed;
    [SerializeField] private float sync0 = 0f; //추가 싱크(개발자 전용)
    private float sync1; //추가 싱크(사용자 설정)
    private float sync2; //기본싱크

    [SerializeField]
    public Transform[] starLocation = null;

    private List<go_Star> stars = new List<go_Star>();
    private GameManager gameManager;
    private SongNoteManager songNoteManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        songNoteManager = FindObjectOfType<SongNoteManager>();

        starSpeed = (int)(gameManager.speed * 1000);
        sync1 = gameManager.sync;
        sync2 = starLocation[0].transform.localPosition.y / starSpeed;

        float tempSync = (starSpeed - 2000.0f) / 5000.0f; //세밀계산
        sync0 += tempSync;
        sync0 += 0.6f; //추가계산

        ReadStarInfo();
        //모든 노트를 정해진 시간에 출발하도록 설정
        for (int i = 0; i < stars.Count; i++)
        {
            StartCoroutine(StartMakeStar(stars[i]));
        }
    }

    private void ReadStarInfo()   //리소스에서 노트 텍스트 파일을 불러오기
    {
        if (songNoteManager.songNotes[gameManager.currentSongNumber].stars != "")
        {
            string[] texts = songNoteManager.songNotes[gameManager.currentSongNumber].stars.Split('\n');

            if (texts.Length > 0) //메모장에 있는 노트 리스트로 정리
            {
                for (int i = 0; i < texts.Length; i++)
                {
                    int tempType = Convert.ToInt32(texts[i].Split(' ')[0]);
                    int tempLocation = Convert.ToInt32(texts[i].Split(' ')[1]);
                    //판정라인에 도달할 시간을 빼서 메모장에 적은 시간에 도착하도록 계산(싱크)
                    float tempAppear = Convert.ToSingle(texts[i].Split(' ')[2]) - sync2 + sync1 + sync0;

                    go_Star tempStar = new go_Star(tempType, tempLocation, tempAppear);
                    stars.Add(tempStar);
                }
            }
        }
    }

    IEnumerator StartMakeStar(go_Star star)
    {
        yield return CoroutineManager.YieldInstructionCache.WaitForSeconds(star.appearTime);
        CreateStar(star);
    }

    private void CreateStar(go_Star star) //스타생성
    {
        GameObject tempStar = ObjectPool.instance.queue[star.starType + 9].Dequeue(); //오브젝트 풀링 사용, 스타위치는 9부터

        tempStar.transform.localScale = new Vector3(1f, 1f, 1f); 
        tempStar.GetComponent<Star>().SetStarSpeed(starSpeed); //스타 속도 설정
        tempStar.GetComponent<Star>().SetStarLocation(star.starLocation); //스타 위치 설정
        tempStar.GetComponent<Star>().SetAppearTime(star.appearTime); //스타 시간 설정
        tempStar.GetComponent<Star>().SetStarType(star.starType); //스타 타입 설정
        tempStar.GetComponent<Image>().enabled = true; //캐릭터 피격모션 중에 false할 때가 있는데 혹시 모르니 초기화

        tempStar.transform.position = starLocation[star.starLocation].position;
        tempStar.SetActive(true);
    }

    private void OnTriggerEnter(Collider other) //쓰레기통
    {
        if (other.CompareTag("Star"))
        {
            ObjectPool.instance.queue[other.GetComponent<Star>().starType + 9].Enqueue(other.gameObject);//오브젝트풀링 집어넣기
            other.gameObject.SetActive(false);
        }
    }
}
