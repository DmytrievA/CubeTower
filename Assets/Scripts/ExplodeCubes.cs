using System;
using System.Security.Cryptography;
using UnityEngine;

public class ExplodeCubes : MonoBehaviour
{
    public static EventHandler OnCubeExploded;

    private bool _collisionWasProcessed = false;
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Cube" && this._collisionWasProcessed == false)
        {
            //get all 'AllCubes' childs from end to start
            for (int i = collision.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = collision.transform.GetChild(i);
                //add phisics to eash children
                var rigidbody = child.gameObject.AddComponent<Rigidbody>();
                //add exposion forse to throw them up little bit
                rigidbody.AddExplosionForce(150f, Vector3.up, 10);
                //remove child from parent
                child.SetParent(null);
            }

            Destroy(collision.gameObject);
            this._collisionWasProcessed = true;

            //send event that cubes touched flor so need to show restart button
            if(OnCubeExploded != null)
                OnCubeExploded(this, null);
        }
    }
}
