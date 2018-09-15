using UnityEngine;
using System.Collections;

public class MouseLooking : MonoBehaviour {

	public float sensitivityX = 15;
	public float sensitivityY = 15;
	public float minimumX = -360;
	public float maximumX = 360;
	public float minimumY = -60;
	public float maximumY = 60;
	public float delayMouse = 3;
	public float noiseX = 0.1f;
	public float noiseY = 0.1f;
	public bool Noise;

	private float rotationX = 0;
	private float rotationY = 0;
	private float rotationXtemp = 0;
	private float rotationYtemp = 0;
	private Quaternion originalRotation;
	private float noisedeltaX;
	private float noisedeltaY;
	private float stunY;
	private float breathHolderValtarget = 1;
	private float breathHolderVal = 1;
	
	
	
	void Start () {
    	if (GetComponent<Rigidbody>())
    	    GetComponent<Rigidbody>().freezeRotation = true;
   			originalRotation = transform.localRotation;
		
	}
	
	
	
	void Update () {
		sensitivityY = sensitivityX;
		
    
    	stunY+= (0-stunY)/20f;
    	
    	if(Noise){
        	noisedeltaX += ((((Mathf.Cos(Time.time) * Random.Range(-10,10)/5f) * noiseX) - noisedeltaX)/100);
        	noisedeltaY += ((((Mathf.Sin(Time.time) * Random.Range(-10,10)/5f) * noiseY) - noisedeltaY)/100);
		}else{
			
			noisedeltaX = 0;
			noisedeltaY = 0;
		}

		rotationXtemp += (Input.GetAxis("Mouse X") * sensitivityX) + (noisedeltaX * breathHolderVal);
		rotationYtemp += (Input.GetAxis("Mouse Y") * sensitivityY) + (noisedeltaY * breathHolderVal);
		rotationX += (rotationXtemp - rotationX)/delayMouse;
		rotationY += (rotationYtemp - rotationY)/delayMouse;

        if(rotationX>=360){
        	rotationX = 0;
        	rotationXtemp = 0;
        }
         if(rotationX<=-360){
        	rotationX = 0;
        	rotationXtemp = 0;
        }
		
        rotationX = ClampAngle (rotationX, minimumX, maximumX);
        rotationY = ClampAngle (rotationY, minimumY, maximumY);
		rotationYtemp = ClampAngle (rotationYtemp, minimumY, maximumY);
        

        Quaternion xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
        Quaternion yQuaternion = Quaternion.AngleAxis (rotationY+stunY, Vector3.left);

        transform.localRotation = originalRotation * xQuaternion * yQuaternion;
		breathHolderVal += (breathHolderValtarget-breathHolderVal)/10;	
		
	}
	
	public void Holdbreath(float val){
		breathHolderValtarget = val;
	}
	
	public void Stun(float val){
		stunY = val;
	}

	static float ClampAngle (float angle,float min,float max) {
    	if (angle < -360.0f)
        	angle += 360.0f;

    	if (angle > 360.0f)
        	angle -= 360.0f;

    	return Mathf.Clamp (angle, min, max);
	}
}
