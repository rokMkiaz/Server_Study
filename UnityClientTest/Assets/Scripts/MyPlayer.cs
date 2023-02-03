using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : Player
{
    NetworkManager network;

    void Start()
    {
        StartCoroutine(CoSendPacket());
        network = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }

    void Update()
    {
        
    }

    IEnumerator CoSendPacket()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.25f);

            C_Move movePacket = new C_Move();
            movePacket.posX = Random.Range(-50f, 50f);
            movePacket.posY = 0;
            movePacket.posZ = Random.Range(-50f, 50f);
            network.Send(movePacket.Write());
        }
    }
}
