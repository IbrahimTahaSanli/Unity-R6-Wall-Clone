using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float MovementSpeedMultiplier;
    [SerializeField] float LookSpeedMultiplier;
    [SerializeField] float BulletSpeed;

    [SerializeField] Camera Camera;

    [SerializeField] GameObject Bullet;

    Vector2 look;
    void Update()
    {
        Vector3 tmp = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            tmp += this.transform.forward ;
        if (Input.GetKey(KeyCode.S))
            tmp += -this.transform.forward;
        if (Input.GetKey(KeyCode.A))
            tmp += -this.transform.right;
        if (Input.GetKey(KeyCode.D))
            tmp += this.transform.right;
        this.transform.position += tmp.normalized * MovementSpeedMultiplier * Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.Tab))
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked? CursorLockMode.None: CursorLockMode.Locked;


        look.x += Input.GetAxis("Mouse X") * LookSpeedMultiplier;
        look.y -= Input.GetAxis("Mouse Y") * LookSpeedMultiplier;
        look.x = Mathf.Repeat(look.x, 360);
        look.y = Mathf.Repeat(look.y, 360);

        //look.y = Mathf.Clamp(look.y, -maxYAngle, maxYAngle);
        this.transform.rotation = Quaternion.Euler(0, look.x, 0);
        this.Camera.transform.rotation = Quaternion.Euler(look.y, look.x, 0);

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GameObject bullet = Instantiate(Bullet);
            bullet.transform.position = this.transform.position;
            bullet.GetComponent<Rigidbody>().AddForce(this.Camera.transform.forward * BulletSpeed);
        }
    }
}
