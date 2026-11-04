# 主要クラスリファレンス

## ゲームループ中核クラス

### SwordControl

ファイルパス: [Assets/Scripts/Sword/SwordControl.cs](../Assets/Scripts/Sword/SwordControl.cs)

責務:
- ポインター入力の取得・ドラッグ状態管理
- 投擲方向・速度・回転量の計算
- 剣の飛行軌道・寿命管理

主な処理:
- `Initialize`：初期化
- `Update`：毎フレーム更新
- `StartDrag`/`Dragging`/`EndDrag`：ドラッグフロー
- `GetRecentDragVector`：投擲方向推定
- `Movement`：飛行軌道更新
- `Rotate`：軌道曲げ

データ管理:
- 入力Action参照
- 時刻付きドラッグサンプル（保持時間0.3秒）
- 投擲状態（方向、速度、回転量）
- 飛行時間

依存関係:
- SwordAttack
- IStateService
- ISwordDraggingArea
- Main Camera
- SpriteRenderer

実装上の特徴:
- ドラッグの直近0.1秒のサンプルから投擲方向を推定
- 回転量の指数補間により滑らかな軌道変化
- 投擲から5秒後に自動破棄

### EnemySpawnController

ファイルパス: [Assets/Scripts/Enemy/EnemySpawnController.cs](../Assets/Scripts/Enemy/EnemySpawnController.cs)

責務:
- 経過時間と難易度に基づく敵種の解放管理
- 敵の抽選・生成
- 特殊敵（胞子、火の玉）の生成
- 敵撃破時の報酬・統計処理

主な処理:
- `Update`：時間進行と敵管理
- `CheckUnlockEnemyType`：新しい敵種の解放判定
- `ChooseSpawnEnemy`：敵抽選
- `SpawnEnemy`：敵Prefabの生成
- `SpawnEnemyBySpore`/`SpawnEnemyFire`：子敵の生成
- `RemoveEnemy`：敵撃破時の後処理

データ管理:
- EnemySpawnScheduleSO参照
- スポーン領域（開始位置、防衛ライン）
- 敵オブジェクトのList
- 解放済み敵Entry管理

依存関係:
- EnemySpawnScheduleSO
- EnemyController派生クラス
- IResultService
- CurrencyManager
- WorldCanvasManager

実装上の特徴:
- 親敵の設定から子Prefabを選択し、親の敵List に統合
- 死亡時のコールバックで敵削除・報酬計算・結果更新を実行

### EnemyController、EnemyMovement、EnemyHealth

ファイルパス: [Assets/Scripts/Enemy/EnemyController.cs](../Assets/Scripts/Enemy/EnemyController.cs), [Assets/Scripts/Enemy/EnemyMovement.cs](../Assets/Scripts/Enemy/EnemyMovement.cs), [Assets/Scripts/Enemy/EnemyHealth.cs](../Assets/Scripts/Enemy/EnemyHealth.cs)

責務（EnemyController）:
- 敵の共通初期化
- MovementとHealthコンポーネントの統合管理
- Animatorとの連携

責務（EnemyMovement）:
- 敵の移動と防衛ライン到達判定

責務（EnemyHealth）:
- HP管理とダメージ処理
- 敵ごとの攻撃可否判定
- 死亡アニメーション・削除フロー

主な処理（EnemyHealth）:
- `CheckAttackable`：敵が攻撃可能か判定
- `TakeDamage`：ダメージ処理
- `Die`：死亡時処理

データ管理:
- EnemyDataSO参照（型安全なジェネリック）
- 進捗（開始点から目標地点への相対位置）
- 現在HP
- 敵固有の状態フラグ

依存関係:
- EnemyDataSO派生クラス
- Animator（死亡アニメーション）
- AnimationDestroyEvent（削除通知）

実装上の特徴:
- ジェネリック制約 `where TData : EnemyDataSO` により型安全な派生データ取得
- 敵固有処理はフック（Override）で追加
- SO型不一致時のログ警告で検出可能

### SwordAttack

ファイルパス: [Assets/Scripts/Sword/SwordAttack.cs](../Assets/Scripts/Sword/SwordAttack.cs)

責務:
- 回転量に基づく攻撃間隔管理
- 攻撃判定（範囲検索）
- ダメージ計算と適用
- 命中演出（SE、カメラシェイク、エフェクト）

主な処理:
- `Update`：攻撃周期の管理
- `Attack`：攻撃判定と敵検索
- ダメージ計算（基礎値 + 強化ボーナス + クリティカル判定）

データ管理:
- SwordDataByType参照
- 経過時間・ヒット数カウンター
- Enemy LayerMask

