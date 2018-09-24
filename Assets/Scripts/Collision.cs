using UnityEngine;


/// <summary>
/// コリジョン種別
/// </summary>
public enum COL_CATEGORY {
    PLAYER, // 味方本体
    ENEMY,  // 敵本体

    MAX,
}

/// <summary>
/// 接触コールバック
/// </summary>
/// <param name="atk">影響を与えるコリジョン</param>
/// <param name="def">影響を受けるコリジョン</param>
public delegate void HitHandler(Collision atk, Collision def);

/// <summary>
/// 当たり判定
/// </summary>
public sealed class Collision {
    public bool enable = false;  // 有効フラグ
    public COL_CATEGORY category = COL_CATEGORY.PLAYER; // コリジョンの分類
    public Vector3 point = Vector3.zero; // 座標
    public float range = 0f;     // 半径
    public HitHandler hitHandler = null; // 接触した際のコールバック
}
