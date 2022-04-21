using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tail : MonoBehaviour
{
    public Transform tailEnd;
    public bool useIk;
    public int count=10;
    List<Transform> bones;
    [Range(0.1f,1)]
    public float stiffness=0.1f;   
    List<Orientation> boneOrient;
    List<float> initDist;
    List<Vector3> lastPose;

    fabrik fik;
    // Start is called before the first frame update
    void Start()
    {
        fik = GetComponent<fabrik>();
        bones = new List<Transform>();
        initDist = new List<float>();
        bones = new List<Transform>();
        boneOrient = new List<Orientation>();
        lastPose = new List<Vector3>();


        Transform temp = tailEnd;
        while (transform.GetInstanceID() != temp.GetInstanceID())
        {
            bones.Add(temp);
            boneOrient.Add(temp.GetComponent<Orientation>());
            temp = temp.parent;

        }

        bones.Add(transform);
        boneOrient.Add(transform.GetComponent<Orientation>());
        bones.Reverse();
        boneOrient.Reverse();
        for (int i = 0; i < bones.Count; i++)
        {
            lastPose.Add(bones[i].position);
        }
        for (int i = 1; i < bones.Count; i++)
        {
            initDist.Add((bones[i - 1].position - bones[i].position).magnitude);
            //initDist.Add(bones[i].localPosition.magnitude);

        }
    }
    float fact = 0;
    // Update is called once per frame
    void Update()
    {

        /*
         * this for loop compares the positions of the bones in the previous frame,
         * uses that position to calculate  the position in which the bone needs to be in to be intact with the parent bones
         */
        for (int i = 1; i < bones.Count; i++)
        {
            Transform curr = bones[i];
            Transform prev = bones[i - 1];

            Orientation prevOrient=boneOrient[i-1];
            Vector3 dir = -(lastPose[i] - prev.position).normalized;

            float xOff = Vector3.Dot(dir, prevOrient.right);
            float yOff = Vector3.Dot(dir, -prevOrient.up);
            xOff = Mathf.Clamp(xOff, -1.0f, 1.0f);
            yOff = Mathf.Clamp(yOff, -1.0f, 1.0f);
            fact = (1.0f - ((float)i / (float)bones.Count));
            float x = Mathf.Asin(xOff) * stiffness *fact;
            float y = Mathf.Asin(yOff)*stiffness*fact;

            Quaternion qz = Quaternion.AngleAxis(-x*Mathf.Rad2Deg,-prevOrient.up);
           
            Quaternion qx = Quaternion.AngleAxis(y*Mathf.Rad2Deg,prevOrient.right);
         

        
            curr.position = (qz*qx )* (-dir * initDist[i - 1]) + prev.position;
         

            Debug.DrawLine(curr.position,prev.position,Color.red,0,false);
           
        }

        
        for (int i = 0; i < bones.Count; i++)
        {
            lastPose[i] = bones[i].position;
        }
        bones[0].rotation = Quaternion.AngleAxis(Mathf.Sin(Time.time * 5)*Time.deltaTime*50 , bones[0].right)*bones[0].rotation;
        if (fik != null && useIk)
            fik.up();
        align();
        

    }
    
    void align()
    {
        //call this to align the each bones's axis the the parent bone's axis
        for (int i = 1; i < bones.Count; i++)
        {
            Vector3 newPose;
            
            newPose = bones[i].position - bones[i-1].position;
            Vector3 childWorldPose=Vector3.zero;
            Quaternion childWorldRot=Quaternion.identity;
            if (i + 1 < bones.Count)
            {
                childWorldPose = bones[i + 1].position;
                childWorldRot = bones[i + 1].parent.transform.rotation * bones[i + 1].localRotation;
            }
            bones[i].rotation = getAxisAlignRot(bones[i - 1].rotation * Vector3.forward, -boneOrient[i].forward, boneOrient[i].right, stiffness) * bones[i].rotation;
            boneOrient[i].orientUpdate();
            bones[i].rotation =getAxisAlignRot(newPose,boneOrient[i].right,-boneOrient[i].up)* bones[i].rotation ;
                  
               
            if (i + 1 < bones.Count)
            {
                bones[i + 1].rotation = childWorldRot;
                bones[i + 1].position = childWorldPose;
            }
        }
    }


    Quaternion getAxisAlignRot(Vector3 alignTo,Vector3 ax1,Vector3 ax2,float amt=1)
    {
        //this function gives a rotation that aligns the  axis perpendicular to ax1 and ax2 along the alingTo vector
        Quaternion rot = Quaternion.Euler(0,0,0);

        float offX = Vector3.Dot(alignTo.normalized, ax1);
        float offZ = Vector3.Dot(alignTo.normalized, ax2);
        offX = Mathf.Clamp(offX, -1.0f, 1.0f);
        offZ = Mathf.Clamp(offZ, -1.0f, 1.0f);
        float angleX = Mathf.Asin(offX);
        float angleZ = Mathf.Asin(offZ);
        Quaternion qz = Quaternion.AngleAxis(-angleX * Mathf.Rad2Deg*amt, ax2);
        Quaternion qx = Quaternion.AngleAxis(angleZ * Mathf.Rad2Deg*amt, ax1);

        rot = qz * qx;

        return rot;
    }
}
