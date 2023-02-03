using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager
{
    MyPlayer myPlayer;
    Dictionary<int, Player> players = new Dictionary<int, Player>();

    public static PlayerManager Instance { get; } = new PlayerManager();

    public void Add(S_PlayerList packet)
    {
        Object obj = Resources.Load("Player");

        foreach (S_PlayerList.Player p in packet.players)
        {
            GameObject go = Object.Instantiate(obj) as GameObject;

            if (p.isSelf)
            {
                MyPlayer myPlayer = go.AddComponent<MyPlayer>();
                myPlayer.PlayerId = p.playerId;
                myPlayer.transform.position = new Vector3(p.posX, p.posY, p.posZ);
                this.myPlayer = myPlayer;
            }
            else
            {
                Player player = go.AddComponent<Player>();
                player.PlayerId = p.playerId;
                player.transform.position = new Vector3(p.posX, p.posY, p.posZ);
                players.Add(p.playerId, player);
            }
        }
    }

    public void Move(S_BroadcastMove packet)
    {
        if (myPlayer.PlayerId == packet.playerId)
        {
            myPlayer.transform.position = new Vector3(packet.posX, packet.posY, packet.posZ);
        }
        else
        {
            if (players.TryGetValue(packet.playerId, out Player player))
            {
                player.transform.position = new Vector3(packet.posX, packet.posY, packet.posZ);
            }
        }
    }

    public void EnterGame(S_BroadcastEnterGame packet)
    {
        if (packet.playerId == myPlayer.PlayerId)
            return;

        Object obj = Resources.Load("Player");
        GameObject go = Object.Instantiate(obj) as GameObject;

        Player player = go.AddComponent<Player>();
        player.transform.position = new Vector3(packet.posX, packet.posY, packet.posZ);
        players.Add(packet.playerId, player);
    }

    public void LeaveGame(S_BroadcastLeaveGame packet)
    {
        if (myPlayer.PlayerId == packet.playerId)
        {
            GameObject.Destroy(myPlayer.gameObject);
            myPlayer = null;
        }
        else
        {
            if (players.TryGetValue(packet.playerId, out Player player))
            {
                GameObject.Destroy(player.gameObject);
                players.Remove(packet.playerId);
            }
        }
    }
}
