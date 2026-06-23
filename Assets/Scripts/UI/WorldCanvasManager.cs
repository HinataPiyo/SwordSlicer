using System.Collections.Generic;
using UnityEngine;

public class WorldCanvasManager : MonoBehaviour
{
    public static WorldCanvasManager I { get; private set; }

    [SerializeField] PopUpDamageText popUpDamageTextPrefab;
    [SerializeField] int initCreateCount = 10;
    [SerializeField] GameObject attackMissTextPrefab;

    Queue<PopUpDamageText> damageTextPool = new Queue<PopUpDamageText>();

    void Awake()
    {
        if(I == null) I = this;

        for (int i = 0; i < initCreateCount; i++)
        {
            PopUpDamageText damageText = Instantiate(popUpDamageTextPrefab, transform);
            damageText.gameObject.SetActive(false);
            damageTextPool.Enqueue(damageText);
        }
    }

    public void ShowDamageText(float damage, Vector2 position)
    {
        PopUpDamageText damageText;
        if (damageTextPool.Count > 0)
        {
            damageText = damageTextPool.Dequeue();      // プールから取得
        }
        else
        {
            damageText = Instantiate(popUpDamageTextPrefab, transform);     // プールが空の場合は新たに生成
        }

        damageText.gameObject.SetActive(true);

        // ダメージテキストの位置と内容を設定
        damageText.transform.position = position;
        damageText.SetDamageUI(damage);
    }

    public void ReturnDamageTextToPool(PopUpDamageText damageText)
    {
        damageText.gameObject.SetActive(false);
        damageTextPool.Enqueue(damageText);     // プールに戻す
    }

    public void ShowAttackMissText(Vector2 position)
    {
        Instantiate(attackMissTextPrefab, position, Quaternion.identity, transform);
    }
}