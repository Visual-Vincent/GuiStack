/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022-2024
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;
using System.Collections.Generic;

namespace GuiStack.Models
{
    public class DynamoDBTableContentsModel
    {
        public DynamoDBAttribute PartitionKey { get; set; }
        public DynamoDBAttribute SortKey { get; set; }
        public string[] AttributeNames { get; set; }
        public string LastEvaluatedKey { get; set; }

        public IEnumerable<DynamoDBItemModel> Items { get; set; }
    }
}
