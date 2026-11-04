# 実装品質レビュー

## 1. 実装の構成

- 剣操作：SwordControl、SwordAttack で入力・移動・攻撃を分離
- 敵管理：EnemySpawnController で生成・撃破・統計処理。EnemyController、EnemyMovement、EnemyHealth で共通化と派生差を実装
- サービス：ServiceLocator でインターフェース型を登録。StatService、SaveAndLoadService、ResultService でデータ・状態を管理
- UI：UI Toolkit 中心。WorldCanvasManager でワールド空間のポップアップを管理

## 2. 既知の問題

### 敵スポーン確率の不整合

`EnemySpawnController.ChooseSpawnEnemy` は `Random.value`（0～1）と各敵の累積重みを比較する。現行の EnemySpawnScheduleSO では重みの合計が1を超えるため、先頭の敵だけが選ばれやすく、後ろの敵は抽選されないことがある。

**詳細**：[EnemySpawnController.cs](../Assets/Scripts/Enemy/EnemySpawnController.cs) の ChooseSpawnEnemy メソッドを参照。

### Scene再ロード時のイベント重複

`GameManager.OnGameOver` は static Action。GameManager と ResultService が匿名 delegate で購読し、OnDestroy で解除しない。Scene をリトライ・ホーム遷移時に購読が蓄積し、古い Scene の処理が再実行される可能性がある。

**詳細**：[GameManager.cs](../Assets/Scripts/Systems/GameManager.cs)、[ResultService.cs](../Assets/Scripts/Service/Implementations/ResultService.cs)を参照。

### 攻撃判定の連続実行

`SwordAttack.Attack` は範囲内に敵がない場合、攻撃状態を維持したまま return する。敵が範囲に入るまで毎フレーム `Physics2D.OverlapCircle` を実行するため、意図した待ち受け仕様なのか、攻撃間隔で再判定するのかを明確にする必要がある。

**詳細**：[SwordAttack.cs](../Assets/Scripts/Sword/SwordAttack.cs) の Attack メソッドを参照。

### 保存データの検証不足

`SaveAndLoadService.Load` は PlayerPrefs から JSON 文字列を取得して直接 `JsonUtility.FromJson` で復元する。不正な JSON やフィールド欠落への処理がない。LevelProperty には範囲補正があるが、SaveData 全体の Version 管理や エラー通知はない。

**詳細**：[SaveAndLoadService.cs](../Assets/Scripts/Service/Implementations/SaveAndLoadService.cs) を参照。

### 強化価格データの二重管理

BattleSettingConfig.asset には旧形式の `priceEntries` フィールドが残るが、実際の価格は [BattleSettingConfig.Upgrades.cs](../Assets/Scripts/ScriptableObject/BattleSettingConfig.Upgrades.cs) の static 配列で定義されている。Inspector 編集時に見た目から実際の価格データがわかりにくい。

## 3. 参照資料

詳細な改善提案は [ImprovementPlan.md](ImprovementPlan.md) を参照。
