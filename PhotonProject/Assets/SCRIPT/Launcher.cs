using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class Launcher : MonoBehaviourPunCallbacks
{
    [Header("輸出文字")]
    public Text textPrint;

    [Header("輸入欄位")]
    public InputField PlayerIF;
    public InputField roomCreateIF;
    public InputField roomJoinIF;
    public Button BtnCreate, BtnJoin;
    
    public string namePlayer, nameCreateRoom, nameJoinRoom;

    public string NamePlayer
    {
        get => namePlayer;
        set
        {
            namePlayer = value;
            PhotonNetwork.NickName = namePlayer;
                
        }
    }

    public string NameCreateRoom { get => nameCreateRoom; set => nameCreateRoom = value; }
    public string NameJoinRoom { get => nameJoinRoom; set => nameJoinRoom = value; }

    private void Start()
    {
        Screen.SetResolution(1280, 768, false); //螢幕設定解析度(寬，高，取消全螢幕)
        Connect();
    }

    public void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
        

    }

    public void BtnCreateRoom()
    {
        PhotonNetwork.CreateRoom(NameCreateRoom, new RoomOptions { MaxPlayers = 20 });
    }
    public void BtnJoinRoom()
    {
        PhotonNetwork.JoinRoom(NameJoinRoom);

    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        textPrint.text = "連線成功!";
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        textPrint.text = "已進入大廳!";
        PlayerIF.interactable = true;
        roomCreateIF.interactable = true;
        roomJoinIF.interactable = true;
        BtnCreate.interactable = true;
        BtnJoin.interactable = true;
    }

    public override void OnCreatedRoom()
    {
        textPrint.text = "已建立房間名：" + NameCreateRoom;
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        textPrint.text = "已加入房間名：" + NameJoinRoom;
        PhotonNetwork.LoadLevel("遊戲場景");

    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        textPrint.text = "加入房間失敗,Code：" + returnCode + "訊息:"+ message;
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        textPrint.text = "加入房間失敗,Code：" + returnCode + "訊息:" + message;
    }

}
