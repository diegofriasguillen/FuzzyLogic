using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using TMPro;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public GameObject healthStation;
    public GameObject ammoStation;

    private NavMeshAgent agent;

    //fuzzy
    private float fuzzyPlayerHealth;
    private float fuzzyAmmo;
    private float fuzzyDistancePlayer;
    private float fuzzyDistanceAmmo;
    private float fuzzyDistanceHealth;

    //bullets
    private int maxAmmo = 8;
    public int currentAmmo;
    private bool canShoot = true;
    private float shootCooldown = 1.5f;
    private float shootRange = 8f;
    public GameObject bulletPrefab;
    public Transform shootPoint;
    private float bulletSpeed = 10f;
    private float reloadTime = 3.5f;

    //health
    private int maxHealth = 100;
    public int currentHealth;

    //panel 
    public GameObject endPanel;

    private bool isReloading = false;
    private bool isHealing = false;
    

    //distance
    private float desiredDistanceToPlayer = 8f;

    //text
    public TMP_Text textoVida;
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentAmmo = maxAmmo;
        currentHealth = maxHealth;
    }

    private void Update()
    {
        Fuzzify();
        textoVida.text = "Enemy Life: " + fuzzyPlayerHealth.ToString();
        MakeDecision();
        if (!isReloading && !isHealing && currentAmmo > 0 && Vector3.Distance(transform.position, player.position) <= shootRange)
        {
            ShootAtPlayer();
        }
    }

    //fuzzylogic
    private void Fuzzify()
    {
        fuzzyPlayerHealth = (currentHealth * 100) / maxHealth;
        fuzzyAmmo = (currentAmmo * 100) / maxAmmo;
        fuzzyDistancePlayer = Vector3.Distance(transform.position, player.position);
        fuzzyDistanceAmmo = Vector3.Distance(transform.position, ammoStation.transform.position);
        fuzzyDistanceHealth = Vector3.Distance(transform.position, healthStation.transform.position);
    }

    
    private void MakeDecision()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= desiredDistanceToPlayer)
        {
            agent.SetDestination(transform.position);
            return;
        }

        //runbitchruuuun
        if (fuzzyPlayerHealth <= 30 && distanceToPlayer < 10)
        {
            Flee();
            return;
        }
        if (fuzzyAmmo <= 30 && distanceToPlayer < 10)
        {
            Flee();
            return;
        }
        if (fuzzyPlayerHealth <= 30 && distanceToPlayer > 10)
        {
            MoveToHealthStation();
            return;
        }

        if (fuzzyPlayerHealth <= 20 && fuzzyDistanceHealth > 11)
        {
            Flee();
            return;
        }

        //healling
        if (fuzzyPlayerHealth <= 40 && fuzzyDistanceHealth <= 10)
        {
            MoveToHealthStation();
            return;
        }
        if (fuzzyPlayerHealth <= 40 && distanceToPlayer > 10)
        {
        MoveToHealthStation();
        return;
        }

        //reload

        if (fuzzyAmmo <= 40 && fuzzyDistanceAmmo <= 15)
        {
            MoveToAmmoStation();
            return;
        }
        else
        {
            agent.SetDestination(player.position);
          }
       

    }

    private void Flee()
    {
        Vector3 fleeDirection = transform.position - player.position;
        fleeDirection.y = 0;
        fleeDirection.Normalize();
        Vector3 fleePoint = transform.position + fleeDirection * 8f;
        agent.SetDestination(fleePoint);
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
        if (other.gameObject == ammoStation && !isReloading)
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
