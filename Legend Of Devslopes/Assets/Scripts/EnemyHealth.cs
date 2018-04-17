using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour {

	[SerializeField] private int startingHealth = 20;
	[SerializeField] private float timeSinceLastHit = 0.5f;
	[SerializeField] private float disappearSpeed = 2f;

	private AudioSource audio;
	private float timer = 0f;
	private Animator anim;
	private UnityEngine.AI.NavMeshAgent nav;
	private bool isAlive;
	private Rigidbody rigidBody;
	private CapsuleCollider capsuleCollider;
	private bool disappearEnemy = false;
	private int currentHealth;
	private ParticleSystem blood;

	public bool IsAlive
	{
		get {return isAlive;}
	}

	// Use this for initialization
	void Start () {

		GameManager.instance.RegisterEnemy(this);
		rigidBody = GetComponent<Rigidbody> ();
		capsuleCollider = GetComponent<CapsuleCollider>();
		nav = GetComponent<UnityEngine.AI.NavMeshAgent> ();
		anim = GetComponent<Animator> ();
		audio = GetComponent<AudioSource> ();
		isAlive = true;
		currentHealth = startingHealth;
		blood = GetComponentInChildren<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if(disappearEnemy)
		{
			transform.Translate(-Vector3.up * disappearSpeed * Time.deltaTime);
		}
	}

	void OnTriggerEnter (Collider other)
	{
		if(timer >= timeSinceLastHit && !GameManager.instance.GameOver)
		{
			if(other.tag == "PlayerWeapon")
			{
				takeHit();
				blood.Play();
				timer = 0f;
			}
		}
	}

	void takeHit()
	{
		if(currentHealth > 0)
		{
			audio.PlayOneShot(audio.clip);
			anim.Play("Hurt");
			currentHealth -= 10;
			if(currentHealth<= 0)
			{
				isAlive = false;
				KillEnemy();
			}
		}
	}

	void KillEnemy()
	{

		GameManager.instance.KilledEnemy(this);
		capsuleCollider.enabled = false;
		nav.enabled = false;
		anim.SetTrigger("EnemyDie");
		rigidBody.isKinematic = true;

		StartCoroutine(removeEnemy()); // WORKING OR NOT?
	}

	IEnumerator removeEnemy()
	{
		yield return new WaitForSeconds(4f);
		disappearEnemy = true;
		yield return new WaitForSeconds(2f);
		Destroy(gameObject);
	}
}
