using UnityEngine;
using System.Collections;

//面板实例化
public class PanelInstance : MonoBehaviour {
	//加入面板，由PanelManager统一控制
	public PanelInstance PanelBefore;
	public RuntimeAnimatorController runtimeAnim;
    public bool LockMouse;
    public bool isClosed;
	private bool hasAnimator;

    //打开面板
	public void OpenPanel(bool active){
		isClosed = !active;
		this.gameObject.SetActive (active);

		Animator animator = this.GetComponent<Animator> ();
		if (animator != null && animator.isActiveAndEnabled) {
			animator.SetBool ("Open", active);
		}
	}

    //停用
	void OnDisable()
	{
		Animator animator = this.GetComponent<Animator> ();
		if(animator)
		Destroy(animator);
	}

    //启用
	void OnEnable()
	{
		if (hasAnimator) {
			Animator animator = this.GetComponent<Animator> ();
			if (animator == null) {
				animator = gameObject.AddComponent<Animator> ();
				animator.runtimeAnimatorController = runtimeAnim;

			}
		}
	}

    //关闭面板
	public void Closed(){
		isClosed = true;
	}


	void Start () {
		Animator animator = this.GetComponent<Animator> ();
		if (animator) {
			runtimeAnim = animator.runtimeAnimatorController;
			hasAnimator = true;
		} else {
			hasAnimator = false;
		}
	}


}
