using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Databox;

/// <summary>
/// Example for a custom class.
/// This example shows how to create custom editor gui to draw the values.
/// Thanks to EditorGUILayout we can create sliders or int textfields without converting string values. But keep in mind, EditorGUILayout won't work at runtime.
/// </summary>
[System.Serializable]
[DataboxTypeAttribute(Name = "CardType")]
public class CardType : DataboxType {

	[SerializeField]
	string _name;
	public string Name
    {
		get => _name;
        set
        {
			if (value == _name)
				return;

			_name = value;
			if (OnValueChanged != null) { OnValueChanged(this); }
		}
    }

	[SerializeField]
	string _desc;
	public string Description
    {
		get => _desc;
		set
		{
			if (value == _desc)
				return;

			_desc = value;

			if (OnValueChanged != null) { OnValueChanged(this); }
		}
	}

	[System.Serializable]
	public class Abilities
	{
		public bool Active;
		public string ID;

		public Abilities(string id)
		{
			ID = id;
		}
	}

	string _abilityId = "";

	[SerializeField]
	public List<Abilities> AbilityList = new List<Abilities>();

	public CardType ()
	{
		// default value
	}
	
	public override void DrawEditor()
	{
		using (new GUILayout.VerticalScope())
		{
			using (new GUILayout.HorizontalScope())
			{
#if UNITY_EDITOR
				_name = EditorGUILayout.TextField("Name: ", _name);
#endif
			}

			using (new GUILayout.VerticalScope())
			{
#if UNITY_EDITOR
				EditorGUILayout.LabelField("Description: ");
				_desc = EditorGUILayout.TextArea(_desc, GUILayout.MinHeight(100));
#endif
			}

			using (new GUILayout.VerticalScope("Box"))
			{
				GUILayout.Label("Abilities");

				_abilityId = GUILayout.TextField(_abilityId);
				if (GUILayout.Button("add"))
				{
					if (string.IsNullOrEmpty(_abilityId))
						return;

					AbilityList.Add(new Abilities(_abilityId));
				}

				for (int i = 0; i < AbilityList.Count; i++)
				{
					using (new GUILayout.HorizontalScope("Box"))
					{
						AbilityList[i].Active = GUILayout.Toggle(AbilityList[i].Active, AbilityList[i].ID);

						if (GUILayout.Button("x", GUILayout.Width(20)))
						{
							AbilityList.RemoveAt(i);
						}
					}
				}
			}
		}
	}
}
