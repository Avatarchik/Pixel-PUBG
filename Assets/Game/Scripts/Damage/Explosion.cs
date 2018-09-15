using UnityEngine;
using System.Linq;

public class Explosion : DamageBase
{
    public bool IgnoreTeam = false;
    public float Duration = 3;
    public float Force = 100;
    public float Radius = 30;
    public byte Damage = 100;
    public string[] BlockerTag = { "Scene" };

    void Start()
    {
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, Radius);
        foreach (Collider hit in colliders)
        {
            float distance = Vector3.Distance(this.transform.position, hit.transform.position);
            float dmMult = 1 - ((1.0f / Radius) * distance);
            if (dmMult < 0)
                dmMult = 0;

            RaycastHit[] hits = Physics.RaycastAll(this.transform.position, (hit.transform.position - this.transform.position).normalized, distance + 0.2f).OrderBy(h => h.distance).ToArray();
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hited = hits[i];

                if (hited.collider.GetComponent<Rigidbody>())
                    hited.collider.GetComponent<Rigidbody>().AddExplosionForce(Force, this.transform.position, Radius, 3.0F);

                DamagePackage dm;
                dm.Damage = Damage;
                dm.Normal = hited.normal ;
                dm.Direction = (hit.transform.position - this.transform.position).normalized * Force;
                dm.Position = hited.point;
                dm.ID = OwnerID;
                dm.DamageType = 0;

                if (IgnoreTeam)
                {
                    dm.Team = OwnerTeam;
                }
                else
                {
                    dm.Team = 0;
                }
                hited.collider.GetComponent<Collider>().SendMessage("OnHit", dm, SendMessageOptions.DontRequireReceiver);
            }

        }
        Destroy(this.gameObject, Duration);
    }


    private bool tagDestroyerCheck(string tag)
    {
        for (int i = 0; i < BlockerTag.Length; i++)
        {
            if (BlockerTag[i] == tag)
            {
                return true;
            }
        }
        return false;
    }


}
