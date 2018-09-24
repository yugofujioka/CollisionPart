using UnityEngine;
using TaskSystem;


/// <summary>
/// コリジョン管理
/// 真円と矩形（回転あり）のみ対応
/// </summary>
public sealed class CollisionPool {
    #region DEFINE
    public const int POOL_MAX = 100;   // 全体のコリジョン数
    public const int POOL_PL_MAX = 10; // 味方用のコリジョン数
    public const int POOL_EN_MAX = 10; // 敵用のコリジョン数
    #endregion


    #region MEMBER
    private Collision[] collisions = new Collision[POOL_MAX];
    private TaskSystem<Collision> pool = new TaskSystem<Collision>(POOL_MAX);       // 大元のプール
    private TaskSystem<Collision> players = new TaskSystem<Collision>(POOL_PL_MAX); // 味方
    private TaskSystem<Collision> enemies = new TaskSystem<Collision>(POOL_EN_MAX); // 敵
    private OrderHandler<Collision> playerHandler = null; // 味方コリジョンの処理
    private MatchHandler<Collision> scanHandler = null;   // Hitしたかのチェック処理
    private OrderHandler<Collision> hitHandler = null;    // Hitした際の処理

    private Collision ccol = null;  // チェックするコリジョン
    #endregion


    #region MAIN FUNCTION
    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize() {
        // 味方検査
        this.playerHandler = new OrderHandler<Collision>(this.PlayerOrder);
        // 接触判定
        this.scanHandler = new MatchHandler<Collision>(this.ScanOrder);
        // 接触処理
        this.hitHandler = new OrderHandler<Collision>(this.HitOrder);

        for (int i = 0; i < POOL_MAX; ++i) {
            this.collisions[i] = new Collision();
            this.pool.Attach(this.collisions[i]);
        }
    }

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="elapsedTime">経過時間</param>
    public void Proc(float elapsedTime) {
        this.players.Order(this.playerHandler); // 接触判定
    }
    #endregion


    #region PUBLIC FUNCTION
    /// <summary>
    /// コリジョンの取得
    /// </summary>
    /// <param name="category">コリジョン種別</param>
    public Collision PickOut(COL_CATEGORY category) {
        if (this.pool.count < 1) {
            Debug.Assert(false, "コリジョン不足");
            return null;
        }

        Collision col = this.pool.PickOutLast();
		col.enable = true;
        col.category = category;
		col.hitHandler = null;
        switch (category) {
            case COL_CATEGORY.PLAYER:
                this.players.Attach(col);
                break;
            case COL_CATEGORY.ENEMY:
                this.enemies.Attach(col);
                break;
        }

        return col;
    }

    /// <summary>
    /// 強制全回収
    /// </summary>
    public void Clear() {
        this.pool.Clear();
        this.players.Clear();
        this.enemies.Clear();
		for (int i = 0; i < POOL_MAX; ++i) {
			this.collisions[i].enable = false;
			this.pool.Attach(this.collisions[i]);
		}
    }
    #endregion


    #region PRIVATE FUNCTION
    /// <summary>
    /// 味方関連命令
    /// </summary>
    /// <param name="pcol">味方コリジョン</param>
    /// <param name="no">命令No.</param>
    /// <returns>引き続き有効か</returns>
    private bool PlayerOrder(Collision pcol, int no) {
        this.ccol = pcol;
        // 敵とのチェック
        this.enemies.ParticularOrder(this.scanHandler, this.hitHandler);

        if (!this.ccol.enable)
            this.pool.Attach(this.ccol);

        return this.ccol.enable;
    }

    /// <summary>
    /// 接触判定命令
    /// </summary>
    /// <param name="tcol">判定コリジョン</param>
    /// <returns>-1:中断, 0:未接触, 1:接触</returns>
    private int ScanOrder(Collision tcol) {
        if (!tcol.enable)
            return 0;

        if (!ccol.enable)
            return -1;

        return (Vector3.Distance(ccol.point, tcol.point) <= (ccol.range + tcol.range) ? 1 : 0);
    }
        
    /// <summary>
    /// 接触処理命令
    /// </summary>
    /// <param name="tcol">接触したコリジョン</param>
    /// <param name="no">命令No.</param>
    /// <returns>まだ有効か</returns>
    private bool HitOrder(Collision tcol, int no) {
        if (this.ccol.hitHandler != null)
            this.ccol.hitHandler(this.ccol, tcol);
        if (tcol.hitHandler != null)
            tcol.hitHandler(tcol, this.ccol);

        return true;
    }
    #endregion
}
