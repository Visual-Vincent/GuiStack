/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022-2023
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;
using GuiStack.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GuiStack.Pages.SNS
{
    public class IndexModel : PageModel
    {
        public ISNSRepository SNSRepository { get; }

        [BindProperty(SupportsGet = true)]
        public string Topic { get; set; }

        public IndexModel(ISNSRepository snsRepository)
        {
            this.SNSRepository = snsRepository;
        }

        public void OnGet()
        {
        }
    }
}
