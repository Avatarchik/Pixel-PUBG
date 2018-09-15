using UnityEngine;
using System.Linq;

//FPS射线激活
public class FPSRayActive : MonoBehaviour
{
    public bool Sorting;
    public string[] IgnoreTag = { "Player" };
    public string[] DestroyerTag = { "scene" };

    public void ShootRayOnce(Vector3 origin, Vector3 direction, int id, byte team)
    {
        //普通伤害射击
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, 100.0f))
        {
            //如果击中
            if (hit.collider.gameObject != this.gameObject)
            {
                //造成伤害，打包数据
                DamagePackage dm;
                dm.Damage = 50;
                dm.Normal = hit.normal;
                dm.Direction = direction;
                dm.Position = hit.point;
                dm.ID = id;
                dm.Team = team;
                dm.DamageType = 0;
                //通过OnHit发送伤害数据
                hit.collider.SendMessage("OnHit", dm, SendMessageOptions.DontRequireReceiver);

            }
        }
    }

    public void CheckingRay(Vector3 origin, Vector3 direction)
    {
        float raySize = 3;
        //Ray转换为任何对象以获取对象信息
        RaycastHit[] casterhits = Physics.RaycastAll(origin, direction, raySize);
        for (int i = 0; i < casterhits.Length; i++)
        {
            if (casterhits[i].collider)
            {
                RaycastHit hit = casterhits[i];
                //通过GetInfo取得物品信息
                hit.collider.SendMessage("GetInfo", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    public void ActiveRay(Vector3 origin, Vector3 direction)
    {
        //Ray投射到Interactive和Pickup
        float raySize = 3;
        RaycastHit[] casterhits = Physics.RaycastAll(origin, direction, raySize);
        for (int i = 0; i < casterhits.Length; i++)
        {
            if (casterhits[i].collider)
            {
                //通过Pickup进行交互
                casterhits[i].collider.SendMessage("Pickup", this.GetComponent<CharacterSystem>(), SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    public void ActiveLocalRay(Vector3 origin, Vector3 direction)
    {
        //Ray投射到Interactive和Pickup
        float raySize = 3;
        RaycastHit[] casterhits = Physics.RaycastAll(origin, direction, raySize);
        for (int i = 0; i < casterhits.Length; i++)
        {
            if (casterhits[i].collider)
            {
                //通过Pickup进行交互
                casterhits[i].collider.SendMessage("PickupLocal", this.GetComponent<CharacterSystem>(), SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    public bool ShootSingleRay(Vector3 origin, Vector3 direction, byte num, byte spread, byte seed, byte damage, float size, int id, byte team)
    {
        bool res = false;
        System.Random random = new System.Random(seed);
        if (seed <= 0)
            spread = 0;
        for (int b = 0; b < num; b++)
        {
            Vector3 dir = direction + new Vector3(random.Next(-spread, spread) * 0.001f, random.Next(-spread, spread) * 0.001f, random.Next(-spread, spread) * 0.001f);
            RaycastHit hit;
            //射击所有对象
            if(Physics.Raycast(origin, dir,out hit, size))
            {
                if (hit.collider)
                {
                    DamagePackage dm;
                    dm.Damage = damage;
                    dm.Normal = hit.normal;
                    dm.Direction = dir;
                    dm.Position = hit.point;
                    dm.ID = id;
                    dm.Team = team;
                    dm.DamageType = 0;
                    //通过OnHit发送数据包
                    hit.collider.SendMessage("OnHit", dm, SendMessageOptions.DontRequireReceiver);
                    res = true;
                }
            }
        }
        return res;
    }

    public bool ShootSingleRayTest(Vector3 origin, Vector3 direction, byte num, byte spread, byte seed, byte damage, float size, int id, byte team)
    {
        bool res = false;
        System.Random random = new System.Random(seed);
        if (seed <= 0)
            spread = 0;
        for (int b = 0; b < num; b++)
        {
            Vector3 dir = direction + new Vector3(random.Next(-spread, spread) * 0.001f, random.Next(-spread, spread) * 0.001f, random.Next(-spread, spread) * 0.001f);
            RaycastHit hit;
            //射击所有对象
            if (Physics.Raycast(origin, dir, out hit, size))
            {
                if (hit.collider)
                {
                    DamagePackage dm;
                    dm.Damage = damage;
                    dm.Normal = hit.normal;
                    dm.Direction = dir;
                    dm.Position = hit.point;
                    dm.ID = id;
                    dm.Team = team;
                    dm.DamageType = 0;
                    //通过OnHit发送伤害数据包
                    hit.collider.SendMessage("OnHitTest", dm, SendMessageOptions.DontRequireReceiver);
                    res = true;
                }
            }
        }
        return res;
    }

    public bool ShootRay(Vector3 origin, Vector3 direction, byte num, byte spread, byte seed, byte damage, float size, byte hitmax, int id, byte team)
    {
        //多穿孔伤害。例如，你可以在很多层中射击
        bool res = false;
        System.Random random = new System.Random(seed);
        if (seed <= 0)
            spread = 0;
        for (int b = 0; b < num; b++)
        {
            Vector3 dir = direction + new Vector3(random.Next(-spread, spread) * 0.001f, random.Next(-spread, spread) * 0.001f, random.Next(-spread, spread) * 0.001f);
            int hitcount = 0;
            //射击所有物体
            RaycastHit[] hits = Physics.RaycastAll(origin, dir, size).OrderBy(h => h.distance).ToArray();
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider)
                {
                    if (tagCheck(hits[i].collider.gameObject) &&
                    hits[i].collider.gameObject != this.gameObject &&
                    ((hits[i].collider.transform.root &&
                        hits[i].collider.transform.root != this.gameObject.transform.root &&
                        hits[i].collider.transform.root.gameObject != this.gameObject) ||
                        hits[i].collider.transform.root == null))
                    {
                        RaycastHit hit = hits[i];
                        //创建伤害包
                        DamagePackage dm;
                        dm.Damage = damage;
                        dm.Normal = hit.normal;
                        dm.Direction = dir;
                        dm.Position = hit.point;
                        dm.ID = id;
                        dm.Team = team;
                        dm.DamageType = 0;
                        //发送伤害数据包
                        hit.collider.SendMessage("OnHit", dm, SendMessageOptions.DontRequireReceiver);
                        res = true;

                        //计算伤害直到最大值
                        hitcount++;
                        if (hitcount >= hitmax || tagDestroyerCheck(hit.collider.gameObject))
                        {
                            break;
                        }
                    }
                }
            }
        }
        return res;
    }

    public bool ShootRayTest(Vector3 origin, Vector3 direction, byte num, byte spread, byte seed, byte damage, float size, byte hitmax, int id, byte team)
    {
        bool res = false;
        System.Random random = new System.Random(seed);
        if (seed <= 0)
            spread = 0;
        for (int b = 0; b < num; b++)
        {
            Vector3 dir = direction + new Vector3(random.Next(-spread, spread) * 0.001f, random.Next(-spread, spread) * 0.001f, random.Next(-spread, spread) * 0.001f);
            int hitcount = 0;
            //射击所有物体
            RaycastHit[] hits = Physics.RaycastAll(origin, dir, size).OrderBy(h => h.distance).ToArray();
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider)
                {
                    if (tagCheck(hits[i].collider.gameObject) &&
                    hits[i].collider.gameObject != this.gameObject &&
                    ((hits[i].collider.transform.root &&
                        hits[i].collider.transform.root != this.gameObject.transform.root &&
                        hits[i].collider.transform.root.gameObject != this.gameObject) ||
                        hits[i].collider.transform.root == null))
                    {
                        RaycastHit hit = hits[i];
                        //创建伤害数据包
                        DamagePackage dm;
                        dm.Damage = damage;
                        dm.Normal = hit.normal;
                        dm.Direction = dir;
                        dm.Position = hit.point;
                        dm.ID = id;
                        dm.Team = team;
                        dm.DamageType = 0;
                        //发送伤害数据包
                        hit.collider.SendMessage("OnHitTest", dm, SendMessageOptions.DontRequireReceiver);
                        res = true;

                        //计算射击伤害直到最大值
                        hitcount++;
                        if (hitcount >= hitmax || tagDestroyerCheck(hit.collider.gameObject))
                        {
                            break;
                        }
                    }
                }
            }
        }
        return res;
    }

    public bool Overlap(Vector3 origin, Vector3 forward, byte damage, float size, float dot, int id, byte team)
    {
        //重叠伤害不是射线，它只是造成伤害区域，例如用于近战伤害
        bool res = false;
        var colliders = Physics.OverlapSphere(origin, size);

        foreach (var hit in colliders)
        {
            if (hit && hit.gameObject != this.gameObject && hit.gameObject.transform.root != this.gameObject.transform)
            {
                Debug.Log(hit.gameObject.transform.root.name);
                var dir = (hit.transform.position - origin).normalized;
                var direction = Vector3.Dot(dir, forward);

                if (direction >= dot)
                {
                    DamagePackage dm;
                    dm.Damage = damage;
                    dm.Normal = dir;
                    dm.Direction = forward;
                    dm.Position = hit.gameObject.transform.position;
                    dm.ID = id;
                    dm.Team = team;
                    dm.DamageType = 0;
                    hit.GetComponent<Collider>().SendMessage("OnHit", dm, SendMessageOptions.DontRequireReceiver);

                    res = true;
                }
            }
        }
        return res;
    }

    public bool OverlapTest(Vector3 origin, Vector3 forward, byte damage, float size, float dot, int id, byte team)
    {
        //重叠测试仅仅用于测试
        bool res = false;
        var colliders = Physics.OverlapSphere(origin, size);

        foreach (var hit in colliders)
        {
            if (hit && hit.gameObject != this.gameObject && hit.gameObject.transform.root != this.gameObject)
            {
                var dir = (hit.transform.position - origin).normalized;
                var direction = Vector3.Dot(dir, forward);

                if (direction >= dot)
                {
                    res = true;
                }
            }
        }

        return res;
    }

    private bool tagDestroyerCheck(GameObject obj)
    {
        for (int i = 0; i < DestroyerTag.Length; i++)
        {
            if (obj.CompareTag(DestroyerTag[i]))
            {
                return true;
            }
        }
        return false;
    }

    private bool tagCheck(GameObject obj)
    {
        for (int i = 0; i < IgnoreTag.Length; i++)
        {
            if (obj.CompareTag(IgnoreTag[i]))
            {
                return false;
            }
        }
        return true;
    }
}
