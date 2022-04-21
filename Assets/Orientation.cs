using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orientation : MonoBehaviour
{

    public enum DefaultOrientation
    {
        ForwardNegativeZUpY,
        ForwardZUpY,
        ForwardXUpZ,
        ForwardXUpY,
        ForwardYUpZ
    };
    public DefaultOrientation defaultOrientation;
    public Vector3 Base;

    [HideInInspector]
    public Vector3 forward;
    [HideInInspector]
    public Vector3 up;
    [HideInInspector]
    public Vector3 right;

    [HideInInspector]
    public Vector3 baseUp;
    [HideInInspector]
    public Vector3 baseRight;
    [HideInInspector]
    public Vector3 baseForward;
   
    Quaternion rot;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        orientUpdate();
    }
    public void orientUpdate()
    {
        setBaseVectors();
        rot = Quaternion.Euler(Base.x, Base.y, Base.z);

        forward = rot * transform.rotation * baseForward;
        right = rot * transform.rotation * baseRight;
        up = rot * transform.rotation * baseUp;

        Debug.DrawRay(transform.position, forward, Color.blue);
        Debug.DrawRay(transform.position, right, Color.red);
        Debug.DrawRay(transform.position, up, Color.green);
    }
    void setBaseVectors()
    {
        switch (defaultOrientation)
        {
            case DefaultOrientation.ForwardNegativeZUpY:
                baseUp = Vector3.up;
                baseForward = -Vector3.forward;
                break;
            case DefaultOrientation.ForwardXUpY:
                baseUp = Vector3.up;
                baseForward = Vector3.right;
                break;
            case DefaultOrientation.ForwardXUpZ:
                baseUp = Vector3.forward;
                baseForward = Vector3.right;
                break;
            case DefaultOrientation.ForwardYUpZ:
                baseUp = Vector3.forward;
                baseForward = Vector3.up;
                break;
            case DefaultOrientation.ForwardZUpY:
                baseUp = Vector3.up;
                baseForward = Vector3.forward;
                break;


        }

        baseRight = Vector3.Cross( baseUp, baseForward);
    }
}