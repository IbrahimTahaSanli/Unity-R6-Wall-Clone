using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{

    [SerializeField] float SecForTimeout;

    Coroutine co;
    void Awake()
    {
        co = StartCoroutine(DestroyAfter(SecForTimeout));
    }

    private void OnDestroy()
    {
        StopCoroutine(co);
    }

    IEnumerator DestroyAfter(float sec)
    {
        yield return new WaitForSeconds(sec);
        GameObject.Destroy(gameObject);
    }


    private void OnCollisionEnter(Collision collision)
    {
        QuadTree.staTree.ObjectHit(collision.contacts[0].point);
        Destroy(this.gameObject);
    }
}
