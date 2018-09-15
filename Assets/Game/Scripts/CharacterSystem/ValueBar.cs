using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//数值条
public class ValueBar : MonoBehaviour {

	public RectTransform Bar;
	public RectTransform BarBG;
	public float Value = 50;
	public float ValueMax = 100;
	public Text ValueText;
	public string CustomText = "";
	void Start () {

	}
	
	void Update () {
		if(Bar!=null && BarBG!=null){
			float width = (BarBG.sizeDelta.x / ValueMax) * Value;
			Bar.sizeDelta = new Vector2(width,Bar.sizeDelta.y);
		}
		if(ValueText){
			ValueText.text = Value.ToString();	
			if(CustomText!=""){
				ValueText.text = CustomText;
			}
		}
	}
}
