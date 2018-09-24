using UnityEngine;


public class GameManager : MonoBehaviour {
	public static CollisionPool collision;

	void Awake() {
		GameManager.collision = new CollisionPool();
		GameManager.collision.Initialize();
	}

	void Start() {
		// 1m = 1pixの位置にカメラを移動させる
		Camera cam = Camera.main;
		float dist = ((float)Screen.height * 0.5f) / Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
		cam.transform.localPosition = new Vector3(0f, 0f, -dist);
	}

	void Update() {
		float elapsedTime = Time.deltaTime;

		// コリジョンの更新（判定）
		GameManager.collision.Proc(elapsedTime);
	}
}
