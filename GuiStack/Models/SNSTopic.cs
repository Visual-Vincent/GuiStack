/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022-2023
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;
using Amazon;

namespace GuiStack.Models
{
    public class SNSTopic
    {
        public string Name { get; set; }
        public Arn Arn { get; set; }

        public SNSTopic()
        {
        }

        public SNSTopic(string arn)
        {
            Arn = Arn.Parse(arn);
            Name = Uri.UnescapeDataString(Arn.Resource);
        }
    }
}
