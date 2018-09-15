using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public enum MortorType
{
	Rear,
	Front,
	FourWheel
}

//汽车控制
public class CarControl : NetworkBehaviour
{
	public MortorType Type;
	public WheelCollider Wheel_FL;
	public WheelCollider Wheel_FR;
	public WheelCollider Wheel_RL;
	public WheelCollider Wheel_RR;
	public float[] GearRatio;
	public int CurrentGear = 0;
	public float EngineTorque = 600.0f;
	public float MaxEngineRPM = 3000.0f;
	public float MinEngineRPM = 1000.0f;
	public float SteerAngle = 10;
	public Transform COM;
	public float Speed;
	public float maxSpeed = 150;
	public AudioSource skidAudio;
	public float EngineRPM = 0.0f;
	private float motorInput;
	public bool IsEngineStarted;


	void Start ()
	{
		GetComponent<Rigidbody> ().centerOfMass = new Vector3 (COM.localPosition.x * transform.localScale.x, COM.localPosition.y * transform.localScale.y, COM.localPosition.z * transform.localScale.z);
	}

	private Vector2 inputAxis;
	private bool inputBrake;
	[SyncVar]
	public float steerAngleTemp;
	[SyncVar]
	private float audiopitch;

	public void Controller (Vector2 input, bool brake)
	{
		inputAxis = input;
		inputBrake = brake;
	}

	private float latestMotorInput;

	void Update ()
	{
		if (isServer) {
			if (GetComponent<Rigidbody> ())
				GetComponent<Rigidbody> ().isKinematic = false;
			
			Speed = GetComponent<Rigidbody> ().velocity.magnitude * 3.6f;
			GetComponent<Rigidbody> ().drag = GetComponent<Rigidbody> ().velocity.magnitude / 100;
			EngineRPM = (Wheel_FL.rpm + Wheel_FR.rpm) / 2 * GearRatio [CurrentGear];

			if (!IsEngineStarted)
				EngineRPM = 0;
	
			ShiftGears ();
	
			//MotorInput.
			motorInput = inputAxis.y;
	
			//音效
			GetComponent<AudioSource> ().pitch = Mathf.Abs (EngineRPM / MaxEngineRPM) + 1.0f;
			if (GetComponent<AudioSource> ().pitch > 2.0f) {
				GetComponent<AudioSource> ().pitch = 2.0f;
			}
	
			//转向
			steerAngleTemp = SteerAngle * inputAxis.x;
			Wheel_FL.steerAngle = steerAngleTemp;
			Wheel_FR.steerAngle = steerAngleTemp;
	
			//限速
			if (Speed > maxSpeed || !IsEngineStarted) {
				Wheel_RL.motorTorque = 0;
				Wheel_RR.motorTorque = 0;
				Wheel_FL.motorTorque = 0;
				Wheel_FR.motorTorque = 0;
			} else {
				switch (Type) {
				case MortorType.FourWheel:
					Wheel_FL.motorTorque = (EngineTorque / GearRatio [CurrentGear]) * inputAxis.y;
					Wheel_FR.motorTorque = (EngineTorque / GearRatio [CurrentGear]) * inputAxis.y;
					Wheel_RL.motorTorque = (EngineTorque / GearRatio [CurrentGear]) * inputAxis.y;
					Wheel_RR.motorTorque = (EngineTorque / GearRatio [CurrentGear]) * inputAxis.y;
					break;
				case MortorType.Front:
					Wheel_FL.motorTorque = (EngineTorque / GearRatio [CurrentGear]) * inputAxis.y;
					Wheel_FR.motorTorque = (EngineTorque / GearRatio [CurrentGear]) * inputAxis.y;
					break;
				case MortorType.Rear:
					Wheel_RL.motorTorque = (EngineTorque / GearRatio [CurrentGear]) * inputAxis.y;
					Wheel_RR.motorTorque = (EngineTorque / GearRatio [CurrentGear]) * inputAxis.y;
					break;
				}
			}
			if (latestMotorInput != motorInput || !IsEngineStarted) {
				Wheel_RL.brakeTorque = 10000;
				Wheel_RR.brakeTorque = 10000;
			}
			//输入
			if (motorInput <= 0) {
				Wheel_RL.brakeTorque = 0;
				Wheel_RR.brakeTorque = 0;
			} else if (motorInput >= 0) {
				Wheel_RL.brakeTorque = 0;
				Wheel_RR.brakeTorque = 0;
			}
			//手刹
			if (inputBrake || !IsEngineStarted) {
				Wheel_RL.brakeTorque = 1000;
				Wheel_RR.brakeTorque = 1000;
		
			}
			if (!inputBrake) {
				Wheel_FL.brakeTorque = 0;
				Wheel_FR.brakeTorque = 0;
			}
	
			//刹车音效
			WheelHit CorrespondingGroundHit;
			Wheel_RR.GetGroundHit (out CorrespondingGroundHit);
			if (Mathf.Abs (CorrespondingGroundHit.sidewaysSlip) > 10) {
				skidAudio.enabled = true;
			} else {
				skidAudio.enabled = false;
			}
		
			inputAxis = Vector2.zero;
			inputBrake = false;

			engineUpdate ();

		} else {
			getengineUpdate ();
			if (GetComponent<Rigidbody> ())
				GetComponent<Rigidbody> ().isKinematic = true;	
		}

		latestMotorInput = motorInput;
	}

	public void engineUpdate ()
	{
		if (GetComponent<AudioSource> ().pitch > 2.0f) {
			GetComponent<AudioSource> ().pitch = 2.0f;
		}
		audiopitch = GetComponent<AudioSource> ().pitch;
		steerAngleTemp = Wheel_FL.steerAngle;
	}

	public void StartEngine(bool start){
		IsEngineStarted = start;
	}

	public void getengineUpdate ()
	{
		GetComponent<AudioSource> ().pitch = audiopitch;
		Wheel_FL.steerAngle = steerAngleTemp;
	}

	
	void ShiftGears ()
	{
		int AppropriateGear = CurrentGear;
		if (EngineRPM >= MaxEngineRPM) {
			
			for (int i = 0; i < GearRatio.Length; i++) {
				if (Wheel_FL.rpm * GearRatio [i] < MaxEngineRPM) {
					AppropriateGear = i;
					break;
				}
			}
			CurrentGear = AppropriateGear;
		}
	
		if (EngineRPM <= MinEngineRPM) {
			AppropriateGear = CurrentGear;
			for (int j = GearRatio.Length - 1; j >= 0; j--) {
				if (Wheel_FL.rpm * GearRatio [j] > MinEngineRPM) {
					AppropriateGear = j;
					break;
				}
			}
			CurrentGear = AppropriateGear;
		}
	}
}
