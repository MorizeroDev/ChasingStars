using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class TestUI : UIController
{
    public override void Click()
    {
        if (this.name == "wey")
        {
            Dramas.PopupDialog("Wey", "�������Ҳ��š�", () =>
            {
                uibase.focuser.PlayExit();
            });
            return;
        }
        Debug.Log("UIԪ��" + uibase.id + "�����");
        uibase.focuser.PlayExit();
    }
}
