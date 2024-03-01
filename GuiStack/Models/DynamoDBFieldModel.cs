/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022-2024
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;

namespace GuiStack.Models
{
    public class DynamoDBFieldModel
    {
        public DynamoDBFieldType Type { get; set; }
        public object Value { get; set; }
    }

    public enum DynamoDBFieldType
    {
        Unknown = 0,
        Binary,
        BinarySet,
        Bool,
        List,
        Map,
        Null,
        Number,
        NumberSet,
        String,
        StringSet
    }
}
