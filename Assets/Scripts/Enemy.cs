using UnityEngine;


public class Enemy : MonoBehaviour {
	CollisionPart col = null;

    void Start() {
		// コリジョンの呼び出し
		this.col = this.GetComponentInChildren<CollisionPart>(true);
		this.col.Initialize(COL_CATEGORY.ENEMY, HitCallback);
		this.col.WakeUp();
    }

	void OnDestroy() {
		// コリジョンの返却
		this.col.Sleep();
	}

	void Update() {
		float elapsedTime = Time.deltaTime;
		this.col.Run(elapsedTime);
	}
	
	/// <summary>
	/// 接触コールバック
	/// </summary>
	/// <param name="atk">影響を与えるコリジョン</param>
	/// <param name="def">影響を受けるコリジョン</param>
	private static void HitCallback(Collision atk, Collision def) {
		// atkに自身、defに相手（この場合は敵）が受け渡される
		Debug.LogWarning("ENEMY HIT !!!");

		atk.enable = false;	// 自身のコリジョンの返却
	}
}