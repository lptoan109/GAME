using UnityEngine;

public class UnitGatherer : MonoBehaviour
{
    // Khai báo các trạng thái bằng Enum cho dễ quản lý
    public enum GathererState { Idle, Gathering, Delivering, Returning }

    [Header("Cấu hình Khai thác & Sức chứa")]
    public float gatherRange;
    public int gatherAmountPerTick;
    public float gatherSpeed;
    public int maxCapacity;       // Sức chứa tối đa của balo dân làng

    [Header("Trạng thái hiện tại (Xem trong Inspector)")]
    public GathererState currentState = GathererState.Idle;
    public string currentResourceType; // Loại tài nguyên đang cầm trong balo
    public int currentLoad = 0;        // Số lượng tài nguyên đang cầm hiện tại

    private float gatherCooldown = 0f;
    private ResourceSource currentResourceNode; // Mỏ tài nguyên mục tiêu
    private TownCenter targetTownCenter;        // Nhà chính mục tiêu để về cất đồ

    private GridMovement movementComponent;
    private SelecttableUnit selectionComponent;
    private Grid gridLayout;

    void Start()
    {
        movementComponent = GetComponent<GridMovement>();
        selectionComponent = GetComponent<SelecttableUnit>();
        gridLayout = FindObjectOfType<Grid>();
    }

    void Update()
    {
        if (gatherCooldown > 0) gatherCooldown -= Time.deltaTime;

        // 1. NGƯỜI CHƠI RA LỆNH (Click chuột trái vào mỏ)
        if (selectionComponent != null && selectionComponent.IsUnitSelected())
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

                if (hit.collider != null && hit.collider.GetComponent<ResourceSource>() != null)
                {
                    // Chọn mỏ mới -> Xóa balo cũ (hoặc giữ lại tùy bạn, ở đây tạm xóa để đổi nghề nhanh)
                    currentResourceNode = hit.collider.GetComponent<ResourceSource>();
                    currentLoad = 0;
                    currentState = GathererState.Gathering;
                    Debug.Log(gameObject.name + " bắt đầu đi farm mỏ mới.");
                }
                else if (hit.collider == null)
                {
                    // Click đất trống -> Hủy trạng thái farm để đi bộ tự do
                    currentResourceNode = null;
                    currentState = GathererState.Idle;
                }
            }
        }

        // 2. CỖ MÁY TRẠNG THÁI TỰ ĐỘNG (AUTOMATIC STATE MACHINE)
        switch (currentState)
        {
            case GathererState.Gathering:
                HandleGatheringState();
                break;

            case GathererState.Delivering:
                HandleDeliveringState();
                break;

            case GathererState.Returning:
                HandleReturningState();
                break;
        }
    }

    // --- TRẠNG THÁI 1: ĐANG ĐI FARM / ĐANG CHẶT CÂY ---
    void HandleGatheringState()
    {
        if (currentResourceNode == null)
        {
            currentState = GathererState.Idle;
            return;
        }

        float distance = Vector3.Distance(transform.position, currentResourceNode.transform.position);

        if (distance <= gatherRange)
        {
            if (movementComponent != null) movementComponent.StopMoving();

            if (gatherCooldown <= 0f)
            {
                // Tính toán xem balo còn chứa được bao nhiêu
                int spaceLeft = maxCapacity - currentLoad;
                int toGather = Mathf.Min(gatherAmountPerTick, spaceLeft);

                if (toGather > 0)
                {
                    int amountGathered = currentResourceNode.Gather(toGather);
                    currentLoad += amountGathered;
                    currentResourceType = currentResourceNode.type.ToString();

                    Debug.Log($"[Balo dân] Đang cầm: {currentLoad}/{maxCapacity} {currentResourceType}");
                    gatherCooldown = gatherSpeed;
                }

                // NẾU BALO ĐÃ ĐẦY -> CHUYỂN SANG TRẠNG THÁI VỀ CẤT ĐỒ
                if (currentLoad >= maxCapacity)
                {
                    targetTownCenter = FindObjectOfType<TownCenter>(); // Tìm Nhà chính gần nhất
                    if (targetTownCenter != null)
                    {
                        currentState = GathererState.Delivering;
                        Debug.Log("Balo đã đầy! Đang gánh đồ chạy về Nhà Chính...");
                    }
                    else
                    {
                        Debug.LogError("Không tìm thấy Nhà chính (TownCenter) nào trên map để cất đồ!");
                        currentState = GathererState.Idle;
                    }
                }
            }
        }
        else
        {
            // Di chuyển tiến lại gần mỏ
            MoveToGridPosition(currentResourceNode.transform.position);
        }
    }

    // --- TRẠNG THÁI 2: ĐANG XÁCH ĐỒ VỀ NHÀ CHÍNH CẤT ---
    void HandleDeliveringState()
    {
        if (targetTownCenter == null)
        {
            currentState = GathererState.Idle;
            return;
        }

        float distance = Vector3.Distance(transform.position, targetTownCenter.transform.position);

        if (distance <= gatherRange) // Đến rìa nhà chính
        {
            if (movementComponent != null) movementComponent.StopMoving();

            // CẤT ĐỒ: Cộng điểm vào kho tổng ResourceManager đầu não
            ResourceManager.Instance.AddResource(currentResourceType, currentLoad);
            Debug.Log($" Đã cất {currentLoad} {currentResourceType} vào Nhà Chính!");

            // Xóa sạch balo sau khi cất
            currentLoad = 0;

            // NẾU MỎ CŨ VẪN CÒN -> TỰ ĐỘNG CHẠY QUAY LẠI FARM TIẾP
            if (currentResourceNode != null)
            {
                currentState = GathererState.Returning;
                Debug.Log("Vác cuốc chạy ngược lại mỏ cũ để farm tiếp...");
            }
            else
            {
                currentState = GathererState.Idle; // Mỏ cũ nát rồi thì đứng chơi
            }
        }
        else
        {
            // Di chuyển tiến lại gần Nhà chính
            MoveToGridPosition(targetTownCenter.transform.position);
        }
    }

    // --- TRẠNG THÁI 3: ĐANG TRÊN ĐƯỜNG QUAY LẠI MỎ CŨ ---
    void HandleReturningState()
    {
        if (currentResourceNode == null)
        {
            currentState = GathererState.Idle;
            return;
        }

        float distance = Vector3.Distance(transform.position, currentResourceNode.transform.position);

        // Chạy lại đến mỏ cũ thì tự động kích hoạt lại trạng thái Chặt/Đào
        if (distance <= gatherRange)
        {
            currentState = GathererState.Gathering;
        }
        else
        {
            MoveToGridPosition(currentResourceNode.transform.position);
        }
    }

    // Hàm phụ trợ ép tọa độ Grid để di chuyển
    void MoveToGridPosition(Vector3 targetWorldPos)
    {
        if (movementComponent != null && gridLayout != null)
        {
            Vector3Int cell = gridLayout.WorldToCell(targetWorldPos);
            Vector3 gridPos = gridLayout.GetCellCenterWorld(cell);
            movementComponent.SetNewTarget(gridPos);
        }
    }
}