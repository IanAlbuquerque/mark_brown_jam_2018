﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAITeste : MonoBehaviour {

	public string enemyTag;
	public string heroTag;
    public bool FollowEnter=false;
	public bool FireEnter=false;
	public bool StopEnter = false;
	public bool StopEnterEnemy = false;

	public GameObject enemyEntered;

    public float moveSpeed;
	public GameObject bulletSpawnerPoint;


	//public float detectDist;

	public float shootDuration;
	public GameObject bulletPrefab;
	private float shootCount;

	private GameObject hero;
	private Rigidbody2D thisEnemyRigidBody;

	public float shootPreparationDuration;

	private float shootPreparationCounter;

	private bool isInShootPreparation;

	private bool directionToggle;

	public float minDirectionToggleTime;
	public float maxDirectionToggleTime;
	private float directionToggleTime;
	public float directionToggleProbability;
	private float directionToggleCount;

	// Use this for initialization
	void Start () {
		this.shootCount = 0;
		GameObject[] heroes = GameObject.FindGameObjectsWithTag(this.heroTag);
		this.hero = heroes[0];
		this.thisEnemyRigidBody = this.GetComponent<Rigidbody2D>();
		this.directionToggle = Random.Range(0.0f, 1.0f) > 0.5f;
		this.directionToggleTime = Random.Range(this.minDirectionToggleTime, this.maxDirectionToggleTime);
	}

	// Update is called once per frame
	void Update () {
		// Find enemies in scene
		this.shootCount += Time.deltaTime;
		this.directionToggleCount += Time.deltaTime;

		if(this.directionToggleCount > this.directionToggleTime) {
			this.directionToggleCount = 0.0f;
			this.directionToggleTime = Random.Range(this.minDirectionToggleTime, this.maxDirectionToggleTime);
			if(Random.Range(0.0f, 1.0f) > this.directionToggleProbability) {
				this.directionToggle = !this.directionToggle;
			}
		}

        if (this.FollowEnter && !this.isInShootPreparation)
        {
			var deltaPlayer = this.hero.transform.position - this.transform.position;
            Debug.Log("Pursuit");
            Vector3 tempVect = deltaPlayer.normalized * this.moveSpeed * Time.deltaTime;
			float angle = Mathf.Atan2(deltaPlayer.y, deltaPlayer.x) * Mathf.Rad2Deg;
			this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
			if(!this.StopEnter) {
				if(!this.StopEnterEnemy) {
					tempVect = Quaternion.Euler(0.0f, 0.0f, 90.0f * (this.directionToggle?1.0f:-1.0f)) * tempVect;
            		this.thisEnemyRigidBody.MovePosition(this.transform.position + tempVect);
				} else {
					if(this.enemyEntered != null) {
            			tempVect = this.enemyEntered.transform.position - this.transform.position;
						tempVect = tempVect.normalized * this.moveSpeed * Time.deltaTime;
						tempVect = Quaternion.Euler(0.0f, 0.0f, 90.0f * (this.directionToggle?1.0f:-1.0f)) * tempVect;
            			this.thisEnemyRigidBody.MovePosition(this.transform.position + tempVect);
					}
				}
			}
        }

        if (this.FireEnter && this.shootCount > this.shootDuration)
        {
			this.isInShootPreparation = true;
			this.shootPreparationCounter = 0.0f;
        }

		if(this.isInShootPreparation) {
			this.shootPreparationCounter += Time.deltaTime;
		}

		if(this.shootPreparationCounter > this.shootPreparationDuration) {
			this.isInShootPreparation = false;
			this.shootPreparationCounter = 0.0f;
			var deltaPlayer = this.hero.transform.position - this.transform.position;
			var bullet = Instantiate(this.bulletPrefab);
			bullet.transform.position = this.bulletSpawnerPoint.transform.position;
			BulletMovement bulletMovementScript = bullet.GetComponent<BulletMovement>();
			bulletMovementScript.movementDirection = deltaPlayer;
		}

		if(this.shootCount > this.shootDuration) {
			this.shootCount = 0.0f;
		}
	}
}
