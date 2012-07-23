﻿#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
	/// Associates a feature token with an action.
	/// </summary>
	/// <remarks>
	/// This attribute sets a feature identification token with an action, allowing the permissibility of the action
	/// to be controlled by application licensing in addition to the other rules of permissibility such as user
	/// authority tokens.
	/// </remarks>
	public class ActionFeatureAttribute : ActionDecoratorAttribute
	{
		private readonly string _featureToken;

		/// <summary>
		/// Associates a feature token with the specified action ID.
		/// </summary>
		/// <param name="actionId">String identifying the action to which this attribute applies.</param>
		/// <param name="featureToken">Feature identification token to be checked against application licensing.</param>
		public ActionFeatureAttribute(string actionId, string featureToken)
			: base(actionId)
		{
			_featureToken = featureToken;
		}

		/// <summary>
		/// Gets the feature identification token to be checked against application licensing.
		/// </summary>
		public string FeatureToken
		{
			get { return _featureToken; }
		}

		public override void Apply(IActionBuildingContext builder)
		{
			if (!string.IsNullOrEmpty(FeatureToken))
			{
				builder.Action.FeatureSpecification = new FeatureAuthorizationSpecification(FeatureToken);
			}
		}
	}
}