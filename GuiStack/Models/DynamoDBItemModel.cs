/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2024
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;
using System.Collections.Generic;

namespace GuiStack.Models
{
    public class DynamoDBItemModel : Dictionary<string, DynamoDBFieldModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamoDBItemModel"/> class.
        /// </summary>
        public DynamoDBItemModel()
        {
        }
    }
}
