using UnityEngine;

public class MinotaurMovement : GoblinMovement
{
    protected override void ConvertData()
    {
        // MinotaurDataSOにキャストして、Minotaur専用のデータを取得
        MinotaurDataSO minotaurData = Data as MinotaurDataSO;
        if (minotaurData != null)
        {
            convertData = minotaurData;
        }
        else
        {
            Debug.LogError("MinotaurMovement: Data is not of type MinotaurDataSO");
        }
    }
}