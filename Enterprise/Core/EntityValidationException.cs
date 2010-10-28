#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core
{
    public class EntityValidationException : Exception
    {
    	private readonly string _message;
        private readonly TestResultReason[] _reasons;

        public EntityValidationException(string message, TestResultReason[] reasons)
        {
        	_message = message;
            _reasons = reasons;
        }

        public EntityValidationException(string message)
            :this(message, new TestResultReason[]{})
        {
        }

		public override string Message
		{
			get
			{
				List<string> messages = new List<string>();
				foreach (TestResultReason reason in _reasons)
					messages.AddRange(BuildMessageStrings(reason));

				if (messages.Count > 0)
				{
					return _message + "\n" + StringUtilities.Combine(messages, "\n");
				}
				else
					return _message;
			}
		}

        public TestResultReason[] Reasons
        {
            get { return _reasons; }
        }

        private static List<string> BuildMessageStrings(TestResultReason reason)
        {
            List<string> messages = new List<string>();
            if (reason.Reasons.Length == 0)
                messages.Add(reason.Message);
            else
            {
                foreach (TestResultReason subReason in reason.Reasons)
                {
                    List<string> subMessages = BuildMessageStrings(subReason);
                    foreach (string subMessage in subMessages)
                    {
                        messages.Add(string.Format("{0} {1}", reason.Message, subMessage));
                    }
                }
            }
            return messages;
        }
    }
}
