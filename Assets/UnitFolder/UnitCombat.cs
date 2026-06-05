using UnityEngine;

public class UnitCombat : MonoBehaviour
{
    [Header("Chỉ số chiến đấu")]
    public float maxHP = 100f;
    public float currentHP;
    public float attackDamage = 20f;
    public float attackSpeed = 1f;
    public float attackRange = 1.5f; // Tầm đánh

    private float attackCooldown = 0f;
    private Transform currentTarget;

    private GridMovement movementComponent;
    private SelecttableUnit selectionComponent;
    private Grid gridLayout; // Cần thêm tham chiếu Grid để ép tọa độ ô khi đuổi theo

    void Start()
    {
        currentHP = maxHP;
        movementComponent = GetComponent<GridMovement>();
        selectionComponent = GetComponent<SelecttableUnit>();
        gridLayout = FindObjectOfType<Grid>(); // Tìm hệ thống Grid trong map
    }

    void Update()
    {
        // 1. ĐẾM NGƯỢC HỒI CHIÊU
        if (attackCooldown > 0) attackCooldown -= Time.deltaTime;

        // 2. NGƯỜI CHƠI RA LỆNH (Click chuột trái khi lính được chọn)
        if (selectionComponent != null && selectionComponent.IsUnitSelected())
        {
            if (Input.GetMouseButtonDown(0)) // Click chuột trái
            {
                // Bắn tia Raycast xem người chơi click trúng cái gì
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

                if (hit.collider != null && hit.collider.CompareTag("Enemy") && hit.collider.gameObject != gameObject)
                {
                    // TRƯỜNG HỢP A: Click TRÚNG kẻ địch -> Khóa mục tiêu để đuổi theo đánh
                    currentTarget = hit.collider.transform;
                    Debug.Log(gameObject.name + " đuổi theo mục tiêu: " + currentTarget.name);
                }
                else
                {
                    // TRƯỜNG HỢP B: Click ra ĐẤT TRỐNG (Hoặc vật thể không phải địch)
                    // HỦY BỎ MỤC TIÊU CŨ NGAY LẬP TỨC để lính tập trung bỏ chạy!
                    currentTarget = null;
                    Debug.Log(gameObject.name + " hủy mục tiêu để di chuyển ra vị trí mới.");
                }
            }
        }

        // 3. XỬ LÝ LOGIC CHIẾN ĐẤU & ĐUỔI THEO (Giữ nguyên đoạn này bên dưới)
        if (currentTarget != null)
        {
            float distance = Vector3.Distance(transform.position, currentTarget.position);

            if (distance <= attackRange)
            {
                if (movementComponent != null) movementComponent.StopMoving();

                if (attackCooldown <= 0f)
                {
                    AttackTarget();
                    attackCooldown = attackSpeed;
                }
            }
            else
            {
                if (movementComponent != null && gridLayout != null)
                {
                    Vector3Int enemyCell = gridLayout.WorldToCell(currentTarget.position);
                    Vector3 enemyGridPos = gridLayout.GetCellCenterWorld(enemyCell);
                    movementComponent.SetNewTarget(enemyGridPos);
                }
            }
        }
    }

    void AttackTarget()
    {
        if (currentTarget == null) return;

        UnitCombat enemyCombat = currentTarget.GetComponent<UnitCombat>();
        if (enemyCombat != null)
        {
            Debug.Log(gameObject.name + " CHÉM " + currentTarget.name + " mất " + attackDamage + " HP!");
            enemyCombat.TakeDamage(attackDamage);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            Debug.Log(gameObject.name + " đã chết!");
            Destroy(gameObject);
        }
    }
}