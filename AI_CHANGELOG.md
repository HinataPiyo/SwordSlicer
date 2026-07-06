# AI変更記録

## 記録目的
このファイルは、AI支援で実施したコード改善の範囲と意図を明確化するための記録です。

## 記録日
- 2026-07-06

## 今回のAI実装サマリー
- Enemy周りを型安全化（ジェネリクス導入）
- Movementをフック方式（Template Method）へ再設計
- BattleSettingConfigの責務分離とアップグレード定義の整合性検証を追加

## AIで変更した主な設計ポイント

### 1. Controllerのジェネリクス化
- 追加: [Assets/Scripts/Enemy/EnemyController.cs](Assets/Scripts/Enemy/EnemyController.cs)
  - EnemyController<TData> を追加
  - TryGetData / TryGetEnemyData を実装
  - OnValidateでData型不一致を警告
- 適用:
  - [Assets/Scripts/Enemy/Devil/DevilController.cs](Assets/Scripts/Enemy/Devil/DevilController.cs)
  - [Assets/Scripts/Enemy/Fire/FireController.cs](Assets/Scripts/Enemy/Fire/FireController.cs)
  - [Assets/Scripts/Enemy/Kinoko/KinokoController.cs](Assets/Scripts/Enemy/Kinoko/KinokoController.cs)
  - [Assets/Scripts/Enemy/Zombie/ZombieController.cs](Assets/Scripts/Enemy/Zombie/ZombieController.cs)

### 2. Movementの共通フロー化 + ジェネリクス化
- 変更: [Assets/Scripts/Enemy/EnemyMovement.cs](Assets/Scripts/Enemy/EnemyMovement.cs)
  - フック追加: ShouldPauseMovement / AdvanceProgress / EvaluatePosition / EvaluateScaleProgress / OnMoved
  - 敵データ解決フック名を ResolveTypedData に統一
  - EnemyMovement<TData> を追加し TryGetTypedData を共通化
- 適用:
  - [Assets/Scripts/Enemy/Fire/FireMovement.cs](Assets/Scripts/Enemy/Fire/FireMovement.cs)
  - [Assets/Scripts/Enemy/Kinoko/KinokoMovement.cs](Assets/Scripts/Enemy/Kinoko/KinokoMovement.cs)
  - [Assets/Scripts/Enemy/Zombie/ZombieMovement.cs](Assets/Scripts/Enemy/Zombie/ZombieMovement.cs)
  - [Assets/Scripts/Enemy/Goblin/GoblinMovement.cs](Assets/Scripts/Enemy/Goblin/GoblinMovement.cs)
  - [Assets/Scripts/Enemy/Minotaur/MinotaurMovement.cs](Assets/Scripts/Enemy/Minotaur/MinotaurMovement.cs)
  - [Assets/Scripts/Enemy/Frog/FrogMovement.cs](Assets/Scripts/Enemy/Frog/FrogMovement.cs)
  - [Assets/Scripts/Enemy/Devil/DevilMovement.cs](Assets/Scripts/Enemy/Devil/DevilMovement.cs)
  - [Assets/Scripts/Enemy/Troll/TrollMovement.cs](Assets/Scripts/Enemy/Troll/TrollMovement.cs)
  - [Assets/Scripts/Enemy/Slime/SlimeMovement.cs](Assets/Scripts/Enemy/Slime/SlimeMovement.cs)

### 3. Healthのジェネリクス化
- 変更: [Assets/Scripts/Enemy/EnemyHealth.cs](Assets/Scripts/Enemy/EnemyHealth.cs)
  - EnemyHealth<TData> を追加
  - ResolveTypedData / TryGetTypedData を共通化
  - dieAnimationのnull安全化
- 適用:
  - [Assets/Scripts/Enemy/Devil/DevilHealth.cs](Assets/Scripts/Enemy/Devil/DevilHealth.cs)
  - [Assets/Scripts/Enemy/Fire/FireHealth.cs](Assets/Scripts/Enemy/Fire/FireHealth.cs)
  - [Assets/Scripts/Enemy/Frog/FrogHealth.cs](Assets/Scripts/Enemy/Frog/FrogHealth.cs)
  - [Assets/Scripts/Enemy/Goblin/GoblinHealth.cs](Assets/Scripts/Enemy/Goblin/GoblinHealth.cs)
  - [Assets/Scripts/Enemy/Kinoko/KinokoHealth.cs](Assets/Scripts/Enemy/Kinoko/KinokoHealth.cs)
  - [Assets/Scripts/Enemy/Minotaur/MinotaurHealth.cs](Assets/Scripts/Enemy/Minotaur/MinotaurHealth.cs)
  - [Assets/Scripts/Enemy/Slime/SlimeHealth.cs](Assets/Scripts/Enemy/Slime/SlimeHealth.cs)
  - [Assets/Scripts/Enemy/Troll/TrollHealth.cs](Assets/Scripts/Enemy/Troll/TrollHealth.cs)
  - [Assets/Scripts/Enemy/Zombie/ZombieHealth.cs](Assets/Scripts/Enemy/Zombie/ZombieHealth.cs)

### 4. BattleSettingConfigの分割 + 定義検証強化
- 変更: [Assets/Scripts/ScriptableObject/BattleSettingConfig.cs](Assets/Scripts/ScriptableObject/BattleSettingConfig.cs)
  - `partial` 化し、アップグレード定義責務を別ファイルへ分離
  - レベル依存配列アクセスを `GetLevelArrayValue` 経由に統一し、未設定時のフォールバックとログを追加
  - クリティカル率を `Mathf.Clamp01` で上限保護
- 追加: [Assets/Scripts/ScriptableObject/BattleSettingConfig.Upgrades.cs](Assets/Scripts/ScriptableObject/BattleSettingConfig.Upgrades.cs)
  - PriceEntry/LevelPropertyの定義を集約
  - UpgradeType列挙体との整合性検証（欠落・重複・未定義）を追加
  - `TryGetPriceEntry` と `GetLevelPropertiesSnapshot` を追加
  - ロード時にUpgradeType単位で安全にマージし、不足項目は既存値維持
- 追加: [Assets/Scripts/ScriptableObject/PriceEntry.cs](Assets/Scripts/ScriptableObject/PriceEntry.cs)
  - PriceEntryを独立クラス化し、価格計算責務を分離
- 適用:
  - [Assets/Scripts/Service/Implementations/StatService.cs](Assets/Scripts/Service/Implementations/StatService.cs)
    - 価格参照を `TryGetPriceEntry` ベースに変更し、未定義時は0返却 + エラーログ
  - [Assets/Scripts/Service/Implementations/SaveAndLoadService.cs](Assets/Scripts/Service/Implementations/SaveAndLoadService.cs)
    - セーブ時に `GetLevelPropertiesSnapshot` を使用して配列参照の直接共有を回避


## 品質観点での効果
- 型キャストの分散を削減し、DataSO型不一致の検出を早期化
- 敵ごとの移動差分をフック化し、重複実装を削減
- null参照リスクとイベント解除漏れリスクを軽減
- 今後の敵追加時に、Controller/Movement/Healthそれぞれで型を指定するだけで拡張しやすい構造へ移行
- Upgrade定義の欠落や重複を起動時に検出でき、運用時の設定ミスを早期発見
- ロード/セーブ処理の防御性を上げ、壊れたセーブデータ入力時の破綻リスクを軽減

## 注意事項
- この記録は、本セッションでAIが提案・実装したコード変更を中心に記載
