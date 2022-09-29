/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;
using System.Collections.Generic;

namespace GuiStack.Models
{
    public class SQSMessage
    {
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

        public string Body { get; set; }
        public string MessageId { get; set; }
        public string MessageGroupId { get; set; }
        public string ReceiptHandle { get; set; }
        public DateTime? SentTimestamp { get; set; }
        public string SequenceNumber { get; set; }
    }
}
