# 実装品質・コードレビュー（リポジトリ確認ベース）

## 1. 総評

2Sceneの2Dサバイバルゲームとして、剣操作・敵基底処理・SOデータ・UI・セーブ／結果の実装が一通り揃う。敵の型付きデータ取得や強化ロード補正は具体的な保守上の工夫である。一方で、スポーン抽選の実動作不整合、staticイベント／サービスの寿命、設定値の二重化、テスト不在が目立つ。ビルド・実機・Profiler結果がないため、実行時品質を断定しない。

## 2. 評価一覧

| 観点 | 良い点 | 問題点・評価 |
|---|---|---|
| 責務分離 | SwordControl/SwordAttack、EnemyController/Movement/Healthなどが分かれる。 | EnemySpawnControllerは選択・解放・生成・報酬・結果まで担当。StatServiceも強化名・説明・値・価格UIモデルを集約する。 |
| クラス設計 | Enemyジェネリック基底で型安全な派生データと共通フローを提供。主要Service APIにInterfaceがある。 | ServiceLocatorは静的グローバル状態。Scene登録と解除が明示されず、依存順序・テスト隔離が難しい。 |
| 命名 | 機能単位のフォルダと大半の型名で役割を追える。 | `SpawnSorwd`、`defanse`、`Intaractable` 等のタイポ、表記揺れ、一部古いコメントが保守性を下げる。 |
| 可読性 | 日本語コメント・XMLコメントが処理目的を説明する箇所がある。 | コメントと実装がズレた箇所があり、長いメソッドや役割複合クラスもある。英日混在・誤字を整理したい。 |
| 再利用性 | Enemy基底とSO派生、UXMLテンプレート、UI Module基底。 | 敵能力の構成は継承に寄り、Spawner／Result／Currency連携が直接。再利用可能な汎用システムとまではいえない。 |
| 拡張性 | 敵の基本数値・出現Prefab・剣データをAssetで追加できる。UpgradeType定義チェックがある。 | 強化価格・LevelPropertyはコード固定。新しい敵行動にはスクリプト実装とPrefab設定が要る。スポーン抽選式は現行データを正しく処理しない。 |
| 依存関係 | 一部の境界はInterface経由で参照できる。 | ServiceLocatorとSingleton (`WorldCanvasManager.I`) によるグローバルアクセスが混在。Unity SceneのInspector参照にも依存。 |
| エラー処理 | データ型誤り・不足アップグレード・未定義価格等でログを出す。ServiceLocator未登録は例外。 | SO/Prefab/Animator/UIDocument、Save JSON等のnull／不正状態が全箇所で扱われない。フォールバック値0や継続が適切かはケースごとの通知が必要。 |
| メモリ管理 | 通常DamageTextはQueue再利用。CurrencyUIと一部イベントは解除する。 | Effect、敵、Sword、各種PopupはInstantiate/Destroy中心。staticイベント購読解除漏れがある。 |
| パフォーマンス | 2D Physicsの円形範囲検索、限定された入力サンプル。 | `Update`利用箇所が多い。対象敵がいない場合のSwordAttackは攻撃状態のまま毎フレーム検索する。敵数・GC・描画・CPU計測なし。実測なしに最適化済みとはいえない。 |
| テスト容易性 | `LevelProperty`や一部計算は切り出され、Interfaceも定義される。 | NUnit／Unity Testコード、asmdef、CI workflowを確認できない。ランダム抽選やScene統合動作の自動回帰がない。 |
| Unity固有設計 | UXML/USS、ScriptableObject、Animator Event、Gizmos、Editor限定PropertyDrawerを利用。 | Animator Event設定やScene/Prefab参照が実行動作の一部。Asset検証・Editor toolingが限定的。 |
| ドキュメント整合 | READMEは主要体験・操作・技術構成を現行コードに沿って記載。 | READMEで説明される敵出現の多様性は、現行重み抽選式だとアセット上の全敵を抽選できない。実装修正・README補足が必要。 |

## 3. 具体的な指摘

### 高優先度：スポーン確率計算

`ChooseSpawnEnemy` は `Random.Range(0, totalWeight)` ではなく `Random.value` を使い、合計重みで正規化しない。現行Entryでは先頭2重みの合計が1を超えるため、3番目以降の累積範囲に乱数が到達しない。データを調整して全敵をアンロックしても、一部の敵は抽選されない。修正後は合計重みを検査するテストが必要。

### 中優先度：攻撃対象がいない間の毎フレーム判定

`SwordAttack.Attack` は範囲内の `EnemyHealth` がnullの場合にreturnするが、攻撃状態を解除しない。敵が入るまで `OverlapCircle` が毎フレーム呼ばれるため、意図した待ち受け仕様か、攻撃間隔ごとに再判定する仕様かを明確にし、頻度を検証する。

### 高優先度：Scene再ロードとイベント

`GameManager.Awake` と `ResultService.Awake` はstatic `GameManager.OnGameOver` に匿名delegateを追加し、解除しない。Sceneをリトライ／ホーム遷移すると登録が残り、古いResultServiceを呼び出す可能性がある。明示的な名前付きハンドラを用いてOnDestroyで解除するか、staticイベントではない所有者付き通知へ変更する。

### 中優先度：保存データの防御性

`Load`はPlayerPrefs文字列を直ちに `JsonUtility.FromJson` し、nullや不正JSONへの処理がない。LevelPropertyにClamp・enum別統合はあるが、SaveData全体にVersion・移行・エラー通知はない。

### 中優先度：重み・価格データの整合

BattleSettingConfig assetには旧形式の価格欄が残るが、現行価格はUpgrade partialのstatic配列。Inspector編集が実際の価格へ反映されるように見えて誤操作につながる。単一の正規データ源を選ぶ必要がある。

## 4. 根拠ファイル

主要箇所：[EnemySpawnController.cs](../Assets/Scripts/Enemy/EnemySpawnController.cs)、[GameManager.cs](../Assets/Scripts/Systems/GameManager.cs)、[ResultService.cs](../Assets/Scripts/Service/Implementations/ResultService.cs)、[SaveAndLoadService.cs](../Assets/Scripts/Service/Implementations/SaveAndLoadService.cs)、[BattleSettingConfig.Upgrades.cs](../Assets/Scripts/ScriptableObject/BattleSettingConfig.Upgrades.cs)、[WorldCanvasManager.cs](../Assets/Scripts/UI/WorldCanvasManager.cs)。

## 5. 非確認事項

- Unity EditorでのPlay確認、Android/WebGLビルド成功、端末操作、Profiler計測は行っていない。
- CI設定、テストコード、asmdefはリポジトリの対象範囲で確認できない。
- アセットのライセンス条件や外部素材と実際のファイルとの対応は未確認。
