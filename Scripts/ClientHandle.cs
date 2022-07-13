using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour {
  public static void Welcome(Packet _packet) {
    string _msg = _packet.ReadString();
    int _myId = _packet.ReadInt();

    Debug.Log($"Message from server: {_msg}");
    Client.instance.myId = _myId;
    ClientSend.WelcomeReceived();

    Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
  }

  public static void SpawnPlayer(Packet _packet) {
    int _id = _packet.ReadInt();
    string _username = _packet.ReadString();
    bool turn = _packet.ReadBool();

    GameManager.instance.SpawnPlayer(_id, _username, turn);
  }
}
