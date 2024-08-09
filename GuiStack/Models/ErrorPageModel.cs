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
    public class ErrorPageModel
    {
        public string Title { get; set; }
        public string Message { get; set; }

        public ErrorPageModel()
        {
        }

        public ErrorPageModel(string message)
        {
            Message = message;
        }

        public ErrorPageModel(string title, string message)
        {
            Title = title;
            Message = message;
        }
    }
}
