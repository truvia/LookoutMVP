using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//	<summary>
/// This delegate is similar to an Event Handler
/// The first parameter is the sender,
/// The second parameter is the arguments/info to pass
///	</summary>

using Handler = System.Action<System.Object, System.Object>;


///<summary>
/// The SenderTable maps from an object (sender of notification) to a List of Handler methods
/// When no sender is specified fro the SenderTable, the NotificationCenter itself is used as the sender key
///</summary>
using SenderTable = System.Collections.Generic.Dictionary<System.Object, System.Collections.Generic.List<System.Action<System.Object, System.Object>>>;
//
//public class NotificationCenter : MonoBehaviour {
//
///// <summary>
//	/// The dictionary "key" (string) represents a notificaitonName property to be obseved
//	/// Thedictionary "value" (SenderTable) maps between sender and observer sub tables
///// </summary>
//	private Dictionary<string, SenderTable> _table = new Dictionary<string, SenderTable>();
//	private HashSet<List<Handler>> _invoking = new HashSet<List<Handler>>();
//
//	public readonly static NotificationCenter instance = new NotificationCenter ();
//	private NotificationCenter(){}
//
//	public void AddObserver(Handler handler, string notificationName){
//		//If only two arguments, the Notification center itself is the sender of the message, this simply adds that.
//		AddObserver (handler, notificationName, null);
//	}
//
//	public void AddObserver(Handler handler, string notificationName, System.Object sender){
//		if (handler == null) {
//			Debug.LogError (this.name + " method AddObserver says: Can't add a null event handler, " + notificationName);
//			return;
//		}
//
//		if(string.IsNullOrEmpty(notificationName){
//			Debug.LogError(this.name + " says: I Can't observe an unnamed notification, silly");
//			return;
//		}
//
//			if(!_table.ContainsKey(notificationName)){
//				_table.Add(notificationName, new SenderTable());
//
//				SenderTable subTable = _table[notificationName];
//				System.Object key = (sender != null) ? sender : this; 
//
//				if(!subTable.ContainsKey(key)){
//					subTable.Add(key, new List<Handler>());
//
//					List<Handler> List = subTable[key];
//
//				}
//			}
//	
//	}
//}
