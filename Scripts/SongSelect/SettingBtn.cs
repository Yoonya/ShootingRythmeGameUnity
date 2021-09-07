using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd; //뒤끝 서버 연결


public class SettingBtn : MonoBehaviour
{
    //setting 들어가는 톱니버튼
    [SerializeField]
    private GameObject settingInterface;
    public bool IsSetting = false;//설정 창 bool 처리

    public void OpenSetting() //설정 창 열기 이벤트
    {
        if (!IsSetting)
        {
            settingInterface.SetActive(true);
            IsSetting = true;
            AudioManager.instance.PlaySFX("Button");
        }
    }
}
