using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
namespace amos
{
    public class Player : MonoBehaviourPun,IPunObservable
    {
        #region 欄位
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
        [Header("同步平滑速度"), Range(0.1f, 20f)]
        public float smoothSpeed = 0.5f;
        [Header("圖片渲染器")]
        public SpriteRenderer sr;
        [Header("玩家名稱介面")]
        public Text textName;
        [Header("生成子彈位置")]
        public Transform pointBullet;
        [Header("子彈")]
        public GameObject bullet;
        private Text textCCU;
        [Header("中心點")]
        public Transform pointCenter;
        #endregion

        #region 事件
        private void Start()
        {
            //如果 不是自己的物件
            if (!pv.IsMine)
            {
               //player.enabled = false; //玩家元件=關閉
                obj.SetActive(false);   //攝影機物件(關閉)
                textName.text=pv.Owner.NickName; 
            }
            //否則 是自己的物件
            else
            {
                //玩家名稱介面.文字 = 伺服器.暱稱
                textName.text = PhotonNetwork.NickName;
                uiPlayer.SetActive(true);
            }

            //連線人數介面 = 遊戲物件.尋找("物件名稱").取得元件<元件類型>
            textCCU = GameObject.Find("連線人數").GetComponent<Text>();
        }
        private void FixedUpdate()
        {
            //如果是自己的物件 執行 移動
            if (pv.IsMine)
            {
                Move();
                FlipSprite();
                Shoot();
                RotateWeapon();
            }
            else
            {
                SmoohtMove();
                SmoothRotateWeapon();

            }
            //PhotonNetwork.CurrentRoom.PlayerCount 伺服器.當前房間.玩家數
            textCCU.text = PhotonNetwork.CurrentRoom.PlayerCount + "/20";

            
        }
        #endregion

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "子彈")
            {
                hp -= 10;
                imageHp.fillAmount = hp/maxHp;
                textHp.text = "HP " + hp + " / " + maxHp;
                if (hp <= 0) Dead();
            }
        }

        private void Dead()
        {
            if (pv.IsMine)  //如果 是自己的物件
            {
                PhotonNetwork.LeaveRoom();          //伺服器.離開房間
                PhotonNetwork.LoadLevel("大廳");    //伺服器.載入大廳
            }
        }

        /// <summary>
        /// 其他玩家的物件同步平滑移動
        /// </summary>
        private void SmoohtMove()
        {
            //其他玩家的座標=插值 (原本的座標，同步座標資訊，百分比-同步平滑速度*一個影格的時間)
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
        [Header("平滑旋轉武器"), Range(0.1f, 20)]
        public float smoothROtateSpeed = 15;

        private void SmoothRotateWeapon()
        {
            //中心點.前方 = 二為向量.插值(中心點.前方，方向，平滑速度*1/60)
            pointCenter.right = Vector2.Lerp(pointCenter.right ,direction,smoothROtateSpeed* Time.deltaTime );//乘上Time.deltaTime用意是讓移動效果更好 官方建議使用

        }
        //同步資料方法( Player : MonoBehaviourPun,IPunObservable寫好後按燈泡 實作  刪掉裡面那行程式)
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(transform.position);            //傳遞資料(座標)
                stream.SendNext(direction);                     //傳遞資料(武器方向)
            }
            else if (stream.IsReading)
            {
                positionNext = (Vector3)stream.ReceiveNext();   //同步座標資訊=(轉型)接收資料
                direction = (Vector2)stream.ReceiveNext();      //同步武器方向資訊=(轉型)接收資料
            }
        }
       


        private void Shoot()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                //伺服器.實例化(物件名稱，座標，角度)
                PhotonNetwork.Instantiate(bullet.name, pointBullet.position, pointBullet.rotation);
            }
        }
        //二維向量 方向
        private Vector2 direction;

        /// <summary>
        /// 旋轉武器
        /// </summary>
        private void RotateWeapon()
        {
            //取得滑鼠座標，屬於螢幕座標
            Vector3 posMouse =  Input.mousePosition;

            //將螢幕座標轉為世界座標
            Vector3 posWorld = Camera.main.ScreenToWorldPoint(posMouse);
            
            //計算方向 = 滑鼠X - 中心點X ， 滑鼠Y - 中心點Y
            direction = new Vector2(posWorld.x - pointCenter.position.x, posWorld.y - pointCenter.position.y);

            //中心點.方向 = 計算方向
            //x軸 紅  pointCenter.right
            //y軸 綠  pointCenter.up
            //Z軸 藍  pointCenter.forward
            pointCenter.right = direction;
        }
        [Header("玩家介面")]
        public GameObject uiPlayer; //整組介面
        public Image imageHp;       //血量圖片
        public Text textHp;         //血量文字
        public float hp = 100;      //當前血量
        private float maxHp = 100;  //最大血量
    }
}
