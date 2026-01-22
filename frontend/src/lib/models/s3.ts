/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2025-2026
 * https://github.com/Visual-Vincent/GuiStack
 */

export interface Bucket
{
    name: string;
    creationDate: Date;
}

export interface Object
{
    name: string;
    size: number;
    lastModified: Date;
    s3Uri: string;
    url: string;
}
