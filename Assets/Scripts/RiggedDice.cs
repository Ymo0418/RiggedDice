using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RiggedDice : MonoBehaviour
{
    public RotationTable rt;

    KeyCode[] keyCodes = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, 
                            KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6 };

    public class Recorded
    {
        public Vector3 pos;
        public Quaternion rot;

        public Recorded(Vector3 _pos, Quaternion _rot) { pos = _pos; rot = _rot; }
    }

    Queue<Recorded> recordDataQueue = new Queue<Recorded>(500);

    Rigidbody rb;

    [SerializeField] GameObject parentObj;

    public Transform[] detectors = new Transform[6];

    bool onRecording = false;

    void Awake()
    {
        rb = parentObj.GetComponent<Rigidbody>();
    }

    void Update()
    {
        //Check pressing number 1,2,3,4,5,6
        for (int i = 0; i < keyCodes.Length; i++)
        {
            if (Input.GetKeyUp(keyCodes[i]))
            {
                SimulateDice(i+1);
            }
        }
    }

    void FixedUpdate()
    {
        if (onRecording) 
            return;

        if(recordDataQueue.Count > 0) 
            FollowRecordData();
    }

    public void SimulateDice(int rigged)
    {
        recordDataQueue.Clear();
        onRecording = true;
        
        //Physics now not visible
        Physics.simulationMode = SimulationMode.Script;
        
        RollDice();

        Recorded recordData;
        for (int i = 0; i < 500; i++)
        {
            Vector3 pos = parentObj.transform.position;
            Quaternion rot = parentObj.transform.rotation;

            recordData = new Recorded(pos, rot);

            recordDataQueue.Enqueue(recordData);
            Physics.Simulate(Time.fixedDeltaTime);
        }

        if (FindUpperSide() != rigged)
            RotateDiceToRigged(rigged);

        //Physics now visible
        Physics.simulationMode = SimulationMode.FixedUpdate;
        onRecording = false;
    }

    void FollowRecordData()
    {   
        Recorded recordData = recordDataQueue.Dequeue();
        parentObj.transform.position = recordData.pos;
        parentObj.transform.rotation = recordData.rot;
    }

    void RollDice()
    {
        parentObj.transform.position = new Vector3(0, 4, 0);
        int x = Random.Range(0, 360);
        int y = Random.Range(0, 360);
        int z = Random.Range(0, 360);
        Quaternion rot = Quaternion.Euler(x, y, z);

        x = Random.Range(0, 25);
        y = Random.Range(0, 25);
        z = Random.Range(0, 25);
        Vector3 force = new Vector3(x, -y, z); //To drop dice early

        x = Random.Range(0, 50);
        y = Random.Range(0, 50);
        z = Random.Range(0, 50);
        Vector3 torque = new Vector3(x, y, z);

        parentObj.transform.rotation = rot;
        rb.velocity = force;
        rb.AddTorque(torque, ForceMode.VelocityChange);
    }

    public void RotateDiceToRigged(int riggedNum)
    {
        Vector3 v3 = rt.needRotation[FindUpperSide() - 1].to[riggedNum - 1];
        transform.Rotate(v3);
    }


    //Return upper side number
    int FindUpperSide()
    {
        int res = 0;

        for(int i = 0; i < 6; i++)
        {
            float y = detectors[res].position.y;

            if (detectors[res].position.y < detectors[i].position.y)
            {
                res = i;
            }
        }

        return res+1;
    }
}
