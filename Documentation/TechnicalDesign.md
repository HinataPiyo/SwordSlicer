# SwordSlicer 技術設計書

## 1. 開発環境・技術選定

| 項目 | 確認事項 |
|---|---|
| Unity | `6000.3.15f1`（ProjectVersion） |
| 言語 | C#、既定の `Assembly-CSharp` |
| 描画 | Universal Render Pipeline 17.3.0、URP 2D Renderer、Light2D |
| 入力 | Input System 1.19.0。`InputActionReference` とMouse/Pointer bindings。 |
| UI | UI Toolkit（UXML/USS、UIDocument、VisualTreeAsset）。TextMesh Pro uGUIもワールドポップアップに使用。 |
| 追加利用 | Cinemachine 3.1.7（カメラ揺れのPerlin）、Unity Animation／Animator、Physics2D OverlapCircle、Coroutine、PlayerPrefs／JsonUtility。 |
| 外部連携 | `com.unityroom.client`。結果画面からスコア送信。 |
| Build Profile | Android、Web Desktop DevelopmentがAssets/Settingsに存在。ビルド成功・端末試験は未確認。 |

Package manifestには2D Animation／Aseprite／PSD Importer／SpriteShape／Tilemap、Test Framework、Timeline、Recorder等もある。Packageが存在することと、本作コードで実際に利用していることは区別する。DOTS/ECS、Jobs/Burst、Addressables、ネットワーク対戦、UI Toolkit以外の全画面uGUI化は確認できない。

## 2. アーキテクチャと依存管理

プロジェクトは Systems・Service・Sword・Enemy・UI・ScriptableObject に分けた MonoBehaviour 中心構成。

**設計**：Scene の Bootstrap がインターフェース型を ServiceLocator に登録。機能側は ServiceLocator.Get<T>() で取得。

**利点**：具象クラス参照を隠蔽、サービス置換を容易化。

**制約**：登録順序・Scene 寿命が暗黙依存。登録漏れは Get<T>() の例外で検出。静的辞書の解除・テスト分離は別途対応が必要。

敵は基底クラスで共通初期化・移動・HP を実装、敵固有差分を派生クラスとフックで追加。ジェネリック制約 `where TData : EnemyDataSO` と実行時検証でデータ型不一致を検出。

## 3. データ設計

- **EnemyDataSO**：基本属性（名前・HP・到達時間・報酬）。派生 SO で敵固有パラメータ（ジャンプ、移動周期、無敵時間、攻撃方向、繁殖など）を追加。
- **EnemySpawnScheduleSO**：難易度・スポーン間隔・時間経過倍率・アンロック間隔・Prefab 重み。
- **SwordDataSO**：攻撃倍率・アイコン。
- **BattleSettingConfig**：剣種別・戦闘パラメータ・レベル別倍率配列。

ScriptableObject で Prefab・パラメータを Inspector で参照・再利用。ただし強化価格と LevelProperty 配列はコード定義のため、全データが Inspector 編集可能ではない。

## 4. ゲームロジック実装

### 剣
- 入力座標をドラッグ差分に変換し、投擲方向・速度を算出。
- 飛行中は反応時間を使った指数補間で回転入力に追従。
- 速度に応じた角度変化を積分して移動。
- 攻撃は独立コンポーネント（SwordAttack）の周期処理で Physics2D.OverlapCircle 判定。
- Rigidbody 移動ではなく Transform 直接更新。

### 敵
- SpawnController がスケジュール・経過時間から Prefab・倍率を決定、EnemyController へ初期化値を渡す。
- 基底 Movement は開始位置・ターゲット間の進捗を計算。
- Health は倍率適用 HP を管理。死亡時に AnimationDestroyEvent コールバックで削除・報酬・結果加算。

### ゲーム状態・UI
- GameOver / GameStop は static bool で Update 処理を停止。
- UI Toolkit は UXML テンプレートを C# で初期化、画面遷移を Stack で管理。
- 結果画面の値は ResultService.Entry に集約。

## 5. 保存とシーン遷移

**SaveAndLoadService**：強化スナップショット・通貨・音量を SaveData に集約、JsonUtility で文字列化、PlayerPrefs へ保存。

**セーブ**：ホーム UI から手動実行。自動セーブ・スキーマ Version・バックアップ・暗号化・クラウド保存は非対応。

**シーン遷移**：SceneManager.LoadScene で制御。結果画面は Build Settings の index 0/1 使用。

## 6. Unity 技術スタック

| 技術 | 用途 |
|---|---|
| MonoBehaviour | ゲーム動作・UI・サービスの主要構成。 |
| ScriptableObject | 敵・剣・スポーン・音声・チュートリアルデータ。 |
| C# event / Action | 通貨・体力・無敵・ゲーム終了・ロード通知。 |
| Coroutine | 剣ストック UI、カメラ揺れ、無敵時間、判定解除、通貨アニメーション。 |
| Input System | Pointer/Mouse 操作。 |
| UI Toolkit | ホーム・ゲーム中・結果・強化 UI。 |
| TextMesh Pro | ワールド空間ポップアップ。 |
| Animator / Animation Event | 敵挙動・死亡アニメーション、削除コールバック。 |
| Physics2D | OverlapCircle による攻撃対象検索。 |
| Object Pooling | WorldCanvasManager の通常ダメージテキスト（初期 10 個、枯渇時追加生成）。 |
| Cinemachine | Perlin ノイズ振幅をCoroutine で変更するカメラシェイク。 |
| Unityroom Client | スコア送信（結果画面）。 |

DOTS/ECS・Jobs/Burst・Addressables・ネットワーク対戦・全画面 uGUI 化は未使用。

## 7. パフォーマンスと実装上の注意点

- **SwordAttack**：対象 Collider がない場合、攻撃状態が継続するため毎フレーム OverlapCircle を実行。CPU 負荷の測定と最適化方針の決定が必要。
- **生成・削除**：敵・剣・エフェクト・ポップアップは Instantiate/Destroy 主体。通常ダメージ表示のみプール。
- **敵管理**：EnemySpawnController はList でList.Contains を毎回のアンロック確認時に実行。規模拡大時の計測推奨。
- **UI 更新**：AdjustAspectRatio.Update は画面寸法変化時のみ実行すべき。毎フレーム再計算は非効率。
- **入力バッファ**：RemoveAt による先頭削除、保持時間 0.3 秒で現状問題なし。
- **Gizmos**：開発時の範囲確認補助に活用。自動検証・データ生成ツールは限定的。
