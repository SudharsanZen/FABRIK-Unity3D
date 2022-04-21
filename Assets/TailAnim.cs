using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailAnim : MonoBehaviour
{
    public Transform tailEnd;
    public float div=3;
   
    List<Transform> bones;
    List<Orientation> boneOrient;
    List<float> initDist;
    List<Vector3> tailPose;
    // Start is called before the first frame update
    private void Awake()
    {

        initDist = new List<float>();
        bones = new List<Transform>();
        boneOrient = new List<Orientation>();
        tailPose = new List<Vector3>();


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
            SphereCollider sp=bones[i].gameObject.AddComponent<SphereCollider>();
            sp.radius = 0.1f;
            tailPose.Add(bones[i].position);
        }
        for (int i = 1; i < bones.Count; i++)
        {
            initDist.Add((bones[i - 1].position - bones[i].position).magnitude);

        }

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        physics();
        //animateTail();
    }


    void physics()
    {
        
    }

    float waveFunction1(float x)
    {
        float val=Mathf.Sin(x/div);





        return val;
    }



    void animateTail()
    {
        float prevAng =0;
        for (int i = 0; i < bones.Count-1; i++)
        {
            float delX =0.1f;
            float x =((float)i/bones.Count)*3.0f*Mathf.PI+Time.time;
            float delY =waveFunction1(x+delX)-waveFunction1(x);

            float angle = Mathf.Abs(Mathf.Atan(delX/delY)*Mathf.Rad2Deg);
            Quaternion childWorldRot = bones[i + 1].parent.transform.rotation * bones[i + 1].localRotation;
            bones[i].transform.rotation =Quaternion.Euler(angle,0,0);
            prevAng =angle;



        }
    }


   
}
