using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject effectObj;
    public Rigidbody rigid;

    void Start()
    {
        StartCoroutine(Explosion());
    }

    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(3f);
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;

        RaycastHit[] rayHits = 
            Physics.SphereCastAll(transform.position,
            15, 
            Vector3.up, 0f, 
            LayerMask.GetMask("Target"));
        GameObject copyeffect = Instantiate(effectObj, transform.position, transform.rotation);
        copyeffect.SetActive(true);
        Destroy(this.gameObject);
        Destroy(copyeffect.gameObject, 3f);
    }
}
