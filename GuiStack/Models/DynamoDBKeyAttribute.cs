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
    public class DynamoDBKeyAttribute
    {
        public string Name { get; set; }
        public DynamoDBKeyAttributeType Type { get; set; }

        public DynamoDBKeyAttribute()
        {
        }

        public DynamoDBKeyAttribute(string name, DynamoDBKeyAttributeType type)
        {
            Name = name;
            Type = type;
        }
    }

    public enum DynamoDBKeyAttributeType
    {
        Unknown = 0,
        String,
        Number,
        Binary
    }
}
