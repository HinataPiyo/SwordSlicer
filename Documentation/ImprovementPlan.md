# 技術的課題と改善計画

優先度はプレイ内容の誤り・クラッシュリスク・将来の変更コストを基準にした提案であり、作業見積ではない。

| 優先度／難易度 | 現状 | 問題点 | 改善案 | 期待効果 |
|---|---|---|---|---|
| **P0 / 低** | `EnemySpawnController.ChooseSpawnEnemy` は0～1乱数と累積Weightを比較する。Scheduleの重み合計は1を超える。 | 先頭2件で累積が1超となり、3件目以降は抽選されない。敵構成の仕様が実際に反映されない。 | `Random.value * totalWeight` と比較する、または正規化確率を作る。合計0／負値／null Prefabも検査し、重み順序を変えたテストを追加。 | データの重みを意図した相対確率として機能させ、敵の多様性・難易度検証を可能にする。 |
| **P1 / 低～中** | `GameManager.OnGameOver` はstatic Action。GameManager／ResultServiceが匿名delegateで購読し解除しない。 | Scene再ロード後に購読が蓄積し、古いScene由来の処理や多重結果表示につながる可能性。 | 名前付きハンドラに変更し、OnDestroyで解除。GameManagerを唯一のイベント所有者にし、GameOverを一度だけ通知する。 | リトライ・ホーム遷移を繰返しても結果処理が一回に保たれ、寿命が明確になる。 |
| **P1 / 中** | 敵クラスはData取得失敗の一部を `enabled=false` にする一方、Spawner・Prefab参照・UIでは前提参照が多い。 | Inspector設定漏れが実行時NullReferenceや不完全状態につながる。 | Scene／Prefabの必須参照をOnValidateまたはEditor検証で検査。初期化の失敗を明示し、敵生成を中止／エラー通知する。 | 設定ミスをPlay前に検出し、原因調査時間を短縮する。 |
| **P1 / 中** | 敵のGoblinMovementBaseはターゲット方向へ一定距離をTransform加算する。 | 1回の移動距離が残距離を超えると防衛ラインを越え、Y差0.1以内に戻らず到達判定されない可能性がある。 | 残距離で移動量をClampする（MoveTowards等）。境界前後・大きい倍率・短経路のテストを追加。 | 敵の防衛到達・ゲームオーバーが安定し、移動速度調整時の回帰を防止する。 |
| **P1 / 中** | SaveAndLoadServiceはPlayerPrefsからJSONを復元し、強化・通貨・音量へ渡す。強化値には一部Clampがある。 | 不正／途中世代／欠落データ、負の通貨、将来のSaveData変更への処理が限定的。 | SaveDataにVersionを追加し、読み込み検証・Version別Migration・失敗時のユーザー通知・バックアップを実装。フィールド範囲を検査する。 | セーブ破損やアップデート時の進行損失を減らす。 |
| **P2 / 低～中** | 価格は `BattleSettingConfig.Upgrades.cs` static配列にあり、BattleSettingConfig.assetには現行コードと対応しない旧 `priceEntries` が残る。 | 実際に使われる値がInspectorの見た目から判別しづらく、バランス調整の誤りを招く。 | 価格を単一SO定義へ移して現行データから古い欄を整理するか、コード固定を明記してアセットの旧データを削除。enumとの整合検査は維持。 | 正規データ源が一つになり、価格調整・レビュー・新強化追加が予測可能になる。 |
| **P2 / 中** | NUnit/Test Framework packageはあるが、テストコード／CI workflowを確認できない。 | 抽選、難易度曲線、強化ロード、ゲームオーバー再ロード等の回帰を自動検出できない。 | 小さな純粋計算をテスト可能なクラスへ切り出し、Editor/EditModeで計算・移行テスト、PlayModeでScene連携テストを追加。CIで実行する。 | 抽選修正やパラメータ変更の安全性を継続検証できる。 |
| **P2 / 中** | 通常DamageTextだけ10個のプール。他のポップアップ・敵・剣・EffectはInstantiate/Destroy。 | 頻度が高い場合の割当／GC負荷は未計測。プール化の効果も実測されていない。 | まず対象端末でProfiler計測し、頻出で負荷が確認された生成物だけプール化。プール上限・返却・初期化を共通化。 | 実測根拠に基づく最適化になり、不要な複雑化を避ける。 |
| **P2 / 低** | 攻撃はOverlapCircleの単一Colliderを取得し、そのColliderからEnemyHealthを検索。 | 複数敵が重なった場合の対象選択がPhysics結果に依存しうる。子Collider等ではEnemyHealthを直接見つけない場合がある。 | ゲーム仕様として単体対象を明示するか、OverlapCircleAllと距離／優先順位で選ぶ。子Colliderを考慮する場合はGetComponentInParentを使う。 | 狙いと命中結果が明確になり、敵密集時の挙動を制御できる。 |
| **P2 / 低** | SwordAttackは対象が見つからないとき攻撃状態を解除せずreturnする。 | 攻撃範囲内に敵が現れるまでOverlapCircleを毎フレーム実行し、判定頻度が攻撃間隔より高くなる。 | 意図を決め、対象なしでも攻撃周期をResetするか、一定間隔の再試行タイマーを設ける。空状態と敵侵入時の動作をテストする。 | 空の攻撃範囲での不要なPhysics問い合わせを減らし、攻撃タイミングを明確にできる。 |
| **P3 / 低** | AdjustAspectRatioは毎UpdateでScreen寸法からViewportを再計算。 | 画面寸法が変わらないフレームでも同処理を繰返す。 | Screen.width/heightの前回値をキャッシュし、変更時だけ再計算する。 | 不必要な毎フレーム処理を減らす。性能効果はProfilerで確認する。 |
| **P3 / 低～中** | UIはUXML名検索とstatic／instanceイベント購読を組み合わせる。UpgradePanelUIの再初期化経路がある。 | 名前変更や再初期化で参照不一致、重複登録、古いUI参照が残る可能性。 | 初期化・解除を対称化し、再初期化時に全コレクションをクリア。UXML必須名を検証する。 | UI変更の安全性とScene再入場の安定性を向上。 |

## 推奨実施順

1. 抽選の重み正規化と確率分布テスト。
2. GameOverイベントの所有権・購読解除とシーン再ロード検証。
3. 敵到達判定境界とセーブ読み込みの異常系テスト。
4. 価格データの単一化・Inspectorでの検証。
5. Profilerでボトルネックを測定し、必要な範囲だけプール化。

改善後にREADMEと本資料の「敵が出現する」「手動セーブ」等の説明を現行動作と再照合する。
