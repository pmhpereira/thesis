using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace NodeEditorFramework 
{
	public static class NodeTypes
	{
		public static Dictionary<Node, NodeData> nodes;

		/// <summary>
		/// Fetches every Node Declaration in the assembly
		/// </summary>
		public static void FetchNodes() 
		{
			nodes = new Dictionary<Node, NodeData> ();
			var newNodes = new List<Node> ();

			List<Assembly> scriptAssemblies = AppDomain.CurrentDomain.GetAssemblies ().Where ((Assembly assembly) => assembly.FullName.Contains ("Assembly")).ToList ();
			if (!scriptAssemblies.Contains (Assembly.GetExecutingAssembly ()))
				scriptAssemblies.Add (Assembly.GetExecutingAssembly ());
			foreach (Assembly assembly in scriptAssemblies) 
			{
				foreach (Type type in assembly.GetTypes ().Where (T => T.IsClass && !T.IsAbstract && T.IsSubclassOf (typeof (Node)))) 
				{
					object[] nodeAttributes = type.GetCustomAttributes (typeof (NodeAttribute), false);
					NodeAttribute attr = nodeAttributes [0] as NodeAttribute;
					if (attr == null || !attr.hide)
					{
						Node node = ScriptableObject.CreateInstance (type.Name) as Node; // Create a 'raw' instance (not setup using the appropriate Create function)
						node = node.Create (Vector2.zero); // From that, call the appropriate Create Method to init the previously 'raw' instance
						nodes.Add (node, new NodeData (attr == null? node.name : attr.contextText));

						newNodes.Add(node);
					}
				}
			}

			newNodes = newNodes.OrderBy(x => (x.GetType().GetCustomAttributes (typeof (NodeAttribute), false)[0] as NodeAttribute).orderIndex).ToList();

			var newDict = new Dictionary<Node, NodeData> ();

			foreach(Node key in newNodes)
			{
				newDict[key] = nodes[key];
			}
			
			nodes = newDict;
		}

		public static NodeData getNodeData (Node node)
		{
			return nodes [getDefaultNode (node.GetID)];
		}

		public static Node getDefaultNode (string nodeID)
		{
			return nodes.Keys.Single<Node> ((Node node) => node.GetID == nodeID);
		}
		public static T getDefaultNode<T> () where T : Node
		{
			return nodes.Keys.Single<Node> ((Node node) => node.GetType () == typeof (T)) as T;
		}
	}

	public struct NodeData 
	{
		public string adress;

		public NodeData (string name) 
		{
			adress = name;
		}
	}

	public class NodeAttribute : Attribute 
	{
		public bool hide { get; private set; }
		public string contextText { get; private set; }
		public int orderIndex { get; private set; }

		public NodeAttribute (bool HideNode, string ReplacedContextText, int OrderIndex = 0) 
		{
			hide = HideNode;
			contextText = ReplacedContextText;
			orderIndex = OrderIndex;
		}
	}
}