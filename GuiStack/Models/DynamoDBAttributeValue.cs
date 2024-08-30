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
    public class DynamoDBAttributeValue
    {
        public string Name { get; set; }
        public DynamoDBFieldModel Value { get; set; }

        public DynamoDBAttributeValue()
        {
        }

        public DynamoDBAttributeValue(string name, DynamoDBFieldModel value)
        {
            Name = name;
            Value = value;
        }
    }
}
