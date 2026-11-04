# 技術的アピールポイント

評価はリポジトリ上の実装範囲に限る。プロファイラ・端末試験・自動テストの結果がないため、性能・操作品質・不具合不存在を保証しない。

## 1. 敵共通処理と型付きデータ・拡張フック

1. **名称**：敵の共通初期化と型付き派生データ。
2. **実装概要**：`EnemyController<TData>`、`EnemyMovement<TData>`、`EnemyHealth<TData>` がEnemyDataSOの具体型を制約付きで扱う。Movement基底は進捗、位置、スケール、到達判定を共通化し、敵は `ShouldPauseMovement`、`AdvanceProgress`、`EvaluatePosition`、`OnMoved` 等を必要に応じて上書きする。
3. **解決する課題**：敵種ごとに初期化や基本HP／移動処理を複製すること、種別データの誤参照。
4. **実装前に想定される問題**：挙動差分の重複、キャスト箇所の分散、共通修正の反映漏れ。
5. **技術・設計**：継承＋フック、ジェネリック制約、型不一致時のエラーログとInspector警告。
6. **処理の流れ**：SpawnerがPrefabを生成し `Initialize` → Controllerが型別SOをMovement/Healthへ渡す → 各派生が共通Tickの差分を上書き。
7. **該当ファイル**：[EnemyController.cs](../Assets/Scripts/Enemy/EnemyController.cs)、[EnemyMovement.cs](../Assets/Scripts/Enemy/EnemyMovement.cs)、[EnemyHealth.cs](../Assets/Scripts/Enemy/EnemyHealth.cs)、[ZombieController.cs](../Assets/Scripts/Enemy/Zombie/ZombieController.cs)、[GoblinMovement.cs](../Assets/Scripts/Enemy/Goblin/GoblinMovement.cs)。
8. **コード根拠**：基底型に共通処理が存在し、特殊挙動は派生で型付きデータを受け取る。AI_CHANGELOGもフック方式・型安全化を変更記録として記載（開発者の企画意図そのものではなく変更履歴）。
9. **メリット**：共通移動やデータ型検証を集約し、新しい敵の基本パラメータ追加はSOで行える。
10. **拡張・保守**：新しい移動差分をフックで追加できる。ただし新しい状態機械や複雑な能力まで継承に積み重ねると基底／派生の結合が強まる。
11. **面接説明**：共通フローと敵固有の差をどう切り分けるか、型制約とUnity Inspector上の参照検証の両立を説明できる。
12. **客観評価**：本作の規模に対し有用な共通化。高度なAIフレームワークやECSではなく、MonoBehaviour継承の整理である。
13. **弱点・改善**：EnemyControllerがMovement/Health/AnimatorをGetComponentし直接調停する。将来は能力単位のコンポジションや共通契約テストを検討できる。

## 2. ドラッグ履歴を使う投擲方向とカーブ制御

1. **名称**：時間窓を使った投擲方向推定と滑らかな旋回。
2. **実装概要**：ドラッグ座標と時刻を保持し、リリース直前約0.1秒の変位から方向を求める。ドラッグ時間で速度を計算し、回転量を指数補間で追従させて軌道へ適用する。
3. **解決する課題**：瞬間のPointer座標だけでは、離した方向や操作強度を安定して評価しづらい。
4. **実装前に想定される問題**：細かいフレーム差・ドラッグ停止時の方向揺れ、回転入力を投擲後の動きへ連続させられない。
5. **技術・設計**：Input System、短時間の時刻付きサンプル、Clamp、Time.deltaTimeと指数補間。
6. **処理の流れ**：サンプル記録 → 古いサンプル削除 → release時に直近変位計算 → throwDir/Speed設定 → 飛行中の旋回・Transform更新。
7. **該当ファイル**：[SwordControl.cs](../Assets/Scripts/Sword/SwordControl.cs)。
8. **コード根拠**：`ThrowDirectionSampleWindow` 0.1秒、`DragSampleKeepSeconds` 0.3秒、`CalculateThrowSpeed`、`GetRecentDragVector`、指数補間のMovement処理。
9. **メリット**：入力の「離す方向」と蓄積回転をそれぞれ投擲方向・カーブに活用し、1本の操作を複数の戦闘値へ結び付ける。
10. **拡張・保守**：軌道式・バッファ窓・速度曲線を変更しやすいが、現状は定数がコードにあり、Inspector調整・操作再生テストはない。
11. **面接説明**：なぜ直近差分を用いるか、フレーム時間に依存しない補間をどう実現しているか説明できる。
12. **客観評価**：入力履歴を使うゲーム固有ロジックとして説明価値がある。実端末で安定・快適とするには検証が必要。
13. **弱点・改善**：カメラ参照のnull前提、Pointerバインディングの実機確認、ドラッグが止まったときの意図的方向選択、操作再現テストを追加したい。

## 3. ScriptableObjectで敵・出現データを分離

