using UnityEngine;

public class AnimationDestroyEvent : MonoBehaviour
{
    [SerializeField] Transform target;
    System.Action onDestroy;    // アニメーションが終了してオブジェクトが削除されるときのコールバック
    public void DestroyObject()
    {
        if(onDestroy != null)
        {
            onDestroy.Invoke();
            return;
        }

        Destroy(target.gameObject);
    }

    /// <summary>
    /// アニメーションイベントにオブジェクトが削除されるときのコールバックを登録する
    /// </summary>
    /// <param name="onDestroy"></param>
    public void RegisterDestroy(System.Action onDestroy)
    {
        this.onDestroy += onDestroy;
    }
}