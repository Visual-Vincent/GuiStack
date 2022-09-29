/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;
using System.Linq;

namespace GuiStack.Models
{
    public class SQSQueue
    {
        public string Name { get; set; }
        public string Url { get; set; }

        public SQSQueue()
        {
        }

        public SQSQueue(string url)
        {
            Uri uri = new Uri(url);

            Url = uri.ToString();
            Name = string.Join("", uri.Segments.Skip(2));
        }
    }
}
