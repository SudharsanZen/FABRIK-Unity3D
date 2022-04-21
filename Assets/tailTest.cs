using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tailTest : MonoBehaviour
{
    public float length = 2;
    public int count = 10;
    List<Transform> bones;
    List<Vector3> lastPose;
    // Start is called before the first frame update
    void Start()
    {
        lastPose = new List<Vector3>();
        bones = new List<Transform>();
        GameObject gb=GameObject.CreatePrimitive(PrimitiveType.Cube);
        for (int i = 0; i < count; i++)
        {
            if (i == 0)
            {
                GameObject b = Instantiate(gb, transform.position + transform.forward * length * i, transform.rotation, transform);
                bones.Add(b.transform);
            }
            else
            {
                GameObject b = Instantiate(gb, transform.position + transform.forward * length * i, bones[i-1].rotation, bones[i-1]);
                bones.Add(b.transform);
            }
        }
        for (int i = 0; i < bones.Count; i++)
        {
            lastPose.Add(bones[i].position);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /*for (int i = 1; i < bones.Count; i++)
        {
            Transform curr = bones[i];
            Transform prev = bones[i - 1];
          
            Vector3 dir = (curr.position - prev.position).normalized;

            Vector3 axis = Vector3.Cross(dir, prev.forward);
            float ang = Vector3.SignedAngle(dir, prev.forward, axis)*Time.deltaTime*10.0f;
            Quaternion rot = Quaternion.AngleAxis(ang , axis);

            curr.position = rot * dir * length + prev.position;

            //make forward component of bone to point towards the prev bone
            axis = Vector3.Cross(curr.forward, dir);
            ang = Vector3.SignedAngle(curr.forward, dir, axis);
            rot = Quaternion.AngleAxis(ang, axis);
            curr.rotation = curr.rotation * rot;

            //align all the bone's up axis
            axis = Vector3.Cross(curr.right, prev.right);
            ang = Vector3.SignedAngle(curr.right, prev.right, axis) * 0.1f;
            rot = Quaternion.AngleAxis(ang, axis);
            curr.rotation = curr.rotation * rot;



        }*/
        for (int i = 1; i < bones.Count; i++)
        {
            Transform curr = bones[i];
            Transform prev = bones[i - 1];

            Vector3 dir = -(lastPose[i] - prev.position).normalized;

            float xOff = Vector3.Dot(dir, prev.right);
            float zOff = Vector3.Dot(dir, prev.forward);
            xOff = Mathf.Clamp(xOff, -1.0f, 1.0f);
            zOff = Mathf.Clamp(zOff, -1.0f, 1.0f);
            float x = Mathf.Asin(xOff) / Mathf.PI;
            float z = Mathf.Asin(zOff) / Mathf.PI;

            Quaternion qz = Quaternion.Euler(Mathf.Rad2Deg * z, 0, Mathf.Rad2Deg * x);
            //Quaternion qx = Quaternion.Euler(Mathf.Rad2Deg * z * 0.1f, 0, 0);

            //curr.localRotation = (qx*qz)*curr.localRotation;
            curr.position = qz * (-dir * length) + prev.position;
            /* Vector3 axis = Vector3.Cross(dir, prev.forward);
             float ang = Vector3.SignedAngle(dir, prev.forward, axis) * Time.deltaTime * 10.0f;
             Quaternion rot = Quaternion.AngleAxis(ang, axis);

             curr.position = rot * dir * length + prev.position;

             float xOff =Vector3.Dot(dir,prev.right);
             float yOff =Vector3.Dot(dir,prev.up);
             float x = Mathf.Asin(xOff)/3.14f;
             float y = Mathf.Asin(yOff)/3.14f;
             Quaternion qy =Quaternion.Euler(0,Mathf.Rad2Deg*x*0.1f,0);
             Quaternion qx =Quaternion.Euler(-Mathf.Rad2Deg*y*0.1f,0,0);

             curr.rotation =qy*qx*curr.rotation;
            */

        }


        for (int i = 0; i < bones.Count; i++)
        {
            lastPose[i] = bones[i].position;
        }
    }
}
