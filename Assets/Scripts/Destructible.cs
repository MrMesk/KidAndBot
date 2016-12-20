using UnityEngine;
using System.Collections;

public class Destructible : MonoBehaviour
{
	public enum DestructibleSize
	{
		small,
		medium,
		large,
		huge
	}
	public DestructibleSize propSize;
	public bool isBreakable;
	public int lifePoints;

	public class BreakableOptions
	{
		public GameObject destroyParticle;
		public GameObject impactParticle;
		[FMODUnity.EventRef]
		public string destroySound = "event:/Step";
		public string impactSound = "event:/Step";
	}

	public BreakableOptions breaking = new BreakableOptions();

	/// <summary>
	/// Checks actual prop Life Points and destroys the object if life points fall below 1.
	/// Returns if the bot keep running after colliding
	/// </summary>
	/// <returns>Does the bot keep running after colliding ?</returns>

	//Can add damage property depending on charge velocity
	public bool Impact()
	{
		bool doesContinueCharging = true;
		if (isBreakable)
		{
			lifePoints--;
			if(lifePoints <= 0)
			{
				FMODUnity.RuntimeManager.PlayOneShot(breaking.destroySound, transform.position);
				Instantiate(breaking.destroyParticle, transform.position, Quaternion.identity);
				Destroy(gameObject);
			}
			else
			{
				FMODUnity.RuntimeManager.PlayOneShot(breaking.impactSound, transform.position);
				Instantiate(breaking.impactParticle, transform.position, Quaternion.identity);
			}

			switch (propSize)
			{
				case DestructibleSize.small:
				doesContinueCharging = true;
				break;

				case DestructibleSize.medium:
				if (lifePoints > 0)
				{
					doesContinueCharging = true;
				}
				else
				{
					doesContinueCharging = false;
				}

				break;

				case DestructibleSize.large:
				if (lifePoints > 0)
				{
					doesContinueCharging = true;
				}
				else
				{
					doesContinueCharging = false;
				}
				break;

				case DestructibleSize.huge:
				doesContinueCharging = false;
				break;
			}

		}
		else
		{
			FMODUnity.RuntimeManager.PlayOneShot(breaking.impactSound, transform.position);
			//Instantiate(breaking.impactParticle, transform.position, Quaternion.identity);
			doesContinueCharging = false;
		}

		Debug.Log("Is still charging ? " + doesContinueCharging);
		return doesContinueCharging;
	}
}
