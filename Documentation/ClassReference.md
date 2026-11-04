# 主要クラスリファレンス

優先度はゲームループ・他システムへの影響を基準とした。各クラスの実装確認範囲と、今後の課題を併記する。

## 優先度A：ゲームループ中核

### 1. `SwordControl`

- **役割／責務**：InputActionからPoint/Pressを読み、剣のドラッグ、投擲方向・速度・回転量、飛行・寿命を管理。
- **主な処理**：`Initialize`、`Update`、`StartDrag`、`Dragging`、`EndDrag`、`ThrowImpact`、`GetRecentDragVector`、`Movement`、`Rotate`、`CheckDistant`。
- **データ**：入力Action、投擲状態、速度、ドラッグ時間、回転量、時刻付きドラッグサンプル。
- **依存**：SwordAttack、SpriteRenderer、Main Camera、IStateService、ISwordDraggingArea。
- **特徴**：0.3秒のサンプル保持、0.1秒窓の投擲方向推定、時間依存の方向補間。
- **改善余地**：入力Action／Camera参照のライフサイクル検証、操作処理と移動計算の分割、数式のユニットテスト。
- 根拠：[SwordControl.cs](../Assets/Scripts/Sword/SwordControl.cs)

### 2. `EnemySpawnController`

- **役割／責務**：経過時間・難易度に基づく敵種解放、敵抽選・生成、特殊敵の子生成、撃破後処理。
- **主な処理**：`Update`、`CheckUnlockEnemyType`、`ChooseSpawnEnemy`、`SpawnEnemy`、`SpawnEnemyBySpore`、`SpawnEnemyFire`、`RemoveEnemy`。
- **データ**：SpawnSchedule、開始・防衛ライン範囲、敵List、解放済みEntry List。
- **依存**：EnemySpawnScheduleSO、EnemyController派生、IResultService、CurrencyManager、WorldCanvasManager。
- **特徴**：親敵の設定から子Prefabを選び、同じEnemy登録・撃破経路へ加える。
- **改善余地**：スポーン重み抽選式の修正、List管理／解除の堅牢化、生成時のPrefab参照検査。
- 根拠：[EnemySpawnController.cs](../Assets/Scripts/Enemy/EnemySpawnController.cs)

### 3. `EnemyController` と `EnemyMovement` / `EnemyHealth`

- **役割／責務**：共通敵初期化と更新を制御し、移動・HPをコンポーネントに分離。
- **主な処理**：Controller `Initialize`/`Update`、Movement `Initialize`/`Tick`/`UpdateMovement`、Health `Initialize`/`CheckAttackable`/`TakeDamage`/`Die`。
- **データ**：EnemyDataSO、進捗・開始／目標位置、HP、死亡・攻撃可否状態。
- **依存・連携**：Movement、Health、Animator。Healthから死亡アニメーション、AnimationDestroyEventからSpawnerへ削除通知。
- **設計上の特徴**：ジェネリック派生型と共通フック。SO型不一致をログで知らせる。
- **改善余地**：共通基底の責務増加、データ未設定時の継続不能状態の統一、敵型追加時のPrefab構成検証。
- 根拠：[EnemyController.cs](../Assets/Scripts/Enemy/EnemyController.cs)、[EnemyMovement.cs](../Assets/Scripts/Enemy/EnemyMovement.cs)、[EnemyHealth.cs](../Assets/Scripts/Enemy/EnemyHealth.cs)

### 4. `SwordAttack`

- **役割／責務**：回転量ベースの攻撃間隔、単一Collider範囲検索、攻撃可否とダメージ処理、命中演出。
- **データ**：SwordDataByType、経過時間、ヒット数、Enemy LayerMask。
- **依存**：SwordControl、IStateService、IResultService、IAudioService、ICameraShake、WorldCanvasManager、Physics2D。
- **改善余地**：OverlapCircleが返す単一対象の選択規則の明文化、Colliderが複数ある場合の優先順位、Effect pool。
- 根拠：[SwordAttack.cs](../Assets/Scripts/Sword/SwordAttack.cs)

## 優先度B：データ・進行・境界

### 5. `BattleSettingConfig` / `LevelProperty`

