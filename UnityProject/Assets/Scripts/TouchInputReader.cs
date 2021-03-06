﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TouchInputReader : PhotonBehaviour {
    
	// Use this for initialization
	void Start () {
        if (photonView.isMine)
        {
            // FindObjectOfType<Text>().text = "starting..";
        }

        virtualpad = new Rect(Screen.width / 2f, 0, Screen.width / 2f, Screen.height);
	}

    Rect virtualpad;

    public float lastX, lastY;
    public bool unhandledDoubleTap = false;
	
	// Update is called once per frame
	void Update () {
        if (!photonView.isMine)
        {
            return;
        }

        bool touchedPad = false;

        for(int i=0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);

            if (!virtualpad.Contains(t.position))
            {
                continue;
            }

            Vector2 screenCenter = virtualpad.center;
            
            // just use as moving
            lastX = (t.position.x - screenCenter.x) / screenCenter.x;
            lastY = (t.position.y - screenCenter.y) / screenCenter.y;

            touchedPad = true;

            // Debug.Log("read " + CoordsToString());
            FindObjectOfType<Text>().text = CoordsToString();
            
        }
        if (!touchedPad)
        {
            lastX = 0;
            lastY = 0;
        }
	}

    public void DoDoubleTap()
    {
        RPC(DoubleTap, PhotonTargets.MasterClient);
    }

    [RPC]
    public void DoubleTap()
    {
        // Debug.Log("doubletapped!");
        unhandledDoubleTap = true;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(lastX);
            stream.SendNext(lastY);
            // Debug.Log("sent " + CoordsToString());
        }
        else
        {
            lastX = (float)stream.ReceiveNext();
            lastY = (float)stream.ReceiveNext();
            // Debug.Log("received" + CoordsToString());
        }
    }

    private string CoordsToString()
    {
        return lastX.ToString("0.000") + ", " + lastY.ToString("0.000");
    }
}
