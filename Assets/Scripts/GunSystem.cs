using UnityEngine;
using TMPro;

public class GunSystem : MonoBehaviour
{
    public int damage;
    public float timeBetweenShooting;
    public float spread;
    public float range;
    public float reloadTime;
    public float timeBetweenShots;
    public int magazineSize;
    public int bulletsPerTap;

    public LayerMask enemy;

    int bulletsLeft;
    int bulletsShot;

    bool readyToShoot;
    bool reloading;

    public RaycastHit rayHit;
    public KeyCode shootKey = KeyCode.Mouse0;

    Vector3 direction;

    [SerializeField] public TextMeshProUGUI bulletsMagazine;
    [SerializeField] GameObject shootPivot;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Camera playerCamera;

    private void Start()
    {
        readyToShoot = true;
        reloading = false;
    }

    private void Update()
    {
        //UpdateBulletsMagazine();
        CheckShoot();
    }

    //private void UpdateBulletsMagazine()
    //{
    //    bulletsMagazine.text = (bulletsLeft + " / " + magazineSize);
    //}

    private void CheckShoot()
    {
        if (readyToShoot && !reloading && bulletsLeft > 0 && Input.GetKeyUp(shootKey))
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }

        if (Input.GetKeyDown("r"))
        {
            Reload();
        }
    }

    public void Shoot()
    {
        readyToShoot = false;

        lineRenderer.enabled = false;
        lineRenderer.transform.position = shootPivot.transform.position;
        lineRenderer.SetPosition(0, shootPivot.transform.position);

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 spreadDirection = shootPivot.transform.forward + new Vector3(x, y, 0);

        if (Physics.Raycast(shootPivot.transform.position, spreadDirection, out rayHit, range))
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(1, rayHit.point);

            HealthSystem health = rayHit.collider.GetComponent<HealthSystem>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }

        bulletsLeft--;
        bulletsShot--;

        Invoke("ResetShoot", timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0)
        {
            Invoke("Shoot", timeBetweenShots);
        }
    }

    private void ResetShoot()
    {
        readyToShoot = true;
        lineRenderer.enabled = false;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
}