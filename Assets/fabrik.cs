using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fabrik : MonoBehaviour
{
    // Start is called before the first frame update
    public bool usedByTail;
    public bool usePole=false;
    public Transform pole;

    public Transform root;
    public bool isRoot=true;
    public Transform ikEnd;
   
    public Transform target;
    List<Transform> bones;
    List<Orientation> boneOrient;
    List<float> initDist;

    private void Awake()
    {
       
        initDist = new List<float>();
        bones = new List<Transform>();
        boneOrient = new List<Orientation>();
        if(isRoot)
            root = transform;
        

        Transform temp = ikEnd;
        while (transform.GetInstanceID() != temp.GetInstanceID())
        {
            bones.Add(temp);
            boneOrient.Add(temp.GetComponent<Orientation>());
            temp = temp.parent;
           
        }
        if (isRoot)
        {
            bones.Add(root);
            boneOrient.Add(root.GetComponent<Orientation>());
        }
        else
        {
            bones.Add(transform);
            boneOrient.Add(GetComponent<Orientation>());
            bones.Add(root);
            boneOrient.Add(root.GetComponent<Orientation>());
        }

        bones.Reverse();
        boneOrient.Reverse();

        for (int i = 1; i < bones.Count; i++)
        {
            initDist.Add((bones[i-1].position-bones[i].position).magnitude);

        }

    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() 
    {

        //align();
        if (!usedByTail)
            up();
      
        //lockAxis();
    }

    public void up()
    {
        Vector3 startPose = root.position;

        reachForward();
        reachbackwards(startPose);
        if (usePole)
            alignToPole();
    }

    void align()
    {
        //call this after backward and forward positional update to do the rotational update
        for (int i = 0; i < bones.Count - 1; i++)
        {
            Vector3 newPose;
            if (i == bones.Count - 1)
                newPose =target.position-bones[i].position;
            else
                newPose =bones[i+1].position-bones[i].position;

          
            
            Vector3 childWorldPose = bones[i + 1].position;
            Quaternion childWorldRot = bones[i + 1].parent.transform.rotation * bones[i + 1].localRotation;

            Vector3 rotAxis = Vector3.Cross(boneOrient[i].forward, newPose);
            float angle = Vector3.SignedAngle(boneOrient[i].forward, newPose, rotAxis);

            bones[i].rotation = Quaternion.AngleAxis(angle, rotAxis) * bones[i].rotation;

            bones[i + 1].rotation = childWorldRot;
            bones[i + 1].position = childWorldPose;

        }
    }


    void alignToPole()
    {
        for (int i = 1; i < bones.Count-1; i++)
        {
            Transform prev = bones[i - 1];
            Transform curr = bones[i];
            Transform next = bones[i + 1];

            Vector3 nPose = next.position;
    
            Plane projPlane =new Plane((next.position-prev.position).normalized,prev.position);
            
            Vector3 currProj =projPlane.ClosestPointOnPlane(curr.position);
            Vector3 poleProj = projPlane.ClosestPointOnPlane(pole.position);

            Debug.DrawLine(prev.position,currProj);
            Debug.DrawLine(currProj,curr.position);
            float angle = Vector3.SignedAngle((currProj-prev.position).normalized,(poleProj-prev.position).normalized,projPlane.normal);
            
            Quaternion rot =Quaternion.AngleAxis(angle,projPlane.normal);
            Vector3 finalPose =rot*(curr.position-prev.position);
            curr.transform.position =finalPose+prev.position;
            next.position = nPose;
        }
    }
    void reachForward()
    {
        Vector3 targetPose = target.position;

        for (int i = bones.Count-1 ; i>0 ; i--)
        {
            
            Vector3 newPose = (bones[i-1].position - targetPose).normalized*initDist[i-1];

            bones[i].position = targetPose;
            Vector3 childWorldPose = bones[i].position;

            bones[i - 1].position = targetPose + newPose;
  
            bones[i].position = childWorldPose;

            targetPose = bones[i - 1].position;

        }
    }


    void reachbackwards(Vector3 startPose)
    {
        Vector3 targetPose = startPose;

        for (int i = 0; i < bones.Count-2; i++)
        {
            Vector3 newPose = (bones[i + 1].position - targetPose).normalized * initDist[i];

            bones[i].position = targetPose;
            bones[i + 1].position = targetPose + newPose;


           

            targetPose = bones[i+1].position;
        }
    }

    void lockAxis(float lockAngle=30)
    {
        for (int i = bones.Count-1; i>0; i--)
        {
            Vector3 currAxis = boneOrient[i].right;
            Vector3 prevAxis = boneOrient[i-1].right;

            float angle = Vector3.SignedAngle(currAxis,prevAxis,boneOrient[i].forward);
            
            Quaternion rot = Quaternion.AngleAxis(angle,boneOrient[i].forward);
            bones[i].rotation = rot * bones[i].rotation;
        }
    }
}
