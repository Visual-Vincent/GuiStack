/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;
using GuiStack.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GuiStack.Pages.S3
{
    public class IndexModel : PageModel
    {
        public IS3Repository S3Repository { get; }

        [BindProperty(SupportsGet = true)]
        public string Bucket { get; set; }

        public IndexModel(IS3Repository s3Repository)
        {
            this.S3Repository = s3Repository;
        }

        public void OnGet()
        {
        }
    }
}
