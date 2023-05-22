using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class BombController : Singleton<BombController>
{

    public GameObject bombPrefab; // Bomba prefab�
    public float bombFuseTime = 3f; // Bomba fitil s�resi
    public int bombAmount = 1; // Sahip olunan bomba say�s�
    private int bombsRemaining; // Kullan�labilir bomba say�s�
    [SerializeField] private int maxBombAmount = 6; // Maksimum bomba say�s�
    public bool ExplosionButton = false; // Patlama tu�u
    public bool isActiveController = true; // Kontrolc� aktif mi?

    [Header("Explosion")]
    public Explosion explosionPrefab; // Patlama prefab�
    public LayerMask explosionLayerMask; // Patlama katman� maskesi
    public float explosionDuration = 1f; // Patlama s�resi
    public int explosionRadius = 1; // Patlama yar��ap�
    [SerializeField] private int MaxexplosionRadius = 7; // Maksimum patlama yar��ap�
    public float bombDropInterval = 0.9f; // Bomba b�rakma aral���
    public float bombDropDuration = 7f; // Bomba b�rakma s�resi

    [Header("Destructible")]
    public Tilemap destructibleTiles; // Y�k�labilir tilemap
    public Destructible destructiblePrefab; // Y�k�labilir prefab

    public bool canIPush = false; // �terek itme �zelli�i
    [SerializeField] private LayerMask bombPushLayers; // �terek itme katman�
    private List<GameObject> bombs = new List<GameObject>(); // B�rak�lan bombalar�n listesi

    public GameObject PushGo;
    public GameObject BombDropGo;

    public float placeBombRadius = 0.5f;
    public LayerMask bombCantPlaceLayers;
    private void Start()
    {
        InputManager.Instance.OnClickBomb.AddListener(OnClickBomb);
        InputManager.Instance.OnLeavekBomb.AddListener(StartCorutineBomb);
    }
    private void OnEnable()
    {
        bombsRemaining = bombAmount;


    }

    private void OnDisable()
    {
        StopAllCoroutines();
        InputManager.Instance.OnClickBomb.RemoveListener(OnClickBomb);
        InputManager.Instance.OnLeavekBomb.RemoveListener(StartCorutineBomb);

    }

    private void Update()
    {


        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnClickBomb();
        }

    }

    // Patlama tu�una bas�ld���nda t�m bombalar� patlat�r
    public void OnClickBomb()
    {
        if (!ExplosionButton) return;
        var copyOfBombs = bombs.ToArray();
        foreach (var bomb in copyOfBombs)
        {
            Bombed(bomb);
        }
    }

    // Bomba yerle�tirme i�lemi
    public IEnumerator PlaceBomb()
    {

        Vector2 bombPosition = transform.position;
        bombPosition.x = Mathf.Round(bombPosition.x);
        bombPosition.y = Mathf.Round(bombPosition.y);




        // Duvar�n �zerindeyken bomba b�rakmay� engelle
        Collider2D hit = Physics2D.OverlapCircle(bombPosition, placeBombRadius, bombCantPlaceLayers);

        if (hit == null)
        {
            // Bombay� ekle ve di�er i�lemleri yap
            GameObject bomb = Instantiate(bombPrefab, bombPosition, Quaternion.identity);
            bombs.Add(bomb);
            bombAmount--;
            bombsRemaining--;

            if (!ExplosionButton)
            {
                yield return new WaitForSeconds(bombFuseTime);
                Bombed(bomb);
            }
        }

    }

    private void OnDrawGizmos()
    {
        Vector2 bombPosition = transform.position;
        bombPosition.x = Mathf.Round(bombPosition.x);
        bombPosition.y = Mathf.Round(bombPosition.y);


        Gizmos.color = Color.red;

        Gizmos.DrawSphere(bombPosition, placeBombRadius);
    }

    private void Bombed(GameObject bomb)
    {
        // Bomban�n bulundu�u konumu al
        Vector2 position = bomb.transform.position;

        // Patlama efektini olu�tur ve ba�lang�� durumunu ayarla
        Explosion explosion1 = Instantiate(explosionPrefab, position, Quaternion.identity);
        explosion1.SetActiveRenderer(explosion1.start);
        explosion1.DestroyAfter(explosionDuration);

        // Patlamay� yukar�, a�a��, sola ve sa�a do�ru geni�let
        Explode(position, Vector2.up, explosionRadius);
        Explode(position, Vector2.down, explosionRadius);
        Explode(position, Vector2.left, explosionRadius);
        Explode(position, Vector2.right, explosionRadius);

        // Bomba say�s�n� ve kalan bomba say�s�n� art�r
        bombAmount++;
        bombsRemaining++;

        // Yok olan bombay� listeden kald�r ve yok et
        bombs.Remove(bomb);
        Destroy(bomb.gameObject);
    }

    private void Explode(Vector2 position, Vector2 direction, int length)
    {
        // Patlama uzunlu�u 0 veya daha k���k ise i�lemi sonland�r
        if (length <= 0)
        {
            return;
        }


        // Yeni pozisyonu hesapla
        position += direction;
        var hit = Physics2D.OverlapBox(position, Vector2.one / 2f, 0f, explosionLayerMask);
        // Patlama s�ras�nda engelle �arp��ma var m� kontrol et
        if (hit && !hit.CompareTag("Player"))
        {
            if (!hit.CompareTag("DontDestroy"))
            {
                // E�er varsa y�k�labilir tile'� temizle
                ClearDestructible(position, hit);
            }
            return;
        }

        // Patlama efektini olu�tur ve aktif durumu ayarla
        Explosion explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
        explosion.SetActiveRenderer(length > 1 ? explosion.middle : explosion.end);
        explosion.SetDirection(direction);
        explosion.DestroyAfter(explosionDuration);

        // Patlamay� geni�letmek i�in Explode fonksiyonunu tekrar �a��r
        Explode(position, direction, length - 1);
    }

    private void ClearDestructible(Vector2 position, Collider2D hit)
    {
        Instantiate(destructiblePrefab, position, Quaternion.identity);
        hit.gameObject.SetActive(false);
        StartCoroutine(SpawnWalls(hit.gameObject));
    }

    private IEnumerator SpawnWalls(GameObject wall)
    {
        yield return new WaitForSeconds(60f);
        Vector2 RoundedPosition;
        Collider2D hit;


        do
        {
            RoundedPosition = wall.transform.position;
            RoundedPosition.x = Mathf.Round(RoundedPosition.x);
            RoundedPosition.y = Mathf.Round(RoundedPosition.y);
            hit = Physics2D.OverlapCircle(RoundedPosition, 0.3f);
            Debug.Log("Bi�ey Var");
            yield return new WaitForSeconds(1f);
        } while (hit != null);
        wall.SetActive(true);

    }
    public void AddBomb()
    {
        // Bomba say�s�n� art�r, maksimum say�y� ge�miyorsa art��� uygula
        bombAmount++;
        if (bombAmount > maxBombAmount)
        {
            bombAmount = maxBombAmount;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Tetikleyiciden ��kan nesne bir bombaysa tetikleyici �zelli�ini kapat
        if (other.gameObject.layer == LayerMask.NameToLayer("Bomb"))
        {
            other.isTrigger = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Explosion"))
        {

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // �arp��an nesne bir bombaysa ve itebilme �zelli�i aktif ise
        if (collision.gameObject.CompareTag("Bomb") && canIPush)
        {
            // E�er hareket kontrolc�s� bile�eni varsa
            if (TryGetComponent<MovementController>(out var movementController))
            {
                // �tme i�lemi i�in gerekli de�i�kenleri tan�mla
                var radius = 0.6f;
                var direction = movementController.Direction;
                var startPos = (Vector2)collision.transform.position + direction * radius;
                RaycastHit2D hit = Physics2D.Raycast(startPos, direction, 500, bombPushLayers);

                Debug.Log($"bombay� it direction:{direction}");

                // E�er itme i�lemi s�ras�nda ba�ka bir nesne ile temas olduysa
                if (hit.collider != null)
                {
                    // Hedef pozisyonu belirle ve bomban�n "Push" metodunu �a��rarak itme i�lemini ger�ekle�tir
                    var targetPos = hit.point - direction * 0.5f;
                    collision.transform.GetComponent<Bomb>().Push(targetPos);
                }
            }
        }
    }

    // �oklu Bomba �temi fonksiyonu
    public void OnItemEaten()
    {
        StartCoroutine(DropBombsRoutine());
        BombDropGo.SetActive(true);
    }

    private IEnumerator DropBombsRoutine()
    {
        // Bomba b�rakma i�lemini belirli bir s�re boyunca tekrarla
        float timer = 0f;
        while (timer < bombDropDuration)
        {
            yield return new WaitForSeconds(bombDropInterval); // Belirlenen aral�klarla bomba b�rak

            // E�er hala bomba say�s� varsa bomba b�rakma i�lemini ba�lat
            if (bombAmount > 0)
            {
                StartCoroutine(PlaceBomb()); // Bomba b�rakma kodunu �al��t�r
            }

            timer += bombDropInterval;
        }
        BombDropGo.SetActive(false);
    }

    public void ExplosionRadius()
    {
        // Patlama yar��ap�n� art�r, maksimum de�eri a�m�yorsa art��� uygula
        explosionRadius++;
        if (explosionRadius > MaxexplosionRadius)
        {
            explosionRadius = MaxexplosionRadius;
        }
    }

    public void MaxExplosionRadius()
    {
        // Patlama yar��ap�n� maksimum de�ere ayarla, maksimum de�eri a�m�yorsa ayar� uygula
        explosionRadius = 6;
        if (explosionRadius > MaxexplosionRadius)
        {
            explosionRadius = MaxexplosionRadius;
        }
    }

    public void Push()
    {
        // �tme �zelli�ini etkinle�tir
        canIPush = true;
        PushGo.SetActive(true);
    }

    public void ExplosionButtonItem()
    {
        // Patlama d��mesini etkinle�tir
        ExplosionButton = true;
    }

    public void CheckActiveButton()
    {
        // E�er "B" tu�una bas�ld�ysa patlama d��mesini etkinle�tir ve bomba fitil s�resini ayarla
        if (Input.GetKeyDown(KeyCode.B))
        {
            ExplosionButton = true;
            bombFuseTime = 0.01f;
        }
    }


    public void StartCorutineBomb()
    {

        if (bombsRemaining > 0)
        {
            StartCoroutine(PlaceBomb());
        }

    }

}

