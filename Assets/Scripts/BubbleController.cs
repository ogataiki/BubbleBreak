using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchScript.Gestures;

public class BubbleController : MonoBehaviour {

	public TapGesture tapGesture;
	
	public bool m_isGame = false;

	public int m_key = 0;
	
	public float m_scale_add = 0.01f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if(m_isGame)
		{
			float xs = transform.localScale.x;
			float ys = transform.localScale.y;
			this.transform.localScale = new Vector3(xs+m_scale_add, ys+m_scale_add, 1);
			Color newColor = new Color(1, 1, 1, 0.5f - (this.transform.localScale.x * 0.1f) );
			this.GetComponent<MeshRenderer> ().material.color = newColor;
		}
	}
	
    void OnEnable()
	{
    	// TapGestureのdelegateに登録
    	GetComponent<TapGesture>().Tapped += tappedHandle;
	}

	void OnDisable()
	{
    	UnsubscribeEvent();
	}

	void OnDestroy()
	{
    	UnsubscribeEvent();
    	
    	var thisRenderer = this.GetComponent<Renderer>();
        if(thisRenderer != null && thisRenderer.materials != null){
            foreach(var m in thisRenderer.materials){
                DestroyImmediate(m);
            }
        }
	}

	void UnsubscribeEvent()
	{
    	// 登録を解除
    	GetComponent<TapGesture>().Tapped -= tappedHandle;
	}

	void tappedHandle(object sender, System.EventArgs e)
	{
		Debug.Log("bubble tapped");
	}
}
