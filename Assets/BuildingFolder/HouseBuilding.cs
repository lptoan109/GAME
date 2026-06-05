using UnityEngine;

public class HouseBuilding : Building
{
    [Header("Cấu hình giới hạn dân")]
    public int populationBonus = 3; // Mỗi nhà cộng 3 dân 

    protected override void Start()
    {
        buildingName = "Nhà Dân";
        [cite_start]// GDD: HP Nhà dân theo 3 cấp là 75 - 110 - 125 [cite: 48]
        hpPerLevel = new float[3] { 75f, 110f, 125f };
        base.Start();

        // Cộng sức chứa dân vào hệ thống khi xây xong
        AddPopulation();
    }

    void AddPopulation()
    {
        // Giả sử sau này bạn làm biến toàn cục quản lý tổng số dân max
        Debug.Log($"Nhà dân mới xây thành công! Tăng thêm {populationBonus} giới hạn dân.");
    }

    protected override void OnBuildingDestroyed()
    {
        // Trừ lại giới hạn dân nếu nhà bị địch đập sập
        Debug.Log($"Nhà dân bị sập! Giảm {populationBonus} giới hạn dân.");
        base.OnBuildingDestroyed();
    }
}