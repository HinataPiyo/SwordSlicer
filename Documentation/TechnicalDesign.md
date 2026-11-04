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

## 2. アーキテクチャ・依存管理

プロジェクトは `Systems`、`Service`、`Sword`、`Enemy`、`UI`、`ScriptableObject` に分かれたMonoBehaviour中心構成。明確なレイヤードアーキテクチャやDIコンテナはなく、Sceneに配置したBootstrapがインターフェース型を `ServiceLocator` に登録し、機能側が静的取得する。

**メリット**：SwordやEnemy側からサービス具象クラスへの直接参照を抑えられ、サービス差し替えの入口をインターフェースにできる。
**制約**：登録順序・Scene寿命が暗黙依存で、登録漏れは `Get<T>()` の例外となる。静的辞書の解除・テスト分離も別途必要。

敵は基底クラスで共通初期化・移動／HPフローを持ち、敵固有差分を派生クラスとフックで追加する。ジェネリック制約 `where TData : EnemyDataSO` と実行時検証によりデータ型不一致を見つけやすい。

## 3. データ設計

- EnemyDataSOは名前・HP・防衛ライン到達時間・報酬を共通化し、種別SOでジャンプ、移動周期、無敵、攻撃方向、繁殖などのパラメータを追加。
- EnemySpawnScheduleSOは難易度、出現間隔、時間経過倍率、アンロック間隔、Prefabごとの重みを保持する。
- SwordDataSOは攻撃倍率とアイコンを保持。BattleSettingConfigは剣種別、戦闘パラメータ、レベル別倍率配列を提供。
- SOを使うことでPrefab／パラメータをInspectorで参照し再利用できる。ただし、強化価格とLevelProperty配列はstaticコード定義のため、全てをInspectorで編集できるデータ駆動設計ではない。
- 現行BattleSettingConfig assetにはソースの現行型に存在しない旧 `priceEntries` serialized dataが見える。価格の実際の参照元は `BattleSettingConfig.Upgrades.cs` の静的配列であり、アセット上の値との二重管理／混乱を解消すべき。

## 4. ゲームロジック設計

### 剣

入力を位置サンプルに変換して投擲方向を算出し、ドラッグ時間を速度に変換。飛行中は反応時間を使った指数補間で回転入力に追従し、速度に応じた角度変化を積分して移動する。攻撃は独立コンポーネントの周期処理とOverlapCircleで判定する。物理エンジンのRigidbody移動ではなくTransform更新中心。

### 敵

SpawnControllerがスケジュールと経過時間からPrefab・倍率を決定し、EnemyControllerへ初期化値を渡す。基底Movementは開始位置とターゲット間の進捗を計算、Healthは倍率適用HPを管理。死亡時はAnimationDestroyEventコールバックでSpawnControllerが削除・報酬・結果加算を行う。

### ゲーム状態・UI

GameOver／GameStopはstatic boolで複数Update処理を停止。UI ToolkitはUXML上のテンプレートをC#で初期化し、ホームの画面遷移をStackで管理。結果画面の値はResultService.Entryに集約される。

## 5. 保存・シーン

`SaveAndLoadService` は強化スナップショット、Currency、AudioVolumeDataをSaveDataに詰め、JsonUtilityで文字列化し、PlayerPrefsへ保存する。セーブはホームUIから手動実行される。Scene遷移は主に`SceneManager.LoadScene`で、結果画面はBuild Settingsのindex 0/1を使用。

**未確認／非実装として扱う事項**：自動セーブ、スキーマVersion、バックアップ、暗号化、クラウド／複数スロット。PlayerPrefsはローカル設定・進行データ向けの簡易保存であり、不正改変耐性を保証しない。

## 6. Unity固有要素の使用状況

| 技術 | 用途・評価 |
|---|---|
| MonoBehaviour | ゲーム動作・UI・サービスの主要構成。 |
| ScriptableObject | 敵／剣／出現／音声／チュートリアルデータ。 |
| C# event / Action | 通貨、体力、無敵、ゲーム終了、ロード通知。UnityEvent使用は確認できない。 |
| Coroutine | 剣ストックUI、カメラ揺れ、無敵時間、トロール判定解除、通貨アニメーション。 |
| Input System | Pointer/Mouse操作。 |
| UI Toolkit | ホーム・ゲーム中UI・結果・強化。 |
| TextMesh Pro | ワールド空間ポップアップ。 |
| Animator / Animation Event | 敵挙動・死亡アニメーションと削除コールバック。 |
| Physics2D | OverlapCircleによる攻撃対象検索。 |
| Object Pooling | WorldCanvasManagerの通常ダメージテキストだけ。初期10個、枯渇時は追加生成。 |
| Gizmos / Editor | 範囲表示とReadOnly用PropertyDrawer。 |
| Cinemachine | Perlinノイズ振幅をCoroutineで一時変更するカメラシェイク。 |

## 7. パフォーマンスと品質上の考慮

- `Update`は剣、全敵、スポーン、UI等で利用。敵数や端末別のProfiler計測値はないため負荷や最適化状態を断定しない。
- SwordAttackは対象Colliderがないと攻撃状態を維持して毎フレームOverlapCircleを実行する。意図した「待ち受け」動作か決めたうえで、一定間隔に戻すかどうかとCPU負荷を測定する。
- 敵／剣／エフェクト／Miss・Critical・通貨ポップアップはInstantiate/Destroy主体。通常ダメージ表示だけプール。
- `EnemySpawnController` は敵をList管理し、毎回のアンロック確認でList.Containsを行う。規模を抑えた実装だが大量の敵／項目での測定はない。
- `AdjustAspectRatio.Update` は画面寸法が変わらなくても毎フレームViewportを再計算する。頻度削減の余地がある。
- 入力サンプルListは古い要素を先頭からRemoveAtするが、保持時間が0.3秒の短い入力バッファであり、現状問題の実測はない。
- GizmosがあるためScene配置の範囲確認を補助する。自動検証やデータ生成用Editorツールは限定的。

## 8. 主要技術課題

1. スポーン重みと0～1乱数のスケール不一致。
2. static ServiceLocator／GameOverイベントのScene再ロード寿命管理。
3. SaveDataの不正JSON・欠落値・未知Versionへの耐性不足。
4. 敵挙動／難易度／シーン進行の自動テストが見当たらない。
5. 価格の実行時コード定義と古いアセット値の不一致。
6. 生成物の部分プール化に留まり、負荷を測るProfiler記録が未確認。

具体的な改善案・優先度は [ImprovementPlan.md](ImprovementPlan.md) を参照。
