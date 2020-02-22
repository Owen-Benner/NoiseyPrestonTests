﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReplayDemon : MonoBehaviour
{

    public LogReader logReader;

    public FrameMovement frameMove;

    public FileWriter writer;

    private float startTime;

    private bool replaying = false;

    public Text rewardText;
    public Text scoreText;

    public string preRew;

    public int[] contexts;

    public int[] leftObjects;
    public int[] leftRewards;
    public int[] rightObjects;
    public int[] rightRewards;

    public GameObject contextN;
    public GameObject contextS;

    public GameObject objectNE;
    public GameObject objectSE;
    public GameObject objectSW;
    public GameObject objectNW;

    public string [] contextList;

    // Start is called before the first frame update
    void Start()
    {
        logReader = GameObject.Find("LogReader").GetComponent<LogReader>();
        frameMove = GetComponent<FrameMovement>();
        frameMove.enabled = true;
        writer = GameObject.Find("FileWriter").GetComponent<FileWriter>();
        startTime = writer.startTime;
        rewardText.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Skip to last applicable frame.
        while(logReader.frames[1].time < Time.time - startTime)
        {
            logReader.frames.RemoveAt(0);
        }
        //Check to read next frame.
        LogReader.Frame frame = logReader.frames[0];
        if(Time.time - startTime > frame.time)
        {
            if(replaying)
            {
                frameMove.MoveToFrame(frame.pose, frame.x, frame.z);
            }
            logReader.frames.RemoveAt(0);
        }

        //Check to read next selection.
        LogReader.Selection select = logReader.selects[0];
        if(Time.time - startTime > select.time)
        {
            rewardText.text = preRew + select.reward.ToString();
        }

        //Check to read next segment.
        LogReader.Segment seg = logReader.segs[0];
        if(Time.time - startTime > seg.time)
        {
            if(seg.segment == "Hallway")
            {
                if(seg.num == 0)
                {
                    replaying = true;
                }
                rewardText.enabled = false;
                contextN.SendMessage(contextList[contexts[seg.num]]);
                contextS.SendMessage(contextList[contexts[seg.num]]);
            }
            if(seg.segment == "HoldA")
            {
                contextN.SendMessage(contextList[0]);
                contextS.SendMessage(contextList[0]);

                objectNE.SendMessage("Sprite", leftObjects[seg.num]);
                objectSE.SendMessage("Sprite", rightObjects[seg.num]);
                //May wish to specify direction.
                objectSW.SendMessage("Sprite", leftObjects[seg.num]);
                objectNW.SendMessage("Sprite", rightObjects[seg.num]);
            }
            if(seg.segment == "Reward")
            {
                rewardText.enabled = true;
                scoreText.text = logReader.selects[0].score.ToString();
                logReader.selects.RemoveAt(0);

                int zero = 0;
                objectNE.SendMessage("Sprite", zero);
                objectSE.SendMessage("Sprite", zero);
                objectSW.SendMessage("Sprite", zero);
                objectNW.SendMessage("Sprite", zero);
            }
            if(seg.segment == "EndRun")
            {
                Debug.Log("End of replay.");
                Application.Quit();
            }
            logReader.segs.RemoveAt(0);
        }
    }

}
