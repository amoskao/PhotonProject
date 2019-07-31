using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviour
{
    [Header("速度")]
    public float speed = 10;
    private void Update()
    {
        Move();   
    }

    /// <summary>
    /// 當物件碰撞開始時會執行一次
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        CancelInvoke();         //取消所有Invoke
        //伺服器.刪除(物件)
        //PhotonNetwork.Destroy(gameObject);
        Invoke("DelayDestiory", 0.1f);
    }

    private void Start()
    {
        Invoke("DelayDestiory", 1);//photon API 刪除必須透過其他方式延遲- Invoke("方法名稱"，延遲時間)
    }

    private void DelayDestiory()
    {
        PhotonNetwork.Destroy(gameObject);
    }
    /// <summary>
    /// 子彈移動
    /// </summary>
    private void Move()
    {
        //變形元件.移動(子彈.右邊 * 1/60 * 速度，空間.世界座標)
        //x軸 紅 transform.right
        //y軸 綠 transform.up
        //Z軸 藍 transform.forward
        transform.Translate(transform.right * Time.deltaTime * speed,Space.World);
    }
}
