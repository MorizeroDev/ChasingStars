using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlurController : MonoBehaviour
{
    public RenderTexture blurTexture;
    public Canvas Container;
    public GameObject Displayer;
    private Texture2D outputTexture;
    private void Start()
    {
        outputTexture = new Texture2D(blurTexture.width, blurTexture.height);
        bool[] active = new bool[Container.transform.childCount];
        for(int i = 0;i < Container.transform.childCount; i++)
        {
            active[i] = Container.transform.GetChild(i).gameObject.activeSelf;
            if(!Container.transform.GetChild(i).gameObject.Equals(this.gameObject))
                Container.transform.GetChild(i).gameObject.SetActive(false);
        }
        Camera.main.useOcclusionCulling = false;
        Camera.main.targetTexture = blurTexture;
        Camera.main.Render();
        if (Input.GetKey(KeyCode.T))
        {
            GetComponent<Image>().material = null;
            GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
        }
        RenderTexture.active = blurTexture;
        outputTexture.ReadPixels(new Rect(0, 0, blurTexture.width, blurTexture.height), 0, 0);
        RenderTexture.active = null;
        outputTexture.Apply();
        Camera.main.useOcclusionCulling = true;
        Camera.main.targetTexture = null;
        Displayer.GetComponent<RawImage>().texture = outputTexture;
        GetComponent<Image>().material = null;
        GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
        //System.IO.File.WriteAllBytes("D:\\test.jpg", outputTexture.EncodeToJPG());
        for (int i = 0; i < Container.transform.childCount; i++)
        {
            Container.transform.GetChild(i).gameObject.SetActive(active[i]);
        }
        Displayer.SetActive(true);
    }
    private void OnDestroy()
    {
        Destroy(outputTexture);
    }
}
