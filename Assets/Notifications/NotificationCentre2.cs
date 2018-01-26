using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NotificationCentre2:MonoBehaviour {

	public static List<Notification> notifications = new List<Notification>();

	//public readonly static NotificationCentre2 instance = new NotificationCentre2 ();
//	private NotificationCentre2(){}


	public void AddObserver(GameObject observer, string notficationName){
		AddObserver (observer, this.gameObject, notficationName);
	
	}

	public void AddObserver(GameObject observer, System.Object sender, string notificationName){

		if(string.IsNullOrEmpty(notificationName)){
			Debug.LogError("I can't observe an empty notification");
			return;
		}

		Notification notification = ConstructNotification (observer, sender, notificationName);

		if (notifications.Contains (notification)) {

			Debug.LogError (this.name + " says: this notification seems to already exist. Details as follows: Notification Name: " + notification.notificationName + " Notification sender " + notification.gameObject.name);		
			return;

		} else {
			
			notifications.Add (notification);

		}

	}

	public void RemoveObserver(GameObject gameobject, string notificationName){
		
	}

	public void PostNotification(string notificationName, System.Object sender){
		Debug.Log ("called");
		//List<GameObject> gameObjectsToNotify = new List<GameObject> ();

		foreach (Notification notification in notifications) {
			if (notification.notificationName == notificationName) {
				
				if (notification.senderGameObject == sender || notification.senderGameObject == this) {

					notification.observerGameObject.SendMessage (notificationName); 
					//gameObjectsToNotify.Add (notification.observerGameObject);

					Debug.Log (sender + " sent notification to " + notification.observerGameObject);
				}

			}
		
		}

		//return gameObjectsToNotify;
	}

	private Notification ConstructNotification(GameObject observer, System.Object sender, string notificationName){
		Notification notification = new Notification ();
		notification.observerGameObject = observer;
		notification.notificationName = notificationName;
		notification.senderGameObject = sender;

		return notification;
	}
		




}
