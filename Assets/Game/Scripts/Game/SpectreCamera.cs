using UnityEngine;

public class SpectreCamera : MonoBehaviour
{

    public Vector3 LookAtPosition;
    public Vector3 PositionOffset = new Vector3(0, 2, 3);
    public GameObject LookAtObject;

    private bool useragdoll;
    private bool lookAtSomething;
    private AudioListener audioList;

    void Start()
    {
        if (UnitZ.playerManager != null)
            UnitZ.playerManager.Spectre = this;
        LookAtPosition = this.transform.forward * 10;
        audioList = this.GetComponent<AudioListener>();
    }

    public void Active(bool active)
    {
        this.GetComponent<Camera>().enabled = active;
        if (audioList)
            audioList.enabled = active;
    }

    void Update()
    {
        lookAtSomething = false;
        if (LookAtObject)
        {
            lookAtSomething = true;
            if (!useragdoll)
                LookAtPosition = LookAtObject.transform.position;
            RagdollReplace ragdoll = LookAtObject.GetComponent<RagdollReplace>();
            if (ragdoll)
            {
                useragdoll = true;
                lookAtSomething = true;
                LookAtPosition = ragdoll.RootRagdoll.transform.position;
            }
        }
        if (lookAtSomething)
        {
            Quaternion lookat = Quaternion.LookRotation(LookAtPosition - this.transform.position);
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, lookat, Time.deltaTime * 10);
            this.transform.position = LookAtPosition + PositionOffset;
        }
    }

    public void LookingAt(Vector3 position)
    {
        LookAtPosition = position;
    }

    public void LookingAtObject(GameObject obj)
    {
        LookAtObject = obj;
    }
}
