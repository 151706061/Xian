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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// This class is experimental, does not entirely work right now.
    /// </summary>
    public partial class SmartTimeField : UserControl
    {
        private List<TimeSpan> _choices = new List<TimeSpan>();

        private TimeSpan _minimum = TimeSpan.Zero + TimeSpan.FromHours(7);      // 7am
        private TimeSpan _maximum = TimeSpan.Zero + TimeSpan.FromHours(7+24);   // 7am tomorrow
        private TimeSpan _interval = TimeSpan.FromMinutes(30);  // 30 mins

        public SmartTimeField()
        {
            InitializeComponent();
            _input.SuggestionProvider = new DefaultSuggestionProvider<TimeSpan>(_choices, FormatTimeSpan);
            _input.Format += new ListControlConvertEventHandler(InputFormatEventHandler);
        }


        public TimeSpan Interval
        {
            get { return _interval; }
            set
            {
                _interval = value;
                if (!DesignMode)
                {
                    GenerateChoices();
                }
            }
        }

        public TimeSpan Minimum
        {
            get { return _minimum; }
            set
            {
                _minimum = value;
                if (!DesignMode)
                {
                    GenerateChoices();
                }
            }
        }

        public TimeSpan Maximum
        {
            get { return _maximum; }
            set
            {
                _maximum = value;
                if (!DesignMode)
                {
                    GenerateChoices();
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            if(!this.DesignMode)
            {
                GenerateChoices();
            }

            base.OnLoad(e);
        }

        private void GenerateChoices()
        {
            _choices.Clear();
            for (TimeSpan value = _minimum; value < _maximum; value += _interval)
                _choices.Add(value);
        }

        private string FormatTimeSpan(TimeSpan ts)
        {
            DateTime time = DateTime.Today + ts;
            return Format.Time(time);
        }

        private void InputFormatEventHandler(object sender, ListControlConvertEventArgs e)
        {
            e.Value = FormatTimeSpan((TimeSpan)e.ListItem);
        }

    }
}
