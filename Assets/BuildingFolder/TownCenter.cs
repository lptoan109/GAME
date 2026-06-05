using UnityEngine;

public class TownCenter : Building
{
    public static TownCenter Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    protected override void Start()
    {
        buildingName = "Nhà Chính";
        // GDD: HP Nhà chính theo 3 cấp là 600 - 800 - 1000 [cite: 48]
        hpPerLevel = new float[3] { 600f, 800f, 1000f };
        base.Start();
    }

    // Ghi đè hàm nâng cấp: Nhà chính tự nâng cấp không cần check điều kiện nhà khác 
    public bool UpgradeTownCenter()
    {
        if (currentLevel >= maxLevel) return false;
        currentLevel++;
        currentHP = hpPerLevel[currentLevel - 1];
        Debug.Log($"Nhà Chính đã tiến hóa lên Thời đại {currentLevel}!");
        return true;
    }

    protected override void OnBuildingDestroyed()
    {
        base.OnBuildingDestroyed();
        Debug.LogError("GAME OVER! Nhà chính đã bị phá hủy. Bạn đã thua cuộc!");
        // Bạn có thể gọi hàm hiển thị UI thất bại/chiến thắng ở đây [cite: 20, 42]
    }
}