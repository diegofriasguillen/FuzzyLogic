using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public GameObject healthStation;
    public GameObject ammoStation;

    private NavMeshAgent agent;

    //bullets
    private int maxAmmo = 8;
    public int currentAmmo;
    private bool canShoot = true;
    private float shootCooldown = 1.5f;
    private float reloadTime = 3.5f;
    private float shootRange = 10f;
    public GameObject bulletPrefab;
    public Transform shootPoint;
    private float bulletSpeed = 10f;

    //health
    private int maxHealth = 100;
    public int currentHealth;

    //panel 
    public GameObject endPanel;

    private bool isReloading = false;
    private bool isHealing = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentAmmo = maxAmmo;
        currentHealth = maxHealth;
    }

    private void Update()
    {
        MakeDecision();
        if (!isReloading && !isHealing && currentAmmo > 0 && Vector3.Distance(transform.position, player.position) <= shootRange)
        {
            ShootAtPlayer();
        }
    }

    //movPOS
    private void MakeDecision()
    {
        if (IsLowHealth() && !isHealing)
        {
            MoveToHealthStation();
        }
        else if (IsOutOfAmmo() && !IsLowHealth() && !isReloading)
        {
            MoveToAmmoStation();
        }
        else
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (!isReloading && !isHealing && distanceToPlayer > shootRange && currentAmmo > 0)
            {
                agent.SetDestination(player.position);
            }
            else
            {
                agent.SetDestination(transform.position);
            }
        }
    }

    private void ShootAtPlayer()
    {
        if (canShoot && currentAmmo > 0)
        {
            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);

            Vector3 direction = (player.position - shootPoint.position).normalized;

            Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
            if (bulletRigidbody != null)
            {
                bulletRigidbody.velocity = direction * bulletSpeed;
            }

            Destroy(bullet, 3f);

            currentAmmo--;

            StartCoroutine(ShootCooldown());
        }
    }

    private bool IsLowHealth()
    {
        return currentHealth <= 40;
    }

    private bool IsOutOfAmmo()
    {
        return currentAmmo <= 0;
    }

    //movLife
    private void MoveToHealthStation()
    {
        agent.SetDestination(healthStation.transform.position);
        if (Vector3.Distance(transform.position, healthStation.transform.position) < 1f)
        {
            StartCoroutine(HealOverTime());
        }
    }

    //movAMMO
    private void MoveToAmmoStation()
    {
        agent.SetDestination(ammoStation.transform.position);
    }

    //bullets
    private void RefillAmmo()
    {
        currentAmmo = maxAmmo;
    }

    //health
    public void Heal()
    {
        currentHealth = maxHealth;
    }

    private System.Collections.IEnumerator ShootCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;
    }

    private System.Collections.IEnumerator ReloadAmmo()
    {
        isReloading = true;

        yield return new WaitForSeconds(reloadTime);

        RefillAmmo();

        isReloading = false;

        agent.isStopped = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == ammoStation && IsOutOfAmmo() && !isReloading)
        {
            agent.isStopped = true;
            StartCoroutine(ReloadAmmo());
        }
        else if (other.CompareTag("PlayerBullet"))
        {
            TakeDamage(20);
            Destroy(other.gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Time.timeScale = 0f;
        endPanel.SetActive(true);
        Debug.Log("Game Over");
    }

    //panel
    public void Restart()
    {
        StartCoroutine(RestartCoroutine());
    }

    private IEnumerator RestartCoroutine()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void InitialMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void Quit()
    {
        Application.Quit();
    }

    private IEnumerator HealOverTime()
    {
        isHealing = true;
        while (currentHealth < maxHealth)
        {
            yield return new WaitForSeconds(1f);
            currentHealth += 20;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        }
        isHealing = false;
    }
}
