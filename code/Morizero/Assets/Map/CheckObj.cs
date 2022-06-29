using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckObj : MonoBehaviour
{
    public bool SteppedIn = false;
    public static List<CheckObj> SteppedInCheckObj = new List<CheckObj>();
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
    public enum CheckDisplayType
    {
        Object,NPC
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
    public CheckDisplayType DisplayType = 0;
    private bool ScriptRunning = false;
    public bool UseContentAsDramaPrefabName = false;
    [Tooltip("��ScriptΪ�յ�����´����ĵ�������/Ԥ�������ơ�")]
    public string Content;
    [Tooltip("��Ĭ�ű����Ὣ�Լ�Ͷ�䵽���鰴ť֮�С�")]
    public bool Silent = false;
    public void SetToAvaliableCheck(){
        MapCamera.AvaliableCheck = this.gameObject;
        MapCamera.AvaliableCheckTransform = this.transform.parent;
        if(DisplayType == CheckDisplayType.Object) {
            MapCamera.mcamera.CheckText.sprite = MapCamera.mcamera.CheckFore;
            MapCamera.mcamera.CheckImg.sprite = MapCamera.mcamera.CheckBack;
        }
        if(DisplayType == CheckDisplayType.NPC) {
            MapCamera.mcamera.CheckText.sprite = MapCamera.mcamera.TalkFore;
            MapCamera.mcamera.CheckImg.sprite = MapCamera.mcamera.TalkBack;
        }
        MapCamera.mcamera.checkHint.SetActive(true);
        checkshowTime = Time.time;
        MapCamera.mcamera.animator.SetFloat("speed",1.0f);
        MapCamera.mcamera.animator.Play("CheckBtn",0,0f);
        CheckAvaliable = true;
    }
    public void EmptyAvaliableCheck(){
        MapCamera.AvaliableCheck = null;
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
    public bool IsReadyToSet(Chara.walkDir dir)
    {
        if (Silent || Sleep) return false;
        switch (triggerType)
        {
            case TriggerType.Passive:
                return AllowDirection.Contains(dir);
            case TriggerType.PassiveTrigger:
                if (JudgeDirection)
                {
                    return AllowDirection.Contains(dir) && SteppedIn;
                }
                else
                {
                    return SteppedIn;
                }
            case TriggerType.Whenever:
            case TriggerType.OnceTrigger:
                return false;
        }
        return false;
    }
    public static void SearchAvaliablePassiveCheck(Vector3 pos, Chara.walkDir dir)
    {
        // �ж�����
        Vector2 spyRay = new Vector2(pos.x, pos.y);
        if (dir == Chara.walkDir.Left)
        {
            spyRay.x -= 0.48f;
        }
        else if (dir == Chara.walkDir.Right)
        {
            spyRay.x += 0.48f;
        }
        else if (dir == Chara.walkDir.Up)
        {
            spyRay.y += 0.48f;
        }
        else
        {
            spyRay.y -= 0.48f;
        }
        CheckObj AvaliableCheck = null;
        if (SteppedInCheckObj.Count > 0)
        {
            //Debug.Log("�Ƚ���������ڡ�");
            foreach(CheckObj c in SteppedInCheckObj)
            {
                if (c.IsReadyToSet(dir))
                {
                    //Debug.Log(c.name + "����Ͷ�䵽���鰴ť��");
                    AvaliableCheck = c;
                }
            }
        }
        if(AvaliableCheck == null)
        {
            CheckObj c = null;
            foreach (RaycastHit2D crash in Physics2D.RaycastAll(spyRay, new Vector2(0, 0)))
            {
                if (crash.collider.gameObject.TryGetComponent<CheckObj>(out c))
                {
                    //Debug.Log(c.name + "�ڵ�������֮�ڡ�");
                    if (c.IsReadyToSet(dir))
                    {
                        //Debug.Log(c.name + "����Ͷ�䵽���鰴ť��");
                        AvaliableCheck = c;
                        break;
                    }
                }
            }
        }

        if (AvaliableCheck == null)
        {
            if (MapCamera.AvaliableCheck != null)
                MapCamera.AvaliableCheck.GetComponent<CheckObj>().EmptyAvaliableCheck();
        }else if(MapCamera.AvaliableCheck != AvaliableCheck.gameObject)
        {
            //Debug.Log("��Ͷ�����嵽���鰴ť��");
            AvaliableCheck.SetToAvaliableCheck();
        }
    }
    public static bool IsCollisionFromPlayer(Collider2D collision)
    {
        return (collision.gameObject.layer == 6);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!IsCollisionFromPlayer(collision)) return;
        if (SteppedInCheckObj.Contains(this)) return;
        SteppedIn = true;  SteppedInCheckObj.Add(this);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!SteppedInCheckObj.Contains(this)) return;
        SteppedIn = false;  SteppedInCheckObj.Remove(this);
    }
    private void OnDestroy()
    {
        if(MapCamera.AvaliableCheck == this.gameObject) EmptyAvaliableCheck();
    }
    public Chara.walkDir GetPlayerDir()
    {
        if (MapCamera.Player == null) return Chara.walkDir.Down;
        return MapCamera.Player.dir;
    }
    public bool IsActive(){
        if (Sleep) return false;
        bool Pressed = (Input.GetKeyUp(KeyCode.Z) || Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.Space) || CheckBtnPressed);
        bool Avaliable = (MapCamera.AvaliableCheck == this.gameObject);
        switch (triggerType)
        {
            case TriggerType.Passive:
                return Pressed && Avaliable;
            case TriggerType.PassiveTrigger:
                return Pressed && Avaliable;
            case TriggerType.Whenever:
                if (JudgeDirection)
                {
                    return AllowDirection.Contains(GetPlayerDir());
                }
                else
                {
                    return true;
                }                
            case TriggerType.OnceTrigger:
                if (JudgeDirection)
                {
                    return AllowDirection.Contains(GetPlayerDir()) && SteppedIn;
                }
                else
                {
                    return SteppedIn;
                }
        }
        return false;
    }
    
    private void Awake() {
        scriptCarrier.parent = this;
        if(Script != null)
        {
            scriptCarrier.code = Script.text.Split(new string[] { "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < scriptCarrier.code.Length; i++)
            {
                scriptCarrier.code[i] = scriptCarrier.code[i].TrimStart();
            }
            scriptCarrier.callback = () => { ScriptRunning = false; };
        }
    }

    public void FixPlayerPos()
    {
        Chara p = MapCamera.Player;
        Chara.walkDir dir = p.dir;
        bool needFix = false;
        if (p.dir == Chara.walkDir.Down || p.dir == Chara.walkDir.Up)
        {
            if (Mathf.Abs(p.transform.localPosition.x - BindChara.transform.localPosition.x) > 0.25f)
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
    public virtual void Update() {
        if (!IsActive()) return;
        if (ScriptRunning) return;

        if (!Silent)
        {
            MapCamera.ForbiddenMove = true;
            MapCamera.Player.ClosePadAni();
        }
        if (Script != null){
            scriptCarrier.currentLine = 0;
            DramaScript.Active = scriptCarrier;
            ScriptRunning = true;
            // ������������
            if (BindChara != null)
                FixPlayerPos();
            else
                scriptCarrier.carryTask();
            return; 
        }
        if(UseContentAsDramaPrefabName)
        {
            Dramas.Launch(Content,() => {
                if(!Settings.Active && !Settings.Loading) MapCamera.ForbiddenMove = false;
            });
        }else{
            Dramas.LaunchCheck(Content,() => {
                if (!Settings.Active && !Settings.Loading) MapCamera.ForbiddenMove = false;
            }).LifeTime = Dramas.DramaLifeTime.DieWhenReadToEnd;
        }
    }
}
