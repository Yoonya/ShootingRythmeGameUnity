using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    //���� ����
    [SerializeField]
    private Text score;

    public int scorePoint = 0;
  
    public void SetScore(int damage, float overDamage) //���ھ� ���, ������ ���
    {
        float tempScorePoint = damage * 20;
        tempScorePoint = tempScorePoint + (tempScorePoint * overDamage / 100); //�߰��������

        scorePoint += (int)Mathf.Round(tempScorePoint);
        
        score.text = scorePoint.ToString();
    }

    public void SetScore(int point) //���ھ� ���, �߰� ���� ���
    {
        scorePoint += point;
        score.text = scorePoint.ToString();
    }

}
