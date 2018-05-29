using System;
using JetBrains.Annotations;

namespace DUCK.PackageManager.Editor.UI.Flux
{
	public class Action
	{
		public string Type { get; set; }
		public object Payload { get; set; }

		public Action(string type, object payload = null)
		{
			if (type == null) throw new ArgumentNullException("type");

			Type = type;
			Payload = payload;
		}
	}
}