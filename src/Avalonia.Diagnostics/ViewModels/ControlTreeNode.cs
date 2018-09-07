﻿// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Diagnostics.ViewModels
{
    internal class ControlTreeNode : EventTreeNode
    {
        public ControlTreeNode(Type type, IEnumerable<RoutedEvent> events, EventsViewModel vm)
            : base(null, type.Name)
        {
            this.Children = new AvaloniaList<EventTreeNode>(events.OrderBy(e => e.Name).Select(e => new EventEntryTreeNode(this, e, vm) { IsEnabled = IsDefault(e) }));
            this.IsExpanded = true;
        }

        RoutedEvent[] defaultEvents = new RoutedEvent[]
        {
           Button.ClickEvent,
           InputElement.KeyDownEvent,
           InputElement.KeyUpEvent,
           InputElement.TextInputEvent,
           InputElement.PointerReleasedEvent,
           InputElement.PointerPressedEvent,
        };

        private bool IsDefault(RoutedEvent e)
        {
            return defaultEvents.Contains(e);
        }

        public override bool? IsEnabled
        {
            get => base.IsEnabled;
            set
            {
                if (base.IsEnabled != value)
                {
                    base.IsEnabled = value;
                    if (_updateChildren && value != null)
                    {
                        foreach (var child in Children)
                        {
                            try
                            {
                                child._updateParent = false;
                                child.IsEnabled = value;
                            }
                            finally
                            {
                                child._updateParent = true;
                            }
                        }
                    }
                }
            }
        }
    }
}
