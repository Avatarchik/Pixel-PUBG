using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class PanelsManager : MonoBehaviour
{
	public PanelInstance[] Pages;
	public PanelInstance currentPanel;

	void Start ()
	{
		// add PanelInstance component to every panels in the list.
		for (int i=0; i<Pages.Length; i++) {
			Pages [i].gameObject.AddComponent<PanelInstance> ();
		}
	}
	
	void Awake ()
	{
		if (Pages.Length <= 0)
			return;
		
		// open first panels at start.
		OpenPanel (Pages [0]);	
	}
	
	// use this function when you need to close all panels in the list normally.
	public void CloseAllPanels ()
	{
		if (Pages.Length <= 0)
			return;
		
		for (int i=0; i<Pages.Length; i++) {
			Pages[i].OpenPanel (false);
			if(Pages [i].isActiveAndEnabled){
				StartCoroutine (DisablePanelDeleyed (Pages [i]));
			}
		}
	}
	
	// use this function when you need to disable all panels in the list directly.
	public void DisableAllPanels ()
	{
		if (Pages.Length <= 0)
			return;
		
		for (int i=0; i<Pages.Length; i++) {
			Pages[i].gameObject.SetActive(false);
		}
	}
	
	// use this function when you need to close all the panels in the scene, even they are not in the lists.
	public void CloseAllPanelsInTheScene ()
	{
		PanelsManager[] panelsManage = (PanelsManager[])GameObject.FindObjectsOfType (typeof(PanelsManager));
		if (panelsManage.Length <= 0)
			return;
		
		for (int i=0; i<panelsManage.Length; i++) {
			panelsManage [i].CloseAllPanels ();	
		}
	}
	
	// open panel by name.
	public void OpenPanelByName (string name)
	{
		PanelInstance page = null;
		for (int i=0; i<Pages.Length; i++) {
			if (Pages [i].name == name) {
				page = Pages [i];
				break;
			}
		}
		if (page == null)
			return;
		
		page.PanelBefore = currentPanel;
		currentPanel = page;
		
		CloseAllPanels ();
		page.OpenPanel (true);
		
	}
	
	// for checking if the panel <name> is opened
	public bool IsPanelOpened (string name)
	{
		for (int i=0; i<Pages.Length; i++) {
			if (Pages [i].name == name) {
				return Pages [i].gameObject.activeSelf;
			}
		}
		return false;
	}
	
	// use this function when you need to open and close in the same way.
	public bool TogglePanelByName (string name)
	{
		PanelInstance page = null;
		for (int i=0; i<Pages.Length; i++) {
			if (Pages [i].name == name) {
				page = Pages [i];
				break;
			}
		}
		if (page == null)
			return false;
		
		if (page.gameObject.activeSelf) {
			ClosePanel (page);
			return false;
		} else {
			page.PanelBefore = currentPanel;
			currentPanel = page;
			CloseAllPanels ();
			page.OpenPanel (true);
			return true;
		}
		
	}
	
	// close panel by name.
	public void ClosePanelByName (string name)
	{
		PanelInstance page = null;
		for (int i=0; i<Pages.Length; i++) {
			if (Pages [i].name == name) {
				page = Pages [i];
				break;
			}
		}
		if (page == null)
			return;
		
		currentPanel = null;
		page.OpenPanel (false);
		StartCoroutine (DisablePanelDeleyed (page));
		
	}
	
	// close panel by object PanelInstance
	public void ClosePanel (PanelInstance page)
	{
		if (page == null)
			return;
		
		currentPanel = null;
		page.OpenPanel (false);
		StartCoroutine (DisablePanelDeleyed (page));
		
	}
	
	// open panel by object PanelInstance
	public void OpenPanel (PanelInstance page)
	{
		if (page == null)
			return;
		
		page.PanelBefore = currentPanel;
		currentPanel = page;
		
		CloseAllPanels ();
		page.OpenPanel (true);

	}

    private PanelInstance setPreviousPanel;
    public void SetPreviousPanel(PanelInstance panel)
    {
        setPreviousPanel = panel;
    }
    // use this function when you need to open previous panel
    public void OpenPreviousPanel ()
	{
        if (setPreviousPanel != null)
        {
            CloseAllPanels();
            setPreviousPanel.OpenPanel(true);
            currentPanel = setPreviousPanel;
            setPreviousPanel = null;
        }
        else
        {
            //Debug.Log ("open previous "+currentPanel.PanelBefore);
            if (currentPanel && currentPanel.PanelBefore)
            {
                CloseAllPanels();
                currentPanel.PanelBefore.OpenPanel(true);
                currentPanel = currentPanel.PanelBefore;
            }
        }
	}
	
	// use this function when you need to open panel without saving a previous.
	// so you can't use OpenPreviousPanel to open a previous panel again.
	public void OpenPanelByNameNoPreviousSave (string name)
	{
		PanelInstance page = null;
		for (int i=0; i<Pages.Length; i++) {
			if (Pages [i].name == name) {
				page = Pages [i];
				break;
			}
		}
		if (page == null)
			return;
		
		currentPanel = page;
		
		CloseAllPanels ();
		page.OpenPanel (true);
		
	}
	
	IEnumerator DisablePanelDeleyed (PanelInstance page)
	{

		//bool closedStateReached = false;
		bool wantToClose = true;
		page.isClosed = false;
		Animator anim = page.GetComponent<Animator> ();
		if (anim && anim.enabled) {

			while (!page.isClosed && wantToClose) {
				yield return new WaitForEndOfFrame();
			}

			if (wantToClose) {
				page.gameObject.SetActive (false);
			}
			
		} else {
			page.gameObject.SetActive (false);		
		}
		
	}
}
