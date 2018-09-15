using UnityEngine;
using UnityEngine.Networking;

public class DoorFrame : NetworkBehaviour
{

    public Animator animator;
    [SyncVar(hook = "OnDoorOpen")]
    public bool IsOpen;
    private float timeTemp;
    public float Cooldown = 0.5f;
    public string DoorKey = "";
    public AudioSource audioSource;

    void Start()
    {
        if (audioSource == null)
            audioSource = this.GetComponent<AudioSource>();

        if (animator == null)
            animator = this.GetComponent<Animator>();
    }

    public override void OnStartClient()
    {
        OnDoorOpen(IsOpen);
        base.OnStartClient();
    }

    void OnDoorOpen(bool open)
    {
        IsOpen = open;
        if (animator)
        {
            animator.SetBool("IsOpen", IsOpen);
        }
        if (audioSource)
            audioSource.Play();
    }

    public void Access(CharacterSystem character)
    {
        AccessDoor(DoorKey);
    }


    private void AccessDoor(string key)
    {
        if (key == DoorKey)
        {
            if (Time.time > timeTemp + Cooldown)
            {   
                IsOpen = !IsOpen;
                timeTemp = Time.time;
            }
        }
    }
}
