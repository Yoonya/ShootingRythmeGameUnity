using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd; //�ڳ� ���� ����


public class SettingBtn : MonoBehaviour
{
    //setting ���� ��Ϲ�ư
    [SerializeField]
    private GameObject settingInterface;
    public bool IsSetting = false;//���� â bool ó��

    public void OpenSetting() //���� â ���� �̺�Ʈ
    {
        if (!IsSetting)
        {
            settingInterface.SetActive(true);
            IsSetting = true;
            AudioManager.instance.PlaySFX("Button");
        }
    }
}
