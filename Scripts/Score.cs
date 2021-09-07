using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    //점수 관리
    [SerializeField]
    private Text score;

    public int scorePoint = 0;
  
    public void SetScore(int damage, float overDamage) //스코어 계산, 데미지 방식
    {
        float tempScorePoint = damage * 20;
        tempScorePoint = tempScorePoint + (tempScorePoint * overDamage / 100); //추가점수계산

        scorePoint += (int)Mathf.Round(tempScorePoint);
        
        score.text = scorePoint.ToString();
    }

    public void SetScore(int point) //스코어 계산, 추가 점수 방식
    {
        scorePoint += point;
        score.text = scorePoint.ToString();
    }

}
