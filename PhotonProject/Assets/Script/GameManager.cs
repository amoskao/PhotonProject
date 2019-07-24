using UnityEngine;
using Photon.Pun;
public class GameManager : MonoBehaviour
{
    [Header("玩家預製物")]
    public GameObject prefabPlayer;
    [Header("生成座標")]
    public Transform[] spawnPoints;

    private void SpawnPlayer()
    {
        //隨機= 隨機(0，生成座標陣列的長度)、(0，6)結果為0~5隨機值
        int r = Random.Range(0, spawnPoints.Length);
        //photon連線.實例化(物件名稱，座標，角度) Quaternion.identity= 沒有角度
        PhotonNetwork.Instantiate(prefabPlayer.name, spawnPoints[r].position, Quaternion.identity);
    }
    private void Start()
    {
        SpawnPlayer();
    }
}
