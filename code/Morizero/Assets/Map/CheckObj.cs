using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckObj : MonoBehaviour
{
    public static bool TriggerRunning = false;
    public enum TriggerType
    {
        [InspectorName("��������")]
        Passive,
        [InspectorName("����ʱ��������")]
        OnceTrigger,
        [InspectorName("����󱻶�����")]
        PassiveTrigger,
        [InspectorName("ʱʱ�̴̿���")]
        Whenever
    }
    [Tooltip("���ӵľ籾�ű���")]
    public TextAsset Script;
    [Tooltip("��֮��ϵ��NPC�����ھ������ꡣ")]
    public Chara BindChara;
    [HideInInspector]
    public DramaScript scriptCarrier = new DramaScript();
    [HideInInspector]
    public bool Sleep = false;
    public static bool CheckBtnPressed;
    public static bool CheckAvaliable = false;
    [Tooltip("�Ƿ������������Ľű����ж�����")]
    public bool JudgeDirection = false;
    [Tooltip("�Ƿ����������ײ������������Script��")]
    public TriggerType triggerType = TriggerType.Passive;
    private static float checkshowTime;
    [Tooltip("�����������ʱ�泯�ķ���")]
    public List<Chara.walkDir> AllowDirection = new List<Chara.walkDir>(3);
    [Tooltip("������������ͣ�0Ϊ�򵥵��飬1ΪDramaԤ���塣\n��Script��Ϊnullʱ�����趨��Ч��")]
    public int CheckType = 0;
    [Tooltip("��ScriptΪ�յ�����´����ĵ�������/Ԥ�������ơ�")]
    public string Content;
    public void CheckEncounter(){
        if(AllowDirection.Contains(MapCamera.Player.dir) || triggerType != TriggerType.Passive){
            MapCamera.HitCheck = this.gameObject;
            MapCamera.HitCheckTransform = this.transform.parent;
            if(CheckType == 0) {
                MapCamera.mcamera.CheckText.sprite = MapCamera.mcamera.CheckFore;
                MapCamera.mcamera.CheckImg.sprite = MapCamera.mcamera.CheckBack;
            }
            if(CheckType == 1) {
                MapCamera.mcamera.CheckText.sprite = MapCamera.mcamera.TalkFore;
                MapCamera.mcamera.CheckImg.sprite = MapCamera.mcamera.TalkBack;
            }
            MapCamera.mcamera.checkHint.SetActive(true);
            checkshowTime = Time.time;
            MapCamera.mcamera.animator.SetFloat("speed",1.0f);
            MapCamera.mcamera.animator.Play("CheckBtn",0,0f);
            CheckAvaliable = true;
        }
    }
    public void CheckGoodbye(){
        MapCamera.HitCheck = null;
        MapCamera.mcamera.animator.SetFloat("speed",-2.0f);
        // �������������ʾ��ʱ��̫�̵Ļ���ֱ������
        if(Time.time - checkshowTime <= 0.6f)
        {
            MapCamera.mcamera.animator.Play("CheckBtn", 0, 0f);
        }
        else
        {
            MapCamera.mcamera.animator.Play("CheckBtn", 0, 1f);
        }
        CheckAvaliable = false;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (triggerType != TriggerType.PassiveTrigger && triggerType != TriggerType.OnceTrigger) return;
        if (MapCamera.Player == null) return;
        Chara c = null;
        if (collision.gameObject.transform.parent == null) return;
        if (collision.gameObject.transform.parent.TryGetComponent<Chara>(out c))
        {
            if (!c.Controller) return;
        }
        else
        {
            return;
        }
        if (triggerType != TriggerType.Passive && !AllowDirection.Contains(MapCamera.Player.dir) && JudgeDirection) return;
        if (MapCamera.HitCheck == this.gameObject) return;
        if (MapCamera.HitCheck != null) MapCamera.HitCheck.GetComponent<CheckObj>().CheckGoodbye();
        CheckEncounter();
        TriggerRunning = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (MapCamera.HitCheck != this.gameObject) return;
        if (collision == null)
        {
            Sleep = false;
            TriggerRunning = false;
            CheckGoodbye();
            return;
        }
        if (triggerType != TriggerType.PassiveTrigger && triggerType != TriggerType.OnceTrigger) return;
        if (MapCamera.Player == null) return;
        Chara c = null;
        if (collision.gameObject.transform.parent == null) return;
        if (collision.gameObject.transform.parent.TryGetComponent<Chara>(out c))
        {
            if (!c.Controller) return;
        }
        else
        {
            return;
        }
        if (MapCamera.HitCheck == this.gameObject)
        {
            Sleep = false;
            TriggerRunning = false;
            CheckGoodbye();
        }
    }
    private void OnDestroy()
    {
        if(MapCamera.HitCheck == this.gameObject)
            CheckGoodbye();
    }
    public bool IsActive(){
        if (Sleep) return false;
        if(MapCamera.HitCheck != this.gameObject && triggerType != TriggerType.Whenever) return false;
        bool ret = (Input.GetKeyUp(KeyCode.Z) || CheckBtnPressed || triggerType == TriggerType.OnceTrigger || triggerType == TriggerType.Whenever);
        if(MapCamera.SuspensionDrama) ret = false;
        CheckBtnPressed = false;
        return ret;
    }
    
    private void Awake() {
        scriptCarrier.parent = this;
    }

    public virtual void Update() {
        if(!IsActive()) return;

        MapCamera.SuspensionDrama = true;
        MapCamera.Player.ClosePadAni();
        if (Script != null){
            scriptCarrier.code = Script.text.Split(new string[]{"\r\n"},System.StringSplitOptions.RemoveEmptyEntries);
            for(int i = 0;i < scriptCarrier.code.Length;i++){
                scriptCarrier.code[i] = scriptCarrier.code[i].TrimStart();
            }
            scriptCarrier.currentLine = 0;
            DramaScript.Active = scriptCarrier;
            // ������������
            if (BindChara != null)
            {
                Chara p = MapCamera.Player;
                Chara.walkDir dir = p.dir;
                bool needFix = false;
                if (p.dir == Chara.walkDir.Down || p.dir == Chara.walkDir.Up)
                {
                    if(Mathf.Abs(p.transform.localPosition.x - BindChara.transform.localPosition.x) > 0.25f)
                    {
                        p.walkTasks.Enqueue(Chara.walkTask.fromRaw(BindChara.transform.localPosition.x, p.transform.localPosition.y));
                        needFix = true;
                    }
                }
                else
                {
                    if (Mathf.Abs(p.transform.localPosition.y - BindChara.transform.localPosition.y) > 0.25f)
                    {
                        p.walkTasks.Enqueue(Chara.walkTask.fromRaw(p.transform.localPosition.x, BindChara.transform.localPosition.y));
                        needFix = true;
                    }
                }
                if (needFix)
                {
                    p.walkTaskCallback = () => {
                        p.walkTaskCallback = null;
                        WaitTicker.Create(0.5f, () =>
                        {
                            p.dir = dir;
                            p.UpdateWalkImage();
                            WaitTicker.Create(0.5f, scriptCarrier.carryTask);
                        });
                    };
                }
                else
                {
                    scriptCarrier.carryTask();
                }
            }
            else
            {
                scriptCarrier.carryTask();
            }
            return; 
        }
        if(CheckType == 1){
            Dramas.Launch(Content,() => {
                if(!Settings.Active && !Settings.Loading) MapCamera.SuspensionDrama = false;
            });
        }else{
            Dramas.LaunchCheck(Content,() => {
                if (!Settings.Active && !Settings.Loading) MapCamera.SuspensionDrama = false;
            }).LifeTime = Dramas.DramaLifeTime.DieWhenReadToEnd;
        }
    }
}
