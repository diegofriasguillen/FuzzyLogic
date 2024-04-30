using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; 

public class IAEnemy : MonoBehaviour
{
    public GameObject Jugador;
    public GameObject BaseVida;
    public GameObject BaseBalas;
    public NavMeshAgent agent;
    public int VidaPLayer;
    public int minVidaPlayer;
    //public GameObject ObjetoPrueba;
  //private Transform jugadorTransform;
    public float vida;
    public int balas;
    public float minArmor;
    public float minVida;
    public float minDistancia;
    public float Distance;
    public GameObject proyectil;
    public bool atacando;
    public int prueba = 0;
    public GameObject[] cantidadBalas;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        //Jugador = GetComponent<GameObject>();
        minArmor = (balas * 0.30f);
        minVida = (balas * 0.30f);
        cantidadBalas = new GameObject[30];

        for (int i = 0; i < cantidadBalas.Length; i++)
        {
            GameObject NuevoProyectil = Instantiate(proyectil, transform.position, Quaternion.identity);
            cantidadBalas[i]=NuevoProyectil.GetComponent<GameObject>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (prueba)
        {
            case 0: atacar(); break;
            case 1: HuirArmor(); break;
            case 2: HuirVida(); break;
            case 3 : break;

        }
        /*   Distance = Vector3.Distance(transform.position, Jugador.transform.position);
           if (vida>minVida)
           {
               if (Distance> minDistancia && balas > minArmor)
               {
                   if (atacando == false)
                   {
                       atacando = true;
                       atacar();
                   }
               }
               if (Distance <= minDistancia && balas <= minArmor && balas != 0 && VidaPLayer >30)
               {

                   //agent.isStopped = false;
                   agent.destination = BaseBalas.transform.position;

               }
           }
           else
           {
               agent.destination = BaseVida.transform.position;

           }*/

    }

   

    public IEnumerator atacar() 
    {
        Console.WriteLine("disparando");
        yield return new WaitForSeconds(0.3f);
        atacando = false;
    }

    public void disparando() 
    {
      
    
    
    }

    public void HuirVida()
    {
        agent.destination = BaseVida.transform.position;
    }

    public void HuirArmor()
    {
        agent.destination = BaseBalas.transform.position;
    }

   
}
