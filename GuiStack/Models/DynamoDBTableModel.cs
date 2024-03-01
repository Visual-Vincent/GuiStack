/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022-2024
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;
using Amazon;

namespace GuiStack.Models
{
    public class DynamoDBTableModel
    {
        public string Name { get; set; }
        public Arn Arn { get; set; }
        public long ItemCount { get; set; }
        public long TableSizeBytes { get; set; }
        public string TableClass { get; set; }
        public string BillingMode { get; set; }
        public long ReadCapacityUnits { get; set; }
        public long WriteCapacityUnits { get; set; }
        public bool DeletionProtectionEnabled { get; set; }
        public string Status { get; set; }

        public DynamoDBAttribute PartitionKey { get; set; }
        public DynamoDBAttribute SortKey { get; set; }
    }
}
