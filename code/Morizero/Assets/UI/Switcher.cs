using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;
using System.Threading.Tasks;

public class Switcher : MonoBehaviour
{
    // 动画控制器
    private Animator animator;
    public static bool isUsing = false;         // 是否在使用
    public static string destination;           // 目标场景
    public static LoadSceneMode loadMode;       // 加载方式
    public static int task;                     // 任务（0=加载，1=卸载）
    /// <summary>
    /// 场景切换器
    /// </summary>
    /// <param name="scene">场景名</param>
    /// <param name="mode">加载方式</param>
    /// <param name="Task">提交任务（0=加载，1=卸载）</param>
    public async static Task Carry(string scene,LoadSceneMode mode = LoadSceneMode.Single,int task = 0){
        if(isUsing) return;
        isUsing = true; destination = scene; loadMode = mode; Switcher.task = task;
        GameObject fab = (GameObject)Resources.Load("Prefabs\\Loading");    // 载入母体
        Instantiate(fab,new Vector3(0,0,-1),Quaternion.identity).SetActive(true);
        await Task.Run(() => {
            while(isUsing) Thread.Sleep(100);
        });
    }
    // 第一过程
    void Stage1(){
        if(task == 0){
            SceneManager.LoadSceneAsync(destination,loadMode);  // 异步加载
        }else{
            SceneManager.UnloadSceneAsync(destination);         // 异步卸载
        }
        animator.SetFloat("rate",0f);                           // 暂停动画
        
    }
    // 第二过程（终止过程）
    void Stage2(){
        isUsing = false;                                        // 恢复使用
        Destroy(this.gameObject);                               // 自鲨
    }
    /**
     * 场景加载完成回调函数
     */
    public void SceneLoaded_CallBack(Scene scene, LoadSceneMode sceneType)
    {
        SceneManager.sceneLoaded -= SceneLoaded_CallBack;       // 取消回调钩子
        SceneManager.sceneUnloaded -= SceneUnLoaded_CallBack;   // 取消回调钩子
        animator.SetFloat("rate",1f);                           // 继续动画
    }
    /**
     * 场景卸载完成回调函数
     */
    public void SceneUnLoaded_CallBack(Scene scene)
    {
        SceneManager.sceneLoaded -= SceneLoaded_CallBack;       // 取消回调钩子
        SceneManager.sceneUnloaded -= SceneUnLoaded_CallBack;   // 取消回调钩子
        animator.SetFloat("rate",1f);                           // 继续动画
    }
    // 醒来
    private void Awake() {
        SceneManager.sceneLoaded += SceneLoaded_CallBack;       // 设置回调钩子
        SceneManager.sceneUnloaded += SceneUnLoaded_CallBack;   // 设置回调钩子
        animator = this.GetComponent<Animator>();               // 取得动画控制器
        DontDestroyOnLoad(this.gameObject);                     // 免死金牌
    } 
}