1. **名称**：敵種別・出現スケジュールのアセット化。
2. **実装概要**：EnemyDataSO派生がHP・到達時間・報酬と個別パラメータを持ち、EnemySpawnScheduleSOが難易度・出現／解放間隔・Prefabエントリーを保持する。
3. **解決する課題**：敵数値やPrefabの参照を挙動コードへ直接埋め込むこと。
4. **実装前に想定される問題**：敵の数値変更にスクリプト編集・再コンパイルが必要になる。
5. **技術・設計**：ScriptableObject、シリアライズ可能なEntry、敵データ型の継承。
6. **処理の流れ**：SceneからSchedule参照 → 時間・難易度で倍率計算 → EntryからPrefabを選ぶ → EnemyControllerが型別SOを参照。
7. **該当ファイル**：[EnemySpawnScheduleSO.cs](../Assets/Scripts/ScriptableObject/EnemySpawnScheduleSO.cs)、[EnemyDataSO.cs](../Assets/Scripts/ScriptableObject/Enemy/EnemyDataSO.cs)、[EnemySpawnScheduleSO.asset](../Assets/Datas/EnemySpawnScheduleSO.asset)、[EnemyDatas/](../Assets/Datas/EnemyDatas)。
8. **コード根拠**：Scheduleに10エントリー、敵の個別アセットとPrefab参照が存在する。
9. **メリット**：数値調整・Prefabの差し替えをInspectorで行いやすい。データアセットの再利用も可能。
10. **拡張・保守**：基本ステータス違いの敵追加には適する。新しい行動を追加する場合は型別コード／Prefab配線が別途必要。
11. **面接説明**：設定データと実行ロジックをどの範囲で分けたか、SOだけでは挙動を追加できない境界を説明できる。
12. **客観評価**：ゲームバランス調整の妥当なアセット化。全ゲーム設定がデータ駆動ではなく、強化定義・価格はコードに残る。
13. **弱点・改善**：確率重み選択が0～1乱数と整合せず、現行スケジュールでは3番目以降が選ばれない。SOのOnValidate検証・重み正規化・編集モードの検証ツールを追加すべき。

## 4. 強化ロードの型別マージとレベル範囲補正

1. **名称**：強化データの列挙型キーによるロード復元。
2. **実装概要**：ロード配列をUpgradeType辞書にし、現行定義へ型ごとにマージ。重複・欠落を警告し、レベルを範囲内にClampし、CurrentLevelとReleaseLevelの関係を整える。
3. **解決する課題**：配列順変更、古いセーブのReleaseLevel欠落、範囲外データによる強化UIの不整合。
4. **実装前に想定される問題**：データ配列の順番変更で別強化へ値が割当たる、現在レベルが解放レベルを超える。
5. **技術・設計**：enum識別子、Dictionary、`LoadLevels`、定義整合性の検査。
6. **処理の流れ**：JSON復元 → enum別Map化 → 現行LevelPropertyごとに値取得 → Clamp／不足保持 → UI向けロードイベント発火。
7. **該当ファイル**：[BattleSettingConfig.Upgrades.cs](../Assets/Scripts/ScriptableObject/BattleSettingConfig.Upgrades.cs)、[LevelProperty.cs](../Assets/Scripts/Systems/LevelProperty.cs)、[SaveAndLoadService.cs](../Assets/Scripts/Service/Implementations/SaveAndLoadService.cs)。
8. **コード根拠**：欠落・重複・未定義UpgradeTypeのログ、LoadLevelsの範囲補正、Snapshot経由の保存。
9. **メリット**：既存データの項目順序に依存しづらく、範囲外値による破綻を緩和。
10. **拡張・保守**：新UpgradeTypeの追加漏れをロード時／初回検証で発見しやすい。
11. **面接説明**：セーブ互換をどう扱い、旧形式・欠落データをどのように段階的に補正するか説明できる。
12. **客観評価**：小規模なローカルセーブとして防御性を高めている。完全なVersion migrationや破損JSON処理ではない。
13. **弱点・改善**：LevelProperty snapshotは配列の浅いコピーで、中のオブジェクトは共有参照。JSON化は即時だが、名称ほどの完全分離ではない。価格定義・旧アセット値との二重管理も解消が必要。

## 5. UI Toolkitのテンプレートと限定的なプール

1. **名称**：UI Toolkitによるパネル構成とダメージテキスト再利用。
2. **実装概要**：UXMLの画面／テンプレートをC#モジュールへ渡し、強化・難易度UIを動的生成。WorldCanvasManagerはダメージTextをQueueで再利用する。
3. **解決する課題**：UIレイアウト・装飾と表示ロジックの混在、頻繁に生成されるダメージ表示の都度生成。
4. **実装前に想定される問題**：UI構造変更がコードに埋まり、オブジェクト生成数が増える可能性。
5. **技術・設計**：UI Toolkit、UXML/USS、VisualTreeAsset、パネルStack、Queue pool。
6. **処理の流れ**：UIDocumentから名前で要素を検索 → UIModuleBaseが初期化 → Screen controllerが遷移 → damage表示はpool取得、終了時返却。
7. **該当ファイル**：[ShowPanelNavigationControllerBase.cs](../Assets/Scripts/UI/ShowPanelNavigationControllerBase.cs)、[UpgradePanelUI.cs](../Assets/Scripts/UI/Home/UpgradePanelUI.cs)、[WorldCanvasManager.cs](../Assets/Scripts/UI/WorldCanvasManager.cs)、[Root_HomeMain.uxml](../Assets/UI%20Toolkit/Root_HomeMain.uxml)。
8. **コード根拠**：テンプレートからの動的生成、Stackによるパネル遷移、初期10件のダメージテキストQueueと枯渇時Instantiate。
9. **メリット**：画面構造と動的表示を分け、通常ダメージポップアップの再生成を一定範囲で回避する。
10. **拡張・保守**：同じモジュール方式でホーム画面パネルを追加可能。要素名の文字列依存は残る。
11. **面接説明**：UI Toolkitを採用した範囲、テンプレートを用いる画面とuGUIを残すワールドテキストを分けた理由（採用理由自体は推定）を説明できる。
12. **客観評価**：UIの整理と局所的プールは確認できるが、全面的なパフォーマンス対策ではない。
13. **弱点・改善**：Miss／Critical／通貨／攻撃Effectは個別生成。プールの上限・再入可能性、イベント解除・UXML名の検証、再ロード時の購読重複対策が必要。
