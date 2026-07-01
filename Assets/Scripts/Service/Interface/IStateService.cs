using System.Collections.Generic;

public interface IStateService
{
    BattleSettingConfig.SwordDataByType CreateSword();
    float DamageAmount(BattleSettingConfig.SwordDataByType data, out bool isCritical);
    float SwordThrowForce();
    float SwordTurnForce();
    float SwordTurnReactTime();
    float StockInterval();
    float SwordAttackRange();
    float SwordAttackInterval(float rotateAmount);
    int CurrentMaxStock();
    float MaxRotateAmount();

    BattleSettingConfig.SwordDataByType[] SwordDatas();
    List<UpgradeEntry> UpgradeEntries();
}