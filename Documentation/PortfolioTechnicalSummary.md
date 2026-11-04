# ポートフォリオ・採用面接向け技術説明

## 1. 作品紹介（実装に基づく）

SwordSlicerは、Unity 6／C#で制作された2D防衛サバイバルゲーム。プレイヤーが剣をドラッグして投擲し、直近のドラッグ方向、回転量、強化値を攻撃軌道・攻撃間隔に反映する。敵は型別SOと挙動スクリプトを持ち、ホーム画面の強化・保存・難易度選択と、戦闘Sceneの敵／剣／結果処理で構成される。

チーム人数・制作期間・担当範囲は資料から確認できないため、自己紹介では実際の担当範囲を別途正確に補足する。

## 2. 技術アピールTOP5

| 順位 | 技術的アピール | 根拠となる実装 | 技術的メリット | 面接で説明する価値 |
|---:|---|---|---|---|
| 1 | 敵共通フローと型付きデータ | EnemyController/Movement/Healthのジェネリック基底、EnemyMovementのフック | 共通処理と敵固有差分を分け、データ型誤りを早期にログ化 | 継承・フックを選んだ理由と、能力コンポジションへ移行する境界 |
| 2 | 入力サンプルに基づく剣投擲とカーブ | 0.3秒バッファ、0.1秒方向窓、指数補間 | ドラッグ入力を方向・速度・曲線移動に展開 | 入力時間窓、Frame-rate依存性、反応時間の式と実機調整方法 |
| 3 | ScriptableObjectを使った敵・出現データ | EnemyDataSO派生、EnemySpawnScheduleSO asset、Prefab参照 | 基本数値調整・Prefabの追加をコードから分離 | データ追加で済む変更と、挙動コードが必要な変更の線引き |
| 4 | 強化セーブ復元の型別マージとClamp | enumキーによるロード、LevelProperty.LoadLevels、UpgradeType定義検証 | 配列順序への依存を抑え、旧／範囲外レベルを補正 | セーブ互換の現状、Version管理・異常系への次の改善 |
| 5 | UI Toolkit構成と局所的オブジェクト再利用 | UXML/USSテンプレート、Panel Stack、ダメージ表示Queue | UI構造と表示制御を分け、一部の頻出生成物を再利用 | UI ToolkitとuGUIの使い分け、プールの適用範囲と計測計画 |

**誠実に添える制約**：敵抽選式は現在の重みデータに対して不整合があり、TOP5の3番目の仕組みには修正余地がある。性能最適化・高負荷耐性はProfilerの測定値がないため主張しない。

## 3. 主要システムの説明

### 剣操作・戦闘

`SwordControl`は入力・ドラッグ履歴・投擲・移動を扱い、`SwordAttack`は周期攻撃・OverlapCircle・ダメージ・演出を担当する。攻撃値と強化はIStateService経由で取得する。説明時は「回転操作をゲーム内の攻撃周期と軌道に結びつけた」点を中心にし、スワイプの気持ちよさを実機計測なしに断定しない。

### 敵

`EnemySpawnController`が時間／難易度を見て敵Prefabを作り、EnemyControllerがSOデータ・Movement・Health・Animatorを連携する。スライムの基底移動、ゴブリンのステップ移動、ゾンビの無敵、トロールの角度判定など、異なる小さな例を説明できる。

### データと成長

EnemyDataSO／EnemySpawnScheduleSO／SwordDataSOはAssetで調整できる。強化はレベル解放／現在レベルを分け、ロード時にenum別で復元する。一方、価格とLevelProperty定義はコード固定で、すべてのバランス値をInspectorで変えられるわけではない。

## 4. 設計判断と改善意識

- **判断**：敵の共通要素は継承基底に集約し、種別SOで能力パラメータを渡す。
  **説明**：敵追加時の共通初期化複製を減らせるが、複合能力が増えた場合は継承よりコンポジションを再検討する。
- **判断**：UI Toolkitとワールド上TextMesh Proを併用。
  **説明**：現在の実装で確認できる用途の分担である。採用時の制作判断は明文化されていないため「こういう理由で選んだ」と断定しない。
- **判断**：static ServiceLocatorでサービスを参照。
  **説明**：具象型への直接依存を抑える一方、テストとScene寿命をグローバル状態に結び付ける課題がある。
- **次に改善**：スポーン重み正規化、staticイベント解除、SaveData Version化、Scene再ロード／抽選／敵到達テスト、Profiler計測。

## 5. 想定される技術質問

1. `GetRecentDragVector`の0.1秒窓はなぜ必要か。サンプルが途切れた場合はどうするか。
2. 指数補間 `1 - exp(-dt / reactTime)` は一定Lerp係数とどう違うか。
3. `EnemyMovement<TData>`の共通フローと各派生の責務は何か。新しい行動をどこへ追加するか。
4. ScriptableObjectに置いた値と、コード定義に残した強化価格の違いは何か。
5. 強化レベルのCurrentLevelとReleaseLevelをどう復元し、旧セーブをどう扱うか。
6. スポーン重みの現在の不整合をどう再現し、修正後の分布をどうテストするか。
7. 通常DamageText以外をプール化する前に、Profilerで何を測るか。
8. ServiceLocatorが未登録、またはScene再ロードされたときに何が起きるか。

## 6. 面接用の説明文例

> 敵は共通の初期化・移動・体力処理を基底クラスにまとめ、種別ごとのデータはScriptableObject、挙動差分は型付き派生クラスに分けました。これにより基本パラメータ違いはアセットで追加しやすく、型の取り違えもログで見つけられます。一方、行動の種類が増えると継承が深くなるので、能力単位の合成へ移す境界を今後の課題にしています。

## 7. 成果物・参照先

技術の詳細：[TechnicalHighlights.md](TechnicalHighlights.md)、[ClassReference.md](ClassReference.md)、[ImprovementPlan.md](ImprovementPlan.md)。企画・仕様：[GameOverview.md](GameOverview.md)、[GameSpecification.md](GameSpecification.md)。