- **役割**：戦闘値のレベル別計算、剣種データ、強化値の解放／現在レベル管理とロード時補正。
- **主な連携**：StatService → 剣・攻撃・ストック／Upgrade UI、SaveAndLoadService → `LoadLevelProperties`。
- **特徴**：強化定義整合性チェック、型ごとのロードマージ、範囲クランプ。
- **改善余地**：強化価格・LevelPropertyのコード定義をSOに残る旧価格欄と整理。Snapshotを必要に応じてdeep copyにする。
- 根拠：[BattleSettingConfig.cs](../Assets/Scripts/ScriptableObject/BattleSettingConfig.cs)、[BattleSettingConfig.Upgrades.cs](../Assets/Scripts/ScriptableObject/BattleSettingConfig.Upgrades.cs)、[LevelProperty.cs](../Assets/Scripts/Systems/LevelProperty.cs)

### 6. `StatService` と `ServiceLocator`

- **役割**：強化レベルから戦闘値を算出し、サービスをインターフェースで取得可能にする。
- **特徴**：SwordAttack等はIStateServiceの値を利用。BootstrapがScene参照を登録する。
- **改善余地**：ServiceLocatorの登録解除、重複登録／未登録検出、テスト時の依存差し替え。`Level`の探索は呼び出しごとに配列を走査する。
- 根拠：[StatService.cs](../Assets/Scripts/Service/Implementations/StatService.cs)、[ServiceLocator.cs](../Assets/Scripts/Service/ServiceLocator.cs)、[GameBootstrap.cs](../Assets/Scripts/Systems/Bootstrap/GameBootstrap.cs)、[BattleBootstrap.cs](../Assets/Scripts/Systems/Bootstrap/BattleBootstrap.cs)

### 7. `SaveAndLoadService`

- **役割**：SaveDataをJSON化してPlayerPrefsに保存し、強化・通貨・音量を復元。
- **データ**：`LevelProperty[]`、currencyAmount、AudioVolumeData。
- **改善余地**：JSON不正時、null、欠落項目、互換性を扱うVersion付き移行・エラー表示。
- 根拠：[SaveAndLoadService.cs](../Assets/Scripts/Service/Implementations/SaveAndLoadService.cs)

### 8. `ResultService` / `GameManager`

- **役割**：結果統計、リザルトUI／スコア送信、およびゲーム停止／終了状態の管理。
- **データ**：防衛時間、敵撃破数、最大ヒット数、通貨、剣生成数。
- **連携**：敵Spawner／SwordAttackから統計更新、GameOverイベントから結果表示、Unityroomクライアントへスコア送信。
- **改善余地**：staticイベント購読解除、Sceneロード後の二重呼び出し防止、Scene番号の名前定数化。
- 根拠：[ResultService.cs](../Assets/Scripts/Service/Implementations/ResultService.cs)、[GameManager.cs](../Assets/Scripts/Systems/GameManager.cs)

## 優先度C：UI・補助

### `ShowPanelNavigationControllerBase` / `UIModuleBase`

UI Toolkitの名前付き要素を各Moduleに渡し、画面遷移をStackで管理する。UXML名の文字列一致に依存する。根拠：[ShowPanelNavigationControllerBase.cs](../Assets/Scripts/UI/ShowPanelNavigationControllerBase.cs)、[UIModuleBase.cs](../Assets/Scripts/UI/UIModuleBase.cs)。

### `WorldCanvasManager`

通常ダメージTextのみ初期Queueプール化し、空なら追加生成。他のポップアップはInstantiate。RootのSingleton参照。根拠：[WorldCanvasManager.cs](../Assets/Scripts/UI/WorldCanvasManager.cs)。

### 敵固有クラス

- `ZombieHealth` / `ZombieMovement`：HP閾値を契機に無敵、Coroutine解除と移動停止。`ZombieController`はイベントをAnimatorへ反映しOnDestroyで解除。
- `FrogHealth`：ジャンプ中の攻撃可否を制御。
- `TrollHealth`：正面角度と短時間の判定固定。
- `KinokoController`：胞子生成状態と一度だけの繁殖。
- `DevilController` / `FireMovement`：定期火の玉生成とサイン波移動。

これらは特殊挙動の例であり、全敵に複雑なAI状態機械が実装されているわけではない。
