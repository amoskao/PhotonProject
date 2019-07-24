using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
namespace amos
{
    public class Player : MonoBehaviourPun,IPunObservable
    {
        [Header("剛體")]
        public Rigidbody2D rig;
        [Header("速度")]
        public float speed = 10;

        [Header("Photon元件")]
        public PhotonView pv;

        [Header("Player腳本")]
        public Player player;

        [Header("攝影機")]
        public GameObject obj;
        [Header("同步座標資訊")]
        public Vector3 positionNext;
        [Header("同步平滑速度"), Range(0.1f, 5f)]
        public float smoothSpeed = 0.5f;
        [Header("圖片渲染器")]
        public SpriteRenderer sr;

        [Header("玩家名稱介面")]
        public Text textName;

        private void Start()
        {
            //如果 不是自己的物件
            if (!pv.IsMine)
            {
               //player.enabled = false; //玩家元件=關閉
                obj.SetActive(false);   //攝影機物件(關閉)
                textName.text=pv.Owner.NickName;
            }
            else
            {
                textName.text = PhotonNetwork.NickName;
            }
                
        }
        private void FixedUpdate()
        {
            if (pv.IsMine)
            {
                Move();
                FlipSprite();
            }
            else
            {
                SmoohtMove();
            }
        }


        /// <summary>
        /// 其他玩家的物件同步平滑移動
        /// </summary>
        private void SmoohtMove()
        {
            //其他玩家的座標=插植(原本的座標，同步座標資訊，百分比-同步平滑速度*一個影格的時間)
            transform.position = Vector3.Lerp(transform.position, positionNext, smoothSpeed * Time.deltaTime);//乘上Time.deltaTime用意是讓移動效果更好 官方建議使用
        }

        private void Move()
        {

            rig.AddForce((
                transform.right * Input.GetAxisRaw("Horizontal") +
                transform.up * Input.GetAxisRaw("Vertical")) * speed);
        }

        [PunRPC]
        private void RPCFlipSprite(bool flip)
        {
            sr.flipX = flip;
        }

        private void FlipSprite()
        {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                sr.flipX = false;
                pv.RPC("RPCFlipSprite", RpcTarget.All, false);
            }
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                sr.flipX = true;
                pv.RPC("RPCFlipSprite", RpcTarget.All, true);

            }
        }

        //同步資料方法( Player : MonoBehaviourPun,IPunObservable寫好後按燈泡 實作  刪掉裡面那行程式)
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(transform.position);            //傳遞資料(座標)
            }
            else if (stream.IsReading)
            {
                positionNext = (Vector3)stream.ReceiveNext();   //同步座標資訊=(轉型)接收資料
            }
        }

        

                    
    }
}
