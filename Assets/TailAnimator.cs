using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailAnimator : MonoBehaviour
{
    public float size=3;
    public float speed=10;
    public Transform tailEnd;
    public List<Transform> bones;
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
            tailPose.Add(bones[i].position);
        }
        for (int i = 1; i < bones.Count; i++)
        {
            initDist.Add((bones[i - 1].position - bones[i].position).magnitude);

        }

    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        tailWag();
        //reachbackwards(transform.position);
        align();
    }

    void reachbackwards(Vector3 startPose)
    {
        Vector3 targetPose = startPose;

        for (int i = 0; i < tailPose.Count - 2; i++)
        {
            Vector3 newPose = (tailPose[i + 1] - targetPose).normalized * initDist[i];

            tailPose[i] = targetPose;
            tailPose[i + 1]= targetPose + newPose;




            targetPose = tailPose[i + 1];
        }

        for (int i = 0; i < tailPose.Count - 2; i++)
            Debug.DrawLine(tailPose[i],tailPose[i+1],Color.green);
    }
    float time=0;

    void tailWag()
    {
        time += Time.deltaTime;
        int m = tailPose.Count;
        Vector3 first=Vector3.zero;
        for (int i = 0; i <m; i++)
        {
            //float fixSt = (1.0f - ((m - i) / m)) * (1.0f / (i + 1.0f));
            //float fixEnd = ((m + 5.0f - i) / m);
            Transform t = bones[i];
            
            float val = (Mathf.Sin(((float)i/size + time*speed))) * 1.0f;

           
            Vector3 rightDisp = Vector3.right*(val) ;
            Vector3 forward = Vector3.forward * Mathf.Sin((0.5f + i) / 2.0f + time * 2.0f)  ;
            if (i == 0)
                first = -rightDisp;
            bones[i].transform.position = tailPose[i] + rightDisp+first;


           

        }
    }

    void align()
    {
        //call this after backward and forward positional update to do the rotational update
        for (int i = 0; i < bones.Count - 1; i++)
        {
            Vector3 newPose;
           
            newPose = bones[i + 1].position - bones[i].position;



            Vector3 childWorldPose = bones[i + 1].position;
            Quaternion childWorldRot = bones[i + 1].parent.transform.rotation * bones[i + 1].localRotation;

            Vector3 rotAxis = Vector3.Cross(boneOrient[i].forward, newPose);
            float angle = Vector3.SignedAngle(boneOrient[i].forward, newPose, rotAxis);

            bones[i].rotation = Quaternion.AngleAxis(angle, rotAxis) * bones[i].rotation;

            bones[i + 1].rotation = childWorldRot;
            bones[i + 1].position = childWorldPose;

        }
    }
}
