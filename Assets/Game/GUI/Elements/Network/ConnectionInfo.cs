using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ConnectionInfo : MonoBehaviour
{

	public InputField PortText;
	public InputField PaasswordText;
	public InputField ServerIPText;
	public InputField ServerNameText;
	public InputField ServerNameFillterText;
    public Text MapSelect;

	void Start ()
	{

	}

	void OnEnable(){
		if (UnitZ.gameNetwork) {

            if (MapSelect)
            {
                MainMenuManager menu = (MainMenuManager)GameObject.FindObjectOfType(typeof(MainMenuManager));
                MapSelect.text = menu.SceneStart;
            }

			if (PortText)
				PortText.text = UnitZ.gameNetwork.networkPort.ToString ();

			if (ServerIPText)
				ServerIPText.text = UnitZ.gameNetwork.networkAddress;

			if (ServerNameText)
				ServerNameText.text = UnitZ.gameNetwork.matchName;

			if (ServerNameFillterText)
				ServerNameFillterText.text = UnitZ.gameNetwork.HostNameFillter;

			if (PortText)
				PortText.text = UnitZ.gameNetwork.networkPort.ToString();

			if (PaasswordText)
				PaasswordText.text = UnitZ.gameNetwork.HostPassword.ToString();
		}
	}

	public void SetServerIP (InputField num)
	{
		if (UnitZ.gameNetwork) {
			UnitZ.gameNetwork.networkAddress = num.text;
		}
	}
	public void SetServerNameFillter (InputField num)
	{
		if (UnitZ.gameNetwork) {
			UnitZ.gameNetwork.HostNameFillter = num.text;
		}
	}
	public void SetServerName (InputField num)
	{
		if (UnitZ.gameNetwork) {
			UnitZ.gameNetwork.matchName = num.text;
		}
	}
	public void SetPassword (InputField num)
	{
		if (UnitZ.gameNetwork) {
			UnitZ.gameNetwork.HostPassword = num.text;
		}
	}
	public void SetPort (InputField num)
	{
		if (UnitZ.gameNetwork) {
			int val = UnitZ.gameNetwork.networkPort;
			if (int.TryParse (num.text, out val)) {
				UnitZ.gameNetwork.networkPort = val;
			}
		}
	}
	
	
		

}
