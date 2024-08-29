/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2024
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;

namespace GuiStack.Models
{
    public class DynamoDBAttribute
    {
        public string Name { get; set; }
        public DynamoDBAttributeType Type { get; set; }

        public DynamoDBAttribute()
        {
        }

        public DynamoDBAttribute(string name, DynamoDBAttributeType type)
        {
            Name = name;
            Type = type;
        }
    }

    public enum DynamoDBAttributeType
    {
        Unknown = 0,
        String,
        Number,
        Binary
    }
}