依存関係:
- SwordControl
- IStateService
- IResultService
- IAudioService
- ICameraShake
- WorldCanvasManager
- Physics2D.OverlapCircle

実装上の特徴:
- 攻撃間隔：`max(minInterval, baseInterval - abs(rotateAmount) * 0.01)`
- 単一Collider返却のため、複数敵重なり時の対象選択はPhysics結果に依存

## データ・進行・サービスクラス

### BattleSettingConfig / LevelProperty

ファイルパス: [Assets/Scripts/ScriptableObject/BattleSettingConfig.cs](../Assets/Scripts/ScriptableObject/BattleSettingConfig.cs), [Assets/Scripts/ScriptableObject/BattleSettingConfig.Upgrades.cs](../Assets/Scripts/ScriptableObject/BattleSettingConfig.Upgrades.cs), [Assets/Scripts/Systems/LevelProperty.cs](../Assets/Scripts/Systems/LevelProperty.cs)

責務:
- 戦闘パラメータのレベル別計算
- 剣種データ管理
- 強化レベルの解放・現在レベル管理
- セーブ復元時のデータ補正

主な処理:
- `LoadLevelProperties`：セーブ値のロードと範囲補正
- UpgradeType列挙型による強化定義
- 型ごとのロードマージ

データ管理:
- 剣種ごとの攻撃倍率
- 強化のレベル定義
- 各難易度のステータス倍率
- 出現間隔・倍率係数

依存関係:
- StatService
- SaveAndLoadService

### StatService / ServiceLocator

ファイルパス: [Assets/Scripts/Service/Implementations/StatService.cs](../Assets/Scripts/Service/Implementations/StatService.cs), [Assets/Scripts/Service/ServiceLocator.cs](../Assets/Scripts/Service/ServiceLocator.cs)

責務（StatService）:
- 強化レベルから戦闘値を計算して提供

責務（ServiceLocator）:
- サービス実装をインターフェースで登録・取得
- Scene単位での依存管理

実装上の特徴:
- Bootstrap時にサービスを登録
- 依存側はインターフェースのみ参照
- 静的グローバル辞書で管理

### SaveAndLoadService

ファイルパス: [Assets/Scripts/Service/Implementations/SaveAndLoadService.cs](../Assets/Scripts/Service/Implementations/SaveAndLoadService.cs)

責務:
- SaveDataをJSON化してPlayerPrefsに保存
- ロード時に強化レベル・通貨・音量を復元

データ管理:
- LevelProperty配列（UpgradeType別）
- 通貨額
- 音量設定

保存対象は手動セーブのみ。

### ResultService / GameManager

ファイルパス: [Assets/Scripts/Service/Implementations/ResultService.cs](../Assets/Scripts/Service/Implementations/ResultService.cs), [Assets/Scripts/Systems/GameManager.cs](../Assets/Scripts/Systems/GameManager.cs)

責務（ResultService）:
- 防衛時間・敵撃破数・最大ヒット数・通貨・剣生成数の統計

責務（GameManager）:
- ゲーム停止・終了状態の管理
- GameOverイベントの発火

データ管理（ResultService）:
- Entry型で統計を集約
- Unityroomへのスコア送信

## UI補助クラス

### ShowPanelNavigationControllerBase / UIModuleBase

ファイルパス: [Assets/Scripts/UI/ShowPanelNavigationControllerBase.cs](../Assets/Scripts/UI/ShowPanelNavigationControllerBase.cs), [Assets/Scripts/UI/UIModuleBase.cs](../Assets/Scripts/UI/UIModuleBase.cs)

責務:
- UI Toolkitのパネル遷移管理
- UXML要素の初期化
- 画面Stack管理

実装上の特徴:
- UXML名で要素を検索して各Moduleに渡す
- Stackによる画面遷移

### WorldCanvasManager

ファイルパス: [Assets/Scripts/UI/WorldCanvasManager.cs](../Assets/Scripts/UI/WorldCanvasManager.cs)

責務:
- ワールド空間のポップアップ表示管理

実装上の特徴:
- 通常ダメージTextは初期10個の Queue プール
- 枯渇時は追加生成
- Miss・Critical・通貨・攻撃Effectは個別生成

### 敵固有クラス

各敵の特殊挙動実装:

- `ZombieHealth` / `ZombieMovement`：HP閾値で無敵状態化。Coroutineで無敵解除、移動停止
- `FrogHealth`：ジャンプアニメーション中の攻撃不可
- `TrollHealth`：正面からの攻撃防御。判定結果は0.5秒固定
- `KinokoController`：胞子生成状態と一度だけの増殖
- `DevilController` / `FireMovement`：定期火の玉生成とサイン波移動
