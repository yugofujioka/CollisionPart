using UnityEngine;


/// <summary>
/// 当たり判定位置
/// </summary>
public sealed class CollisionPart : MonoBehaviour {
    #region DEFINE
    /// <summary>
    /// コリジョン設定
    /// </summary>
    [System.Serializable]
    public struct CollisionData {
        public float range;    // 範囲
        public Vector3 offset; // 平行移動
    }
    #endregion


    #region MEMBER
    [SerializeField, Tooltip("コリジョン設定")]
    private CollisionData[] collisionDatas = new CollisionData[0];

    [System.NonSerialized]
    public Vector3 centerPoint = Vector3.zero;

    private Transform trans_ = null;       // Transformキャッシュ
    private Camera camera_ = null;         // 表示カメラキャッシュ
    private Transform cameraTrans = null;  // 表示カメラTransformキャッシュ
    private COL_CATEGORY category = COL_CATEGORY.PLAYER; // コリジョン分類
    private Collision[] collisions = null; // コリジョンリスト
    private HitHandler hitHandler = null;  // 接触処理
    private int collisionCount = 0;        // コリジョン数
    private bool awake = false;            // 起動済フラグ
    #endregion


    #region PROPERTY
    /// <summary> 起動中か </summary>
    public bool isAwake { get { return this.awake; } }
    #endregion


    #region MAIN FUNCTION
    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="category">コリジョンカテゴリ</param>
    /// <param name="hitHandler">接触処理</param>
    public void Initialize(COL_CATEGORY category, HitHandler hitHandler) {
        this.trans_ = this.transform;
        this.category = category;
        this.hitHandler = hitHandler;
        this.collisionCount = this.collisionDatas.Length;
        this.collisions = new Collision[this.collisionCount];
    }

    /// <summary>
    /// 全起動
    /// </summary>
    public void WakeUp() {
        Debug.Assert(!this.awake, "CollisionPart 二重起動");

        Vector3 worldPoint = this.trans_.position;

        this.camera_ = Camera.main;
        this.cameraTrans = this.camera_.transform;

        // コリジョン呼び出し
        this.centerPoint = this.camera_.WorldToScreenPoint(worldPoint);
        for (int i = 0; i < this.collisionCount; ++i) {
            this.collisions[i] = GameManager.collision.PickOut(this.category);
            if (this.collisions[i] != null) {
                this.collisions[i].enable = true;
                this.collisions[i].range = this.collisionDatas[i].range;
                this.collisions[i].hitHandler = this.hitHandler;

                Vector3 offset = this.collisionDatas[i].offset;
                this.collisions[i].point.x = this.centerPoint.x + offset.x;
                this.collisions[i].point.y = this.centerPoint.y + offset.y;
            }
        }
        this.awake = true;
    }

    /// <summary>
    /// 全停止
    /// </summary>
    public void Sleep() {
        for (int i = 0; i < this.collisionCount; ++i) {
            if (this.collisions[i] != null) {
                // コリジョン返却
                this.collisions[i].enable = false;
                this.collisions[i] = null;
            }
        }

        this.camera_ = null;    // MEMO: 自分外の参照を残さない
        this.cameraTrans = null;
        this.awake = false;
    }

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="elapsedTime">経過時間</param>
    public void Run(float elapsedTime) {
        if (!this.awake)
            return;

		Vector3 worldPoint = this.trans_.position;
        Vector3 centerPoint = this.camera_.WorldToScreenPoint(worldPoint);
        this.centerPoint = centerPoint;

		Quaternion inv = Quaternion.Inverse(this.cameraTrans.rotation);
        Vector3 point = inv * (worldPoint - this.cameraTrans.position);
        float distance = Mathf.Abs(point.z);
		float rad = this.camera_.fieldOfView * 0.5f * Mathf.Deg2Rad;
        float standardDistance = ((float)Screen.height * 0.5f) / Mathf.Tan(rad);
        float actualScale = standardDistance / distance;

        for (int i = 0; i < this.collisionCount; ++i) {
            this.collisions[i].range = this.collisionDatas[i].range * actualScale;
            Vector3 offset = this.collisionDatas[i].offset * actualScale;
            this.collisions[i].point.x = this.centerPoint.x + offset.x;
            this.collisions[i].point.y = this.centerPoint.y + offset.y;
        }
    }
    #endregion
}
