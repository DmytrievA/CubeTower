using UnityEngine;

public class RotateScript : MonoBehaviour
{
    public float Speed = 5f;

    private Transform rotatorTransform;

    private void Start()
    {
        this.rotatorTransform = this.GetComponent<Transform>();
    }

    private void Update()
    {
        this.rotatorTransform.Rotate(0, this.Speed * Time.deltaTime, 0);
    }
}
