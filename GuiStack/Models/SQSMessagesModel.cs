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
    public class SQSMessagesModel
    {
        public IEnumerable<SQSMessage> Messages { get; set; }
        public string Prefix { get; set; }
    }
}